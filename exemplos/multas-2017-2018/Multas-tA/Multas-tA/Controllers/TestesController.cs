using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Multas.Controllers
{
    public class TestesController : Controller
    {
        // GET: Testes/Agentes
        public ActionResult Agentes()
        {
            return View();
        }
    }
}