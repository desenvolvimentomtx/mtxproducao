using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class CstIcmsGeralViewModel
    {

        [Required(ErrorMessage = "O Código é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Código")]
        public int codigo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Descrição")]
        public String descricao { get; set; }


        public DateTime? dataCad { get; set; }


        public DateTime? dataAlt { get; set; }


    }
}