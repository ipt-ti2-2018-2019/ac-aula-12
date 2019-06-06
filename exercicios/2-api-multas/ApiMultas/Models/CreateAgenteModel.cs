using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultas.Models
{
    public class CreateAgenteModel : IValidatableObject
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Esquadra { get; set; }

        [Required]
        public IFormFile Fotografia { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Esquadra == "IPT")
            {
                yield return new ValidationResult("O IPT não é uma esquadra!!", new[] { "Esquadra" });
            }

            if (Fotografia.ContentType != "image/jpeg")
            {
                yield return new ValidationResult("Só se aceita JPEGs.", new[] { "Fotografia" });
            }
        }
    }
}
