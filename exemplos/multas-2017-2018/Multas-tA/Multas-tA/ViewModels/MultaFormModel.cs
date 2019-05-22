using Multas.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Multas.ViewModels
{
    /// <summary>
    /// Um exemplo de um View Model mais útil,
    /// em que colocamos dados para dropdowns no modelo
    /// em vez de usarmos a ViewBag.
    /// </summary>
    public class MultaFormModel
    {
        public MultaFormModel()
        {

        }

        public MultaFormModel(Models.Multas multas)
        {
            this.ID = multas.ID;

            this.AgenteFK = multas.AgenteFK;
            this.CondutorFK = multas.CondutorFK;
            this.ViaturaFK = multas.ViaturaFK;

            this.DataDaMulta = multas.DataDaMulta;
            this.Infracao = multas.Infracao;
            this.LocalDaMulta = multas.LocalDaMulta;
            this.ValorMulta = multas.ValorMulta;
        }
        
        public int? ID { get; set; }

        public string Infracao { get; set; }

        public string LocalDaMulta { get; set; }

        // Notar o '?'. Isto permite que o campo receba valores nulos.
        // O '[Required]' só verifica se o campo não é null, e em caso de strings,
        // se a string não está vazia.
        // No entanto, 'int', 'decimal', 'double', etc., têm o problema
        // de não serem nulos, e serem inicializados a 0 em .net quando não têm valor.
        // Isto faria com que o campo aparecesse com um 0, e nunca falharia a validação
        // do obrigatório.
        // No entanto, alguns valores por defeito induzem pessoas em erro (especialmente aquelas
        // que preenchem o formulário à pressa, e depois ficam valores de 0, que até podem
        // ser válidos, mas não é o que as pessoas queriam...
        [Required]
        [Range(0d, double.MaxValue)] // Usamos 'double' porque não existe [Range] para decimal...
        public decimal? ValorMulta { get; set; }

        // Devemos usar [DataType(DataType.Date)] quando queremos datas,
        // [DataType(DataType.DateTime)] quando queremos data e hora.
        // Como não existe suporte para data e hora em todos os browsers,
        // (ver https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/datetime-local)
        // vamos usar Date (só data, sem hora). Para DateTime, usaria um date picker custom (ex: jQuery UI).
        // Existem outros valores úteis que se podem usar no "2º" DataType, como 'EmailAddress'.
        // Mais uma vez, isto pode ser usado nos EditorTemplates (usar o nome, como 'EmailAddress.cshtml').
        [Required, DataType(DataType.Date)]
        public DateTime? DataDaMulta { get; set; }

        public int? AgenteFK { get; set; }

        public int? ViaturaFK { get; set; }

        public int? CondutorFK { get; set; }

        #region Dados para dropdowns.

        public IEnumerable<SelectListItem> AgentesSelectList { get; set; }

        public IEnumerable<SelectListItem> ViaturasSelectList { get; set; }

        public IEnumerable<SelectListItem> CondutoresSelectList { get; set; }

        #endregion
    }
}