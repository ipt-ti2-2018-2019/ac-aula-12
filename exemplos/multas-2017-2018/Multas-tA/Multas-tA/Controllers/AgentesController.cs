using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Multas.ApiViewModels;
using Multas.Models;
using Multas.ViewModels;

namespace Multas.Controllers
{
    public class AgentesController : Controller
    {

        // cria um objeto privado, que representa a base de dados
        private MultasDb db = new MultasDb();

        // GET: Agentes
        public ActionResult Index()
        {

            // (LINQ)db.Agente.ToList() --> em SQL: SELECT * FROM Agentes ORDER BY 
            // constroi uma lista com os dados de todos os Agentes
            // e envia-a para a View

            var listaAgentes = db.Agentes.OrderBy(a => a.Nome).ToList();

            return View(listaAgentes);
        }

        // GET: Agentes/Details/5
        /// <summary>
        /// Apresenta os detalhes de um Agente
        /// </summary>
        /// <param name="id"> representa a PK que identifica o Agente </param>
        /// <returns></returns>
        public ActionResult Details(int? id)
        {

            // int? - significa que pode haver valores nulos

            // protege a execução do método contra a Não existencia de dados
            if (id == null)
            {

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                // ou não foi introduzido um ID válido,
                // ou foi introduzido um valor completamente errado
                //return RedirectToAction("Index");

                ViewBag.MensagemErro = "O link para o Agente não é válido.";
                return View("AgenteErro");
            }

            // vai procurar o Agente cujo ID foi fornecido
            Agentes agente = db.Agentes.Find(id);

            // se o Agente NÃO for encontrado...
            if (agente == null)
            {
                // return HttpNotFound();
                //return RedirectToAction("Index");

                ViewBag.MensagemErro = "Este Agente não existe, ou não tem permissão para ver os seus dados.";
                return View("AgenteErro");
            }

            // envia para a View os dados do Agente
            return View(agente);
        }





        // GET: Agentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agentes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAgenteViewModel model)
        {
            // ModelState.IsValid --> confronta os dados fornecidos com o modelo
            // se não respeitar as regras do modelo, rejeita os dados
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var agente = new Agentes
            {
                ID = db.GetIdAgente(),
                Nome = model.Nome,
                Esquadra = model.Esquadra
            };

            //    verificar se o ficheiro é realmente uma imagem ---> Ver a classe "CreateAgenteViewModel"
            //    redimensionar a imagem --> ver em casa (pode ser feito com validação custom)

            // var. auxiliar
            string nomeFotografia = "Agente_" + agente.ID + Path.GetExtension(model.Fotografia.FileName);
            string caminhoParaFotografia = Path.Combine(Server.MapPath("~/imagens/"), nomeFotografia); // indica onde a imagem será guardada

            // guardar o nome da imagem na BD
            agente.Fotografia = nomeFotografia;
            
            try
            {
                // guardar a imagem no disco rígido
                model.Fotografia.SaveAs(caminhoParaFotografia);

                // adiciona na estrutura de dados, na memória do servidor,
                // o objeto Agentes
                db.Agentes.Add(agente);
                // faz 'commit' na BD
                db.SaveChanges();

                // redireciona o utilizador para a página de início
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // gerar uma mensagem de erro para o utilizador
                ModelState.AddModelError("", "Ocorreu um erro não determinado na criação do novo Agente...");
            }

            // se se chegar aqui, é pq aconteceu algum problema...
            // devolve os dados do agente à View
            return View(agente);
        }
        
        // GET: Agentes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }

            Agentes agentes = db.Agentes.Find(id);
            if (agentes == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }

            var model = new EditAgenteViewModel
            {
                ID = agentes.ID,
                Esquadra = agentes.Esquadra,
                FotografiaAtual = agentes.Fotografia
            };

            return View(model);
        }




        // POST: Agentes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Editar os dados de um Agente
        /// </summary>
        /// <param name="agente"> dados (nome + esquadra) do Agente a editar </param>
        /// <param name="uploadFoto"> ficheiro (opcional) com a fotografia do Agente a editar (.jpg) </param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditAgenteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Obter o agente a editar, e produzir um erro se não existir.
            var agente = db.Agentes.Find(model.ID);

            if (agente == null)
            {
                ModelState.AddModelError("", "O agente não existe.");

                return View(model);
            }

            /// como se pretende editar os dados de um Agente,
            /// tem de haver a hipótese de se editar a fotografia dele.
            /// Por esse motivo, é necessário adicionar aos parâmetros de entrada do método
            /// uma variável do tipo HttpPostedFileBase para receber o ficheiro da imagem.
            /// É igualmente necessário adicionar, na View, um objeto do tipo <input type="file" />
            /// para possibilitar a escolha da imagem a efetuar upload.
            /// O nome da variável no método do controller e na view tem de ser o mesmo.
            /// 
            /// De igual forma, é necessário alterar a forma como se configura o objeto do tipo <form />,
            /// responsável por enviar os dados do browser para o servidor,
            /// adicionando-lhe o parâmetro 'enctype = "multipart/form-data" '

            /// quando se faz uma simples substituição de uma fotografia por outra,
            /// mantendo o nome original, nem sempre os browsers atualizam, no ecrã,
            /// a nova imagem, pela forma como fazem a gestão da 'cache'.
            /// Por isso, é frequente alterar-se o nome da nova imagem, adicionando-lhe um termo
            /// associado à data+hora da alteração.

            // vars. auxiliares
            string novoNome = "";
            string nomeAntigo = "";

            if (ModelState.IsValid)
            {
                try
                {              /// se foi fornecida uma nova imagem,
                               /// preparam-se os dados para efetuar a alteração
                    if (model.Fotografia != null)
                    {
                        /// antes de se fazer alguma coisa, preserva-se o nome antigo da imagem,
                        /// para depois a remover do disco rígido do servidor
                        nomeAntigo = agente.Fotografia;
                        /// para o novo nome do ficheiro, vamos adicionar o termo gerado pelo timestamp
                        /// devidamente formatado, mais
                        /// A extensão do ficheiro é obtida automaticamente em vez de ser escrita de forma explícita
                        novoNome = "Agente_" + agente.ID + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + Path.GetExtension(model.Fotografia.FileName).ToLower(); ;
                        /// atualizar os dados do Agente com o novo nome
                        agente.Fotografia = novoNome;
                        /// guardar a nova imagem no disco rígido
                        model.Fotografia.SaveAs(Path.Combine(Server.MapPath("~/imagens/"), novoNome));
                    }

                    // Passar os dados do modelo para o agente
                    agente.Esquadra = model.Esquadra;

                    // guardar os dados do Agente
                    db.Entry(agente).State = EntityState.Modified;
                    // Commit
                    db.SaveChanges();

                    /// caso tenha sido fornecida uma nova imagem há necessidade de remover 
                    /// a antiga
                    if (model.Fotografia != null)
                    {
                        System.IO.File.Delete(Path.Combine(Server.MapPath("~/imagens/"), nomeAntigo));
                    }

                    // enviar os dados para a página inicial
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    // caso haja um erro deve ser enviada uma mensagem para o utilizador
                    ModelState.AddModelError("", string.Format("Ocorreu um erro com a edição dos dados do agente {0}", agente.Nome));
                }
            }

            // Preencher novamente os campos que se possam ter perdido...
            model.FotografiaAtual = agente.Fotografia;

            return View(model);
        }



        // GET: Agentes/Delete/5
        /// <summary>
        /// Procura os dados de um Agente,
        /// e mostra-os no ecrã
        /// </summary>
        /// <param name="id"> identificador do Agente a pesquisar </param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            Agentes agentes = db.Agentes.Find(id);
            if (agentes == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index");
            }
            return View(agentes);
        }




        // POST: Agentes/Delete/5
        /// <summary>
        /// concretiza, torna definitiva (qd possível)
        /// a remoção de um Agente
        /// </summary>
        /// <param name="id"> o identificador do Agente a remover</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // procurar o Agente
            Agentes agente = db.Agentes.Find(id);

            try
            {
                // remover da memória
                db.Agentes.Remove(agente);
                // commit na BD
                db.SaveChanges();
                // redirecionar para a página inicial
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                // gerar uma mensagem de erro, a ser apresentada ao utilizador
                ModelState.AddModelError("",
                           string.Format("Não foi possível remover o Agente '{0}', porque existem {1} multas associadas a ele.",
                                          agente.Nome, agente.ListaDeMultas.Count));
            }

            // reenviar os dados para a View
            return View(agente);
        }






        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
