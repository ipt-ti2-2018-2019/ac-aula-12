using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMultas.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMultas.Controllers
{
    // Serve como prefixo para QUALQUER action result
    // deste controller que tem [Route] (a não ser que o route
    // do ActionResult comece por ~
    [Route("api/agentes")]
    public class AgentesController : Controller
    {
        private readonly ApplicationDbContext db;

        public AgentesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET /api/agentes
        // [Route(""), HttpGet] // Alternativa 1
        [HttpGet("")] // Alternativa 2
        public IActionResult Index()
        {
            var resultado = db.Agentes
                //.Include(a => a.ListaDeMultas)
                .Select(a => new
                {
                    a.ID,
                    a.Esquadra,
                    a.Fotografia,
                    a.Nome
                    // Não se incluem as multas por questões de
                    // referências circulares.
                })
                .ToList();

            return Ok(resultado);
        }

        // GET /api/agentes/{id}
        // Obter um único agente, pelo seu ID.
        [HttpGet("{id}")]
        public IActionResult AgentePorId(int id)
        {
            var agente = db.Agentes
                .Select(a => new
                {
                    a.ID,
                    a.Nome,
                    a.Esquadra,
                    a.Fotografia,
                    ListaDeMultas = a.ListaDeMultas
                        .Select(multa => new
                        {
                            multa.ID,
                            multa.DataDaMulta,
                            multa.ValorMulta,
                            Condutor = new
                            {
                                multa.Condutor.ID,
                                multa.Condutor.Nome
                            },
                            Viatura = new
                            {
                                multa.Viatura.ID,
                                multa.Viatura.Matricula
                            }
                        })
                        .ToList()
                })
                .FirstOrDefault(a => a.ID == id);

            if (agente == null)
            {
                return NotFound("O agente com ID " + id + " não existe.");
            }
            
            return Ok(agente);
        }
    }
}
