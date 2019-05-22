using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Multas.Models;
using Multas.ViewModels;

namespace Multas.Controllers
{
    public class MultasController : Controller
    {
        private MultasDb db = new MultasDb();

        // GET: Multas
        public ActionResult Index()
        {
            var multas = db.Multas.Include(m => m.Agente).Include(m => m.Condutor).Include(m => m.Viatura);
            return View(multas.ToList());
        }

        // GET: Multas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multas multas = db.Multas.Find(id);
            if (multas == null)
            {
                return HttpNotFound();
            }
            return View(multas);
        }

        // GET: Multas/Create
        public ActionResult Create()
        {
            // Criação de um View Model com dados para as dropdowns.
            var model = new MultaFormModel();

            PreencherDropDownsComDadosBd(model);

            return View(model);
        }

        // POST: Multas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MultaFormModel model)
        {
            if (ModelState.IsValid)
            {
                var multa = new Models.Multas
                {
                    Infracao = model.Infracao,
                    LocalDaMulta = model.LocalDaMulta,

                    // Os seguintes campos são nullable. 
                    // Temos que usar '.Value' para obter o valor,
                    // senão dá erro
                    // (Ex: cannot convert int? to int)
                    DataDaMulta = model.DataDaMulta.Value,
                    ValorMulta = model.ValorMulta.Value,

                    AgenteFK = model.AgenteFK.Value,
                    CondutorFK = model.CondutorFK.Value,
                    ViaturaFK = model.ViaturaFK.Value
                };

                db.Multas.Add(multa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Se obtermos um erro, teremos que reinicializar as nossas dropdowns.
            PreencherDropDownsComDadosBd(model);

            return View(model);
        }

        // GET: Multas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multas multas = db.Multas.Find(id);
            if (multas == null)
            {
                return HttpNotFound();
            }

            // Fiz um construtor que recebe uma multa, para "limpar"
            // o código do controller um bocadinho...
            var model = new MultaFormModel(multas);

            PreencherDropDownsComDadosBd(model);

            return View(multas);
        }

        // POST: Multas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MultaFormModel model)
        {
            if (ModelState.IsValid)
            {
                var multas = db.Multas.Find(model.ID);

                // TODO: Preencher campos da multa...
                //       Ver como se faz no controller dos Agentes, método Edit.

                db.Entry(multas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PreencherDropDownsComDadosBd(model);

            return View(model);
        }

        // GET: Multas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Multas multas = db.Multas.Find(id);
            if (multas == null)
            {
                return HttpNotFound();
            }
            return View(multas);
        }

        // POST: Multas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Multas multas = db.Multas.Find(id);
            db.Multas.Remove(multas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Preenche as dropdowns do view model com dados da bd.
        private void PreencherDropDownsComDadosBd(MultaFormModel model)
        {
            // Opção nº 1: Usar um SelectList.
            // Nota: nameof(Agentes.ID) -> "ID"
            //       nameof(Agentes.Nome) -> "Nome"
            // (mais resistente também a refactorings).
            model.AgentesSelectList = new SelectList(db.Agentes, nameof(Agentes.ID), nameof(Agentes.Nome), model.AgenteFK);
                
            // Opção nº 2: Linq. Mais flexível do que a opção nº 1, mas é mais código...
            // eu prefiro esta opção, para ser sincero.
            model.ViaturasSelectList = db.Viaturas
                .Select(viatura => new SelectListItem
                {
                    Value = viatura.ID.ToString(), // SelectListItem só suporta string.
                    Text = viatura.Marca + " " + viatura.Modelo + " de " + viatura.NomeDono + ", matrícula " + viatura.Matricula,
                    // Selected serve para fazer com que a opção seja seleccionada by default.
                    // seleccionaremos se a viatura da BD for a viatura do modelo.
                    Selected = model.ViaturaFK == viatura.ID
                })
                .ToList(); // Convém fazer o ToList()

            model.CondutoresSelectList = db.Condutores
                .Select(condutor => new SelectListItem
                {
                    Value = condutor.ID.ToString(),
                    Text = condutor.Nome + ", carta de condução " + condutor.NumCartaConducao,
                    Selected = model.CondutorFK == condutor.ID
                })
                .ToList();
        }
    }
}
