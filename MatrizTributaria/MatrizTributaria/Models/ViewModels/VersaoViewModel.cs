using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models.ViewModels
{
    public class VersaoViewModel
    {
       
        public int id { get; set; }

        [Required(ErrorMessage = "A Nota é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        [Display(Name = "Nota da versão")]
        public string nota { get; set; }

        [Required(ErrorMessage = "A Versão é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        [Display(Name = "Descricao da Versão")]
        public string versao { get; set; }
    }
}