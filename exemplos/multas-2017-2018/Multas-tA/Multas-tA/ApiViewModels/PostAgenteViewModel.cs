using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Multas.ApiViewModels
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
    public class PostAgenteViewModel : IValidatableObject
    {
        [Required]
        [RegularExpression("[A-ZÂÍ][a-záéíóúãõàèìòùâêîôûäëïöüç.]+(( | de | da | dos | d'|-)[A-ZÂÍ][a-záéíóúãõàèìòùâêîôûäëïöüç.]+){1,3}",
            ErrorMessage = "O nome apenas aceita letras. Cada palavra começa por uma maiúscula, seguida de minúsculas...")]
        [StringLength(40)]
        public string Nome { get; set; }

        [Required]
        public string Esquadra { get; set; }

        /// <summary>
        /// Validação custom.
        /// 
        /// Aqui estou a fazer uso de um exemplo 'absurdo',
        /// em que a <see cref="Esquadra"/> não pode ter o valor de 'IPT',
        /// e que o <see cref="Nome"/> não pode ser 'Zé Carlos'.
        /// 
        /// Atenção: Este método só é chamado pelo MVC/Web API quando as validações
        /// [Required], [StringLength], [RegularExpression], etc.
        /// terminarem todas com sucesso. Se alguma falhar, este método não é chamado.
        /// 
        /// Este método é invocado depois dos campos da classe terem os seus valores preenchidos.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Quando se quer fazer uma comparação de strings, sem ter em conta
            // maiúsculas ou minúsculas, temos que fazer uso do método 'Equals'.
            // Caso contrário, o '==' serve.
            if (Esquadra.Equals("IPT", StringComparison.OrdinalIgnoreCase))
            {
                // Adição de um erro 'custom' ao campo 'Esquadra'.
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
                    "O IPT não é uma esquadra, é uma instituição de ensino...", 
                    new[] { nameof(Esquadra) } // O 'nameof(Variavel)' dá o nome da variável, como string.
                );

            }

            // Ao contrário de um 'return', o 'yield return' não sai da função.
            // A função pode continuar a sua execução, o que significa
            // que, se colocasse o nome da esquadra como 'IPT', ele daria o erro,
            // continuava, e se o nome fosse 'Zé Carlos', também daria esse erro.
            
            if (Nome == "Zé Carlos")
            {
                yield return new ValidationResult(
                    "O Zé Carlos só marca presença regular na antena da SIC.",
                    new[] { nameof(Nome) }
                );
            }
        }
    }
}