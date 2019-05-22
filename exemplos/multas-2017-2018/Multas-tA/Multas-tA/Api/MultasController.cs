using Multas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Multas.Api
{
    [RoutePrefix("api/multas")]
    public class MultasController : ApiController
    {
        private MultasDb db = new MultasDb();
        
        // GET: /api/multas/exemplo-1-pesquisa-simples
        [HttpGet, Route("exemplo-1-pesquisa-simples")]
        public IHttpActionResult Exemplo1PesquisaSimples(string nomeInfracao = null, string nomeAgente = null)
        {
            // Este exemplo mostra como pesquisar por multas
            // através de um conjunto de critérios opcionais.
            // Os parâmetros são todos opcionais (daí o = null),
            // e podem ser passados na query string do URL.

            IQueryable<Models.Multas> query = db.Multas;

            // Se o nome da infração estiver explícito,
            // fazer um Contains (traduzido num Like em SQL)
            if (nomeInfracao != null)
            {
                query = query
                    .Where(multa => multa.Infracao.Contains(nomeInfracao));
            }

            // Eu também posso aplicar o Where e outros operadores em sub-objetos
            if (nomeAgente != null)
            {
                query = query
                    .Where(multa => multa.Agente.Nome == nomeAgente);
            }

            // Por último, aplico o Select para prevenir referências circulares.
            var resultado = query
                .Select(multa => new
                {
                    multa.ID,
                    multa.Infracao,
                    multa.DataDaMulta,
                    multa.LocalDaMulta,
                    // Desde que tenha cuidado, eu posso ter referências para
                    // os objetos relacionados.
                    Viatura = new
                    {
                        multa.Viatura.ID,
                        multa.Viatura.Marca,
                        multa.Viatura.Modelo,
                        multa.Viatura.Matricula
                    },
                    Agente = new
                    {
                        multa.Agente.ID,
                        multa.Agente.Nome
                    },
                    Condutor = new
                    {
                        multa.Condutor.ID,
                        multa.Condutor.Nome
                    },
                    multa.ValorMulta
                });

            return Ok(resultado.ToList());
        }

        // GET: /api/multas/exemplo-2-valorespormes-groupby
        [HttpGet, Route("exemplo-2-valorespormes-groupby")]
        public IHttpActionResult Exemplo2ValoresPorMesGroupBy(int? numeroMes = null)
        {
            // Este exemplo mostra como fazer uso do GroupBy do Linq
            // para obter valores de multas por mês,
            // e também fazer o somatório dos valores.

            var query = db.Multas
                .GroupBy(
                    // Agrupar pelo mês da multa.
                    multa => multa.DataDaMulta.Month,  
                    // 'mes' é o nº do mês, 'multasDoMes' são as multas desse mês.
                    (mes, multasDoMes) => new          
                    {
                        Mes = mes,
                        
                        // No 'multasDoMes' posso aplicar Linq novamente
                        // para, por exemplo, extrair apenas o valor...
                        ValoresMultas = multasDoMes
                            .Select(multa => multa.ValorMulta),

                        // Ou fazer um somatório dos seus valores.
                        TotalValorMultas = multasDoMes
                            .Sum(multa => multa.ValorMulta)
                    }
                );

            // Se o utilizador especificou o nº do mês (valor != null),
            // então vamos aplicar na query original um Where para filtrar as multas
            // desse mês.
            if (numeroMes != null)
            {
                query = query.Where(resultadoMes => resultadoMes.Mes == numeroMes);
            }

            // ToList() executa a query.
            return Ok(query.ToList());
        }

        // GET: /api/multas/exemplo-3-paginacao
        [HttpGet, Route("exemplo-3-paginacao")]
        public IHttpActionResult Exemplo3Paginacao(string infracao = null, int numeroPagina = 1, int tamanhoPagina = 10)
        {
            // Este exemplo aplica questões de paginação.
            // Paginação é importante por questões de desempenho;
            // se tiver 1 milhão de registos na base de dados,
            // não é viável devolvê-los todos de uma vez.
            // Paginação é uma solução para o problema.

            // Este exemplo, além de aplicar o Skip e o Take, 
            // também aplica o Count para indicar ao cliente quantos elementos existem
            // na base de dados no total. Isto é usado para que o cliente possa calcular
            // quantas 'páginas' existem.

            IQueryable<Models.Multas> query = db.Multas;

            // 1) Aplicar filtros. Devem-se aplicar os filtros o quanto antes...
            if (infracao != null)
            {
                // Filtrar pelo nome (LIKE).
                query = query.Where(multa => multa.Infracao.Contains(infracao));
            }

            // 2) Contar o nº de itens que existem para os termos de pesquisa especificados
            //    na nossa fonte de dados.
            var totalMultas = query.Count();

            // 3) Convém, mas não é obrigatório, ter os dados ordenados.
            //    Neste caso, estou a ordenar pela data,
            //    de forma a que as multas mais recentes apareçam primeiro.
            //    Se souberem que a vossa fonte de dados já produz os dados ordenados
            //    como pretendem, devem omitir este passo.
            //    Operações de ordenação são caras!
            query = query.OrderByDescending(multa => multa.DataDaMulta);

            // 4) Calcular o valor a colocar no Skip.
            //    Se a página 1 corresponde ao início dos dados,
            //    é preciso aplicar alguma matemática.
            //    Considerando tamanho 10, deve ser algo como:
            //    - pagina 1: skip 0
            //    - pagina 2: skip 10
            //    - pagina 3: skip 20
            //    - etc.
            var valorSkip = (numeroPagina - 1) * tamanhoPagina;

            query = query
                .Skip(valorSkip)
                .Take(tamanhoPagina);

            // 5) Aplico o Select para controlar o objeto resultante.
            var paginaDeMultas = query
                .Select(multa => new
                {
                    multa.ID,
                    multa.Infracao,
                    multa.DataDaMulta,
                    multa.LocalDaMulta,
                    multa.ValorMulta
                })
                .ToList();

            // 6) Crio um objeto que tem informações sobre a paginação.
            //    Este objeto deve ter, no mínimo, o nº total de itens,
            //    o resultado, o nº da página, e o tamanho da página.
            var resultadoPaginacao = new
            {
                NumItens = totalMultas,
                Itens = paginaDeMultas,
                NumPagina = numeroPagina,
                TamanhoPagina = tamanhoPagina
            };

            return Ok(resultadoPaginacao);
        }

        // GET: /api/multas/exemplo-4-join-multas-com-agentes
        [HttpGet, Route("exemplo-4-join-multas-com-agentes")]
        public IHttpActionResult Exemplo4JoinMultasComAgentes(string localDaMulta = null, string nomeDaEsquadra = null)
        {
            // Este exemplo faz uso do Join para combinar as multas com os agentes.
            
            var query = db.Multas
                .Join(
                    db.Agentes, // Tabela dos agentes
                    multa => multa.AgenteFK, // Multa -> ID Agente
                    agente => agente.ID, // Agente -> ID Agente
                    (multa, agente) => new // Objeto resultante da combinação dos dois (Select).
                    {
                        IdAgente = agente.ID,
                        IdMulta = multa.ID,
                        NomeAgente = agente.Nome,
                        EsquadraAgente = agente.Esquadra,
                        LocalMulta = multa.LocalDaMulta,
                        ValorMulta = multa.ValorMulta
                    }
                );

            // Mais uma vez, posso aplicar os operadores pela ordem que quiser.
            if (localDaMulta != null)
            {
                query = query.Where(multaComAgente => multaComAgente.LocalMulta == localDaMulta);
            }

            // Múltiplos Where são como AND.
            if (nomeDaEsquadra != null)
            {
                query = query.Where(multaComAgente => multaComAgente.EsquadraAgente == nomeDaEsquadra);
            }

            return Ok(query.ToList());
        }

        // GET: /api/multas/exemplo-5-top-multas-orderbydescending
        [HttpGet, Route("exemplo-5-top-multas-orderbydescending")]
        public IHttpActionResult Exemplo5TopMultasOrderByDescending()
        {
            // Este exemplo faz uso do OrderByDescending para se obter um 'top' das multas,
            // pelo seu custo.
            var query = db.Multas
                .OrderByDescending(multa => multa.ValorMulta);

            var resultado = query
                .Select(multa => new
                {
                    multa.ID,
                    multa.Infracao,
                    multa.DataDaMulta,
                    multa.LocalDaMulta,
                    multa.ValorMulta
                });

            return Ok(resultado.ToList());
        }
    }
}