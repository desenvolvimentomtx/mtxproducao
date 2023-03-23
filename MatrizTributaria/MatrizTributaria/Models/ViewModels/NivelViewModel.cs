using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class NivelViewModel
    {

        public int id { get; set; }


        [Required(ErrorMessage = "A descrição do NÍVEL é obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 10 caracteres")]
        public string descricao { get; set; }

        public DateTime DataCad { get; set; }


        public DateTime DataAlt { get; set; }


        public sbyte Ativo { get; set; }

        public virtual ICollection<Usuario> usuario { get; set; }
    }
}
