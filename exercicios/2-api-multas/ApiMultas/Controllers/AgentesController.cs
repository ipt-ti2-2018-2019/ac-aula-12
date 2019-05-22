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

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(db.Agentes.ToList());
        }
    }
}
