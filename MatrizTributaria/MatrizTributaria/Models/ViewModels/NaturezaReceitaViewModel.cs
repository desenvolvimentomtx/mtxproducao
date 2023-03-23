using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class NaturezaReceitaViewModel
    {


        [Required(ErrorMessage = "O Código é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Código")]
        public int id { get; set; }


        [Required(ErrorMessage = "A descrição é obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Descrição")]
        public String descricao { get; set; }


        [Required(ErrorMessage = "A classificação é campo obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Classificação")]
        public int idClassificacao { get; set; }


    }
}