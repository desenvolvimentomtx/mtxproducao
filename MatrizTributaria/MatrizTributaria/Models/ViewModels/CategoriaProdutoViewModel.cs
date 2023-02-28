using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class CategoriaProdutoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A Descrição é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        [Display(Name = "Descricao da Categoria")]
        public String CategoriaDescricao { get; set; }
    }
}