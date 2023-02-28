using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("setor_produtos")]
    public class SetorProdutos
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Display(Name = "Descrição")]
        [Column("SetorDescricao")]
        public String descricao { get; set; }


        public virtual ICollection<Tributacao> tributacoes { get; set; }
    }
}