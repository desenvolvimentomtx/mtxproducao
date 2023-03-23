using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class ProdutoViewModel
    {

        public int Id { get; set; }


        public Int64 codInterno { get; set; }


        public Int64 codBarras { get; set; }

        [Required(ErrorMessage = "Descrição é campo obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Descrição")]
        public string descricao { get; set; }


        public string cest { get; set; }


        public string ncm { get; set; }


        public DateTime? dataCad { get; set; }


        public DateTime? dataAlt { get; set; }

        [Required(ErrorMessage = "Categoria é campo obrigatório", AllowEmptyStrings = false)]
        [Display(Name = "Categoria")]
        public int idCategoria { get; set; }


        public Nullable<sbyte> status { get; set; }
    }
}