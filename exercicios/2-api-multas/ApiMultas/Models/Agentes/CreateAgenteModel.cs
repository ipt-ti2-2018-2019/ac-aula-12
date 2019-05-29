using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMultas.Models.Agentes
{
    /// <summary>
    /// Classe usada para criar um Agente.
    /// </summary>
    public class CreateAgenteModel : IValidatableObject
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Esquadra { get; set; }

        [Required]
        public IFormFile Fotografia { get; set; }

        /// <summary>
        /// Este método é chamado pelo MVC/Web API 
        /// DEPOIS das validações das anotações ([Required], [RegularExpression], ...)
        /// Para permitir validações mais complicadas.
        /// Aqui, uso o "yield return" do C# para disponibilizar vários erros de uma só vez.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validar a fotografia (só "permito" JPGs)
            if (Fotografia.ContentType != "image/jpeg" && 
                Fotografia.ContentType != "image/jpg")
            {
                // Disponibilizar um erro se a fotografia não for uma imagem.
                // O segundo argumento é um array a indicar o(s) campos que estão inválidos.
                yield return new ValidationResult("A fotografia tem que ser um JPG.", new[] { "Fotografia" });
            }

            // Validar (também) a esquadra
            if (Esquadra == "IPT")
            {
                yield return new ValidationResult("O IPT não é uma esquadra.", new[] { "Esquadra" });
            }
        }
    }
}
