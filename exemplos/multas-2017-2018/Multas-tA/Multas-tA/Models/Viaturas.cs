using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Multas.Models {
   public class Viaturas {

      public Viaturas() {
         ListaDeMultas = new HashSet<Multas>();
      }

      [Key]
      public int ID { get; set; } // PK

      // dados específicos da viatura
      public string Matricula { get; set; }

      public string Marca { get; set; }

      public string Modelo { get; set; }

      public string Cor { get; set; }

      // dados do dono da viatura
      public string NomeDono { get; set; }

      public string MoradaDono { get; set; }

      public string CodPostalDono { get; set; }

      // complementar a informação sobre o relacionamento
      // de uma Viatura com as Multas a ela relacionadas
      public virtual ICollection<Multas> ListaDeMultas { get; set; }

   }
}