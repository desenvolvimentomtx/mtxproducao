﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class SetorProdutosViewModel
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Required(ErrorMessage = "A Descrição é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        [Display(Name = "Descrição")]
        public String descricao { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tributacao> tributacoes { get; set; }
    }
}