using ApiMultas.Data;
using ApiMultas.Models;
using ApiMultas.Models.Agentes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultas.Controllers
{
    [Route("api/agentes")]
    public class AgentesController : Controller
    {
        private readonly ApplicationDbContext db;
        
        public AgentesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        #region CRUD - Read
        /// <summary>
        /// GET /api/agentes
        /// 
        /// Devolve uma lista de agentes, num array.
        /// 
        /// Opcionalmente, a lista pode ser filtrada por um termo de pesquisa,
        /// presente na Query String, que incide sobre o nome do agente, ou na sua esquadra.
        /// 
        /// É também possível filtrar só agentes que têm foto, usando o parâmetro na query string
        /// 'comFoto'. Um valor de 'null' neste campo não incluí esta pesquisa.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string pesquisa, [FromQuery] bool? comFoto)
        {
            // Linq - queries dinâmicas.
            // Muitas vezes, os parâmetros de pesquisa são opcionais.
            // Logo, podemos compor uma query Linq começando a partir da
            // tabela dos agentes.
            IQueryable<Agentes> query = db.Agentes;

            // Se o termo de pesquisa estiver definido, então compõe-se
            // a query com um .Where para filtrar no nome, usando o .Contains
            // (LIKE em SQL)
            // O string.IsNullOrWhiteSpace(texto) devolve verdadeiro se
            // texto != null && texto != "" && texto.Trim() != ""
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                // Como queries são imutáveis, guardamos a nova query na variável acima.
                query = query.Where(a => a.Nome.Contains(pesquisa) || a.Esquadra.Contains(pesquisa));
            }

            // Podemos fazer várias composições dinâmicamente, e não só com o .Where.
            if (comFoto == true)
            {
                query = query.Where(a => a.Fotografia != null);
            }

            // Usar o .Select para remover as referências circulares
            // Agentes <-> Multas, que iriam fazer com que ocorressem
            // erros ao produzir o JSON.
            var resultado = query
                .Select(a => new
                {
                    a.ID,
                    a.Nome,
                    a.Esquadra,
                    a.Fotografia,
                    // Não incluo a Lista de Multas aqui.
                    // Eu posso inclui-la desde que não coloque na multa
                    // a propriedade do agente.
                    // Neste caso não o faço por questões de desempenho
                    // (menos dados a passar na BD e em JSON).
                    // Devemos limitar os dados que obtemos e devolvemos.
                })
                .ToList();

            return Ok(resultado);
        }

        /// <summary>
        /// GET /api/agentes/{id}
        /// 
        /// Devolve um agente com um determinado ID, e as suas multas. O ID é passado
        /// no endereço.
        /// 
        /// Devolve 404 se o agente não existe.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOneById(int id)
        {
            // Vou usar mais uma vez o .Select para impedir referências circulares e seleccionar
            // APENAS os dados que preciso.
            // Depois, faço uso do .FirstOrDefault() para obter apenas um agente (ou null, se não existe).
            var agente = db.Agentes
                .Where(a => a.ID == id)
                .Select(a => new
                {
                    a.ID,
                    a.Nome,
                    a.Esquadra,
                    a.Fotografia,
                    // Posso usar o Linq dentro de queries de Linq,
                    // para fazer subqueries.
                    ListaDeMultas = a.ListaDeMultas
                        .Select(m => new
                        {
                            m.ID,
                            m.Infracao,
                            m.ValorMulta,
                            m.LocalDaMulta,
                            m.DataDaMulta,
                            // Posso incluir as entidades referênciadas da Multa,
                            // (como a Viatura eo  Condutor) desde que faça sentido
                            // (necessite dos dados) e que o resultado não fique muito pesado.
                            // (Muitos dados ou muitas queries).
                            Viatura = new
                            {
                                m.Viatura.ID,
                                m.Viatura.Marca,
                                m.Viatura.Matricula,
                            },
                            Condutor = new
                            {
                                m.Condutor.ID,
                                m.Condutor.Nome
                            },
                            // Não incluo a referência ao Agente para impedir a referência circular.
                        })
                        .ToList()
                })
                .FirstOrDefault();

            // Enquanto que em MVC eu devolvo uma página, ou faço redirect,
            // APIs são usadas por código, por isto deve ser tratado como uma "exceção"
            // (404 Not Found)
            if (agente == null)
            {
                return NotFound(new ErroApi { Mensagem = "O agente com ID " + id + " não existe." });
            }

            // Se chegarmos aqui, está tudo OK.
            return Ok(agente);
        }
        #endregion

        #region CRUD - Create
        /// <summary>
        /// POST /api/agentes
        /// Cria um agente a partir dos dados do formulário.
        /// 
        /// Devolve 201 Created se o agente foi criado com sucesso.
        /// Devolve 400 Bad Request se os dados de input não forem válidos.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromForm] CreateAgenteModel model)
        {
            // Da mesma forma que no MVC valido erros, faço o mesmo na API.
            // A diferença é que geramos uma "exceção" (400 Bad Request) em vez de
            // mostrarmos uma view.
            if (!ModelState.IsValid)
            {
                // Passar o ModelState para o BadRequest() faz 
                // com que os erros de validação fiquem no output.
                return BadRequest(ModelState);
            }

            var pastaFotografias = CaminhoParaFotos();

            if (!Directory.Exists(pastaFotografias))
            {
                Directory.CreateDirectory(pastaFotografias);
            }

            // Gerar caminho da fotografia (FotosAgentes/<guid>.jpg)
            var nomeFicheiroFoto = Guid.NewGuid().ToString() + Path.GetExtension(model.Fotografia.FileName);

            var caminhoFoto = Path.Combine(pastaFotografias, nomeFicheiroFoto);

            // Guardar os dados num ficheiro, copiando o output para o stream
            // infelizmente não existe o método SaveAs().
            using (Stream output = System.IO.File.OpenWrite(caminhoFoto))
            {
                model.Fotografia.CopyTo(output);
            }

            // Guardar o agente na BD.
            var novoAgente = new Agentes
            {
                Nome = model.Nome,
                Esquadra = model.Esquadra,
                Fotografia = nomeFicheiroFoto,
            };

            db.Agentes.Add(novoAgente);

            db.SaveChanges();

            // Criar o output.
            // Apesar de, neste caso, não existir risco de 
            // referências circulares, não custa nada fazer um output correto.
            var resultado = new
            {
                novoAgente.ID,
                novoAgente.Nome,
                novoAgente.Fotografia,
                novoAgente.Esquadra
            };

            // Se quisermos ser 100% corretos numa API,
            // então devemos indicar ONDE foi criado o objeto que 
            // se submeteu, usando o Status Code 201 Created.
            // Isto indica sucesso, tal como o 200 OK, mas também indica
            // no cabeçalho "Location", o link para o novo objeto.
            // Se quiserem simplificar, usem 200 OK.
            // O CreatedAtAction pode ser usado para isto. Tem os seguintes parâmetros:
            // 1. Nome do Action do controller.
            // 2. Parâmetros a enviar (para construir o link)
            // 3. Dados a enviar (como se fosse no OK)
            return CreatedAtAction("GetOneById", new { id = resultado.ID }, resultado);

            // return Ok(resultado);
        }
        #endregion

        #region CRUD - Update
        /// <summary>
        /// PUT /api/agentes/{id}
        /// 
        /// Permite atualizar um agente, dado o seu ID, e os novos dados
        /// no body (como JSON).
        /// 
        /// Devolve 204 No Content se o agente for atualizado com sucesso.
        /// Devolve 400 Bad Request se existirem erros de validação.
        /// Devolve 404 Not Found se o agente não existe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult UpdateAgente(int id, [FromBody] UpdateAgenteModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound(new ErroApi("Não é possível atualizar o agente " + id + " porque não existe."));
            }

            // Atualizar...
            agente.Nome = model.Nome;

            db.Agentes.Update(agente); // OU db.Entry(agente).State = EntityState.Modified;

            db.SaveChanges();

            // Porque sou preguiçoso, vou devolver 204 (No Content)
            // em vez de 200 OK com os dados.
            // 204 é como o 200, excepto que não se envia nada no body.
            return NoContent();
        }
        #endregion

        #region CRUD - Delete
        /// <summary>
        /// DELETE /api/agentes/{id}
        /// 
        /// Permite apagar um agente, dado o seu ID.
        /// 
        /// Devolve:
        /// - 204 No Content se for apagado com sucesso.
        /// - 404 Not Found se não existir.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteAgenteById(int id)
        {
            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound(new ErroApi("Não é possível apagar o agente " + id + " porque não existe."));
            }

            // Apagar a foto, se o agente tiver foto...
            if (agente.Fotografia != null)
            {
                var caminhoFoto = Path.Combine(CaminhoParaFotos(), agente.Fotografia);

                try
                {
                    System.IO.File.Delete(caminhoFoto);
                }
                catch (IOException)
                {
                    // Nada -- mas o ficheiro da foto pode não ser apagado com sucesso!
                }
            }

            // Apagar da BD...
            db.Agentes.Remove(agente);

            db.SaveChanges();

            return NoContent();
        }
        #endregion

        #region Fotografias
        /// <summary>
        /// GET /api/agentes/{id}/foto
        /// 
        /// Permite obter a foto de um agente, dado o seu ID.
        /// 
        /// Devolve:
        /// - 200 OK se o agente existir.
        /// - 404 Not Found caso contrário.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/foto")]
        public IActionResult GetFotografiaById(int id)
        {
            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound(new ErroApi("Não é possível obter a fotografia do agente " + id + " porque este não existe."));
            }

            // Um agente pode não ter foto, por isso temos que ter cuidado.
            var foto = agente.Fotografia;

            if (foto == null)
            {
                foto = "agente.jpg";
            }

            var caminhoFoto = Path.Combine(CaminhoParaFotos(), foto);

            // O método PhysicalFile do controller permite fazer download de um ficheiro numa
            // determinada pasta, dado o seu caminho. O caminho tem que ser ABSOLUTO.
            // (o método File é igual, mas usa a directoria wwwroot, e usa um caminho relativo a essa pasta)
            return PhysicalFile(caminhoFoto, "image/jpeg");
        }
        #endregion

        /// <summary>
        /// Devolve o caminho ABSOLUTO para a pasta das fotos dos agentes.
        /// </summary>
        /// <returns></returns>
        private string CaminhoParaFotos()
        {
            var fullPath = Path.GetFullPath("FotosAgentes");

            return fullPath;
        }
    }
}
