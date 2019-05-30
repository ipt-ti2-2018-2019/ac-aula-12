using ApiMultas.Data;
using ApiMultas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public IActionResult Index([FromQuery]string pesquisa, decimal? minMultas)
        {
            IQueryable<Agentes> query = db.Agentes;

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.ToLower();

                query = query
                    .Where(a =>
                        a.Nome.ToLower().Contains(pesquisa) ||
                        a.Esquadra.ToLower().Contains(pesquisa)
                    );
            }

            if (minMultas != null)
            {
                query = query
                    .Where(a => a.ListaDeMultas
                        .Sum(m => m.ValorMulta) > minMultas.Value);
            }

            var resultado = query
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

        [HttpGet("{id}/foto")] // GET /api/agentes/{id}/foto
        public ActionResult GetFoto(int id)
        {
            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound("Não é possível obter a foto do agente " + id + " porque este não existe.");
            }

            var caminhoFoto = Path.Combine("FotosAgentes", agente.Fotografia);

            return File(caminhoFoto, "image/jpeg");
        }

        [HttpPost("")] // POST /api/agentes
        public ActionResult Create([FromForm]CreateAgenteModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var novoAgente = new Agentes
            {
                Nome = model.Nome,
                Esquadra = model.Esquadra
            };

            var pastaFotos = Path.Combine("wwwroot", "FotosAgentes");

            var nomeFicheiro =
                Guid.NewGuid().ToString() + Path.GetExtension(model.Fotografia.FileName);

            using (Stream output = System.IO.File.OpenWrite(Path.Combine(pastaFotos, nomeFicheiro)))
            {
                model.Fotografia.CopyTo(output);
            }

            novoAgente.Fotografia = nomeFicheiro;

            db.Agentes.Add(novoAgente);

            db.SaveChanges();

            return Ok(new
            {
                novoAgente.ID,
                novoAgente.Nome,
                novoAgente.Esquadra,
                novoAgente.Fotografia
            });
        }
    }
}
