using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultas.Models.Agentes
{
    public class UpdateAgenteModel
    {
        [Required]
        public string Nome { get; set; }
    }
}
