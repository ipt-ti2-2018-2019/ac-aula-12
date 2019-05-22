using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Multas.ViewModels
{
    /// <summary>
    /// Variação do <see cref="CreateAgenteViewModel"/> que não tem
    /// a fotografia como obrigatória, e por questões de exemplificação de um
    /// ataque de Overposting, também remove a possibilidade de editar o Nome.
    /// 
    /// É um bocado duplicar código, mas técnicas como herança poderiam ajudar
    /// (mas pouco).
    /// </summary>
    public class EditAgenteViewModel : IValidatableObject
    {
        [Required]
        public int ID { get; set; }
        
        [Required]
        public string Esquadra { get; set; }

        public HttpPostedFileBase Fotografia { get; set; }

        /// <summary>
        /// Campo auxiliar para a view que indica a foto atual.
        /// </summary>
        public string FotografiaAtual { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Cuidado: Se o [Required] não está presente,
            // a Fotografia pode ser null...
            if (
                Fotografia != null &&
                Fotografia.ContentLength > 0 &&
                Fotografia.ContentType.Split('/').First() != "image"
            )
            {
                yield return new ValidationResult(
                    "Só são aceites imagens.",
                    new[] { nameof(Fotografia) }
                );
            }
        }
    }
}