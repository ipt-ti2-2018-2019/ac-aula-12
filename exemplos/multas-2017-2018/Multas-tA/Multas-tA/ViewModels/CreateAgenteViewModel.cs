using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Multas.ViewModels
{
    /// <summary>
    /// Classe auxiliar de criação de Agente.
    /// 
    /// Ajuda a prevenir contra ataques de "Overposting" / "Mass assignment", e também
    /// permite formulários com campos diferentes da BD.
    /// 
    /// Por exemplo, quando criamos um agente, não vamos especificar as suas multas,
    /// ou o seu ID, mas se usarmos a classe 'Agentes', estamos a 'deixar a porta aberta'
    /// a que se especifiquem esses campos no pedido, e o Model Binder do MVC / Web API lê-os.
    /// Nota que este problema é comum a todas as frameworks que fazem uso de mecanismos de model
    /// binding, seja ASP.NET, Spring, etc.
    /// 
    /// Alternativamente, podia fazer uso do [Bind] (como no <see cref="Controllers.AgentesController.Create(Models.Agentes, HttpPostedFileBase)"/>),
    /// mas prefiro usar view models quando possível.
    /// 
    /// O uso de view models, tanto em Web API, como em MVC, também permite colocar
    /// informação sobre validações, sem termos que editar as classes da BD
    /// (por vezes, não podemos fazê-lo).
    /// 
    /// Quando se quer implementar validações custom, faz-se uso da interface
    /// <see cref="IValidatableObject"/>
    /// 
    /// Ver http://www.abhijainsblog.com/2015/04/over-posting-attack-in-mvc.html
    /// </summary>
    public class CreateAgenteViewModel : IValidatableObject
    {
        [Required]
        [RegularExpression("[A-ZÂÍ][a-záéíóúãõàèìòùâêîôûäëïöüç.]+(( | de | da | dos | d'|-)[A-ZÂÍ][a-záéíóúãõàèìòùâêîôûäëïöüç.]+){1,3}",
            ErrorMessage = "O nome apenas aceita letras. Cada palavra começa por uma maiúscula, seguida de minúsculas...")]
        [StringLength(40)]
        public string Nome { get; set; }

        [Required]
        public string Esquadra { get; set; }

        /// <summary>
        /// Campos que são HttpPostedFileBase também podem ser colocados em View Models,
        /// e também podem ser validados.
        /// 
        /// Nota: O editor default não funciona... temos que criar um custom.
        /// Ver: Views/Shared/EditorTemplates/HttpPostedFileBase.cshtml
        /// </summary>
        [Required]
        public HttpPostedFileBase Fotografia { get; set; }

        /// <summary>
        /// Validação custom.
        /// 
        /// Aqui estou a fazer validação do tipo da imagem do agente.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Usar o Content-Type para determinar o tipo do ficheiro.
            // Imagens são geralmente image/png, image/jpeg, etc.
            if (Fotografia.ContentType.ToLower().Split('/').First() != "image")
            {
                // Adição de um erro 'custom' ao campo 'Fotografia'.
                // A classe 'ValidationResult' permite definir não só a mensagem,
                // como também os nomes dos campos (num array).
                // Só defino os nomes se necessário. Se não for preciso, 
                // omito o array.

                // Uma nota sobre o 'yield return' no C#:
                // O 'yield return' pode ser usado para fazer com que a função
                // devolva um conjunto de valores.
                // Nota: só pode ser usado com um tipo de retorno 'IEnumerable<T>'.
                // o valor no 'yield return' tem que ser um 'T'.
                yield return new ValidationResult(
                    "Só são aceites imagens.",
                    new[] { nameof(Fotografia) } // O 'nameof(Variavel)' dá o nome da variável, como string.
                );
            }
        }
    }
}