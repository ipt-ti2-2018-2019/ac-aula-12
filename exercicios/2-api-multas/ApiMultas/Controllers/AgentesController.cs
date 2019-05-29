using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMultas.Data;
using Microsoft.AspNetCore.Mvc;

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

        // [HttpGet, Route("lista")] // /api/agentes/lista
        [HttpGet("lista")] // /api/agentes/lista
        public IActionResult Index()
        {
            var resultado = db.Agentes
                .Select(agente => new
                {
                    agente.ID,
                    agente.Nome,
                    agente.Esquadra,
                    agente.Fotografia,
                    // agente.ListaDeMultas  // cortar o link do agente para as multas.
                })
                .ToList();

            return Ok(resultado);
        }

        [HttpGet("{id}")] // /api/agentes/5
        public ActionResult GetOne(int id)
        {
            var resultado = db.Agentes
                .Where(agente => agente.ID == id)
                .Select(agente => new
                {
                    agente.ID,
                    agente.Nome,
                    agente.Esquadra,
                    agente.Fotografia,
                    ListaDeMultas = agente.ListaDeMultas
                        .Select(multa => new
                        {
                            multa.ID,
                            multa.Infracao,
                            multa.LocalDaMulta,
                            Viatura = new
                            {
                                ID = multa.Viatura.ID,
                                Matricula = multa.Viatura.Matricula
                            },
                            Condutor = new
                            {
                                ID = multa.Condutor.ID,
                                Nome = multa.Condutor.Nome
                            }
                        })
                })
                .FirstOrDefault();

            if (resultado == null)
            {
                return NotFound("Agente com ID " + id + " não existe.");
            }
            else
            {
                return Ok(resultado);
            }
        }

    }
}
  