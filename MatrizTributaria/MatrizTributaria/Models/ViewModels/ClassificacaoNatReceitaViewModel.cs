using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class ClassificacaoNatReceitaViewModel
    {
        [Required(ErrorMessage = "O Código é campo obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "A Descrição é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        [Display(Name = "Descrição")]
        public String Descricao { get; set; }

    }
}