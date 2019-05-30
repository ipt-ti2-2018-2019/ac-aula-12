using ApiMultas.Data;
using ApiMultas.Models;
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

        [HttpGet("")]
        public IActionResult Index(string pesquisa, decimal? minMultas)
        {
            IQueryable<Agentes> query = db.Agentes;

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.ToLower();
                query = query.Where(a => a.Nome.ToLower().Contains(pesquisa) || a.Esquadra.ToLower().Contains(pesquisa));
            }

            if (minMultas != null)
            {
                query = query
                    .Where(a => a.ListaDeMultas
                        .Sum(m => m.ValorMulta) > minMultas.Value);
            }

            var resultado = query
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

        [HttpPost("")] // POST /api/agentes
        public ActionResult Create([FromForm]CreateAgenteModel model)
        {
            if (ModelState.IsValid == false)
            {
                // Exceção - erro de validação (400 Bad Request)
                return BadRequest(ModelState);
            }

            var novoAgente = new Agentes
            {
                Nome = model.Nome,
                Esquadra = model.Esquadra,

            };

            var pastaFotos = Path.Combine("wwwroot", "FotosAgentes");

            var nomeFicheiro = Guid.NewGuid().ToString() + Path.GetExtension(model.Fotografia.FileName);

            using (Stream output = System.IO.File.OpenWrite(Path.Combine(pastaFotos, nomeFicheiro)))
            {
                model.Fotografia.CopyTo(output);
            }

            novoAgente.Fotografia = nomeFicheiro;

            db.Agentes.Add(novoAgente);

            db.SaveChanges();

            return Ok(new { novoAgente.ID, novoAgente.Nome, novoAgente.Esquadra, novoAgente.Fotografia });
        }
        
        [HttpGet("{id}/foto")] // GET /api/agentes/{id}/foto
        public ActionResult GetFoto(int id)
        {
            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound("Não é possível obter a foto do agente com ID " + id + " porque este não existe.");
            }

            var caminhoFoto = Path.Combine("FotosAgentes", agente.Fotografia);

            return File(caminhoFoto, "image/jpeg");
        }


    }
}
