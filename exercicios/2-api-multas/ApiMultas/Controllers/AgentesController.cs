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

        [HttpGet("")]
        public IActionResult Index()
        {
            var resultado = db.Agentes
                .Select(a => new
                {
                    a.ID,
                    a.Nome,
                    a.Esquadra,
                    a.Fotografia,
                    // Não incluo as multas por questões de desempenho
                    // (mais queries à BD)
                })
                .ToList();

            return Ok(resultado);
        }

        //[HttpGet, Route("{id}")]
        [HttpGet("{id}")]
        public ActionResult Detalhes(int id)
        {
            var agente = db.Agentes
                .FirstOrDefault(a => a.ID == id);

            if (agente == null)
            {
                return NotFound("Agente com id " + id + " não existe.");
            }
            else
            {
                var resultado = new
                {
                    agente.ID,
                    agente.Nome,
                    agente.Esquadra,
                    agente.Fotografia,
                    ListaDeMultas = agente.ListaDeMultas
                        .Select(m => new
                        {
                            m.ID,
                            m.Infracao,
                            m.LocalDaMulta,
                            IDViatura = m.Viatura.ID,
                            m.DataDaMulta
                            // ...
                        })
                        .ToList()
                };

                return Ok(resultado);
            }
        }
    }
}
