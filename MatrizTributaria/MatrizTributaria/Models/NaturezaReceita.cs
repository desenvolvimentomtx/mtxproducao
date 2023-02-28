using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("natureza_receita")]
    public class NaturezaReceita
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Codigo")]
        public int id { get; set; }

        [Column("Descricao")]
        public String descricao { get; set; }

        [ForeignKey("classificacaoNatRec")]
        [Column("Id_Class_Nat_Receita")]
        public int idClassificacao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ClassificacaoNatReceita classificacaoNatRec { get; set; }//relacionamento com a categoria


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tributacao> tributacao { get; set; }
    }
}