using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiMultas.Data;
using ApiMultas.Models;
using Microsoft.AspNetCore.Http;
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

        // [HttpGet, Route("")] // /api/agentes
        [HttpGet("")] // /api/agentes
        public IActionResult Index(string pesquisa)
        {
            IQueryable<Agentes> query = db.Agentes;

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                // https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
                pesquisa = pesquisa.ToLower();

                query = query
                    .Where(a => a.Nome.ToLower().Contains(pesquisa) || 
                                a.Esquadra.ToLower().Contains(pesquisa));
            }

            var resultado = query
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


        [HttpPost("")]
        public ActionResult Create([FromForm] CreateAgenteModel model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var pasta = Path.Combine("wwwroot", "FotosAgentes");

            var nomeFicheiro =
                Guid.NewGuid().ToString() + Path.GetExtension(model.Fotografia.FileName);

            using (Stream output = System.IO.File.OpenWrite(Path.Combine(pasta, nomeFicheiro)))
            {
                model.Fotografia.CopyTo(output);
            }

            var novoAgente = new Agentes
            {
                Nome = model.Nome,
                Esquadra = model.Esquadra,
                Fotografia = nomeFicheiro,
                //ContentTypeFotografia = model.Fotografia.ContentType
            };

            db.Agentes.Add(novoAgente);

            db.SaveChanges();

            return Ok(new
            {
                novoAgente.ID,
                novoAgente.Esquadra,
                novoAgente.Fotografia,
                novoAgente.Nome
            });
        }

        [HttpGet("{id}/foto")]
        public ActionResult GetFoto(int id)
        {
            var agente = db.Agentes.Find(id);

            if (agente == null)
            {
                return NotFound("Não é possível obter a foto do agente " + id + " porque este não existe.");
            }

            var caminhoFicheiro = Path.Combine("FotosAgentes", agente.Fotografia);
            
            //return File(caminhoFicheiro, agente.ContentTypeFotografia);
            return File(caminhoFicheiro, "image/jpeg");
        }
    }
}
  