using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("classificacao_nat_receita")]
    public class ClassificacaoNatReceita
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Codigo")]
        public int Codigo { get; set; }

        [Column("Descricao")]
        public String Descricao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NaturezaReceita> naturezaReceita { get; set; }
    }
}