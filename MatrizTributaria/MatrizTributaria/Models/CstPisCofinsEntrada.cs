using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace MatrizTributaria.Models
{

    [Table("cst_pis_cofins_e")]
    public class CstPisCofinsEntrada
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Codigo")]
        public int codigo { get; set; }

        [Display(Name = "Descrição")]
        [Column("Descricao")]
        public String descricao { get; set; }

        [Column("DataCad")]
        public DateTime? dataCad { get; set; }

        [Column("DataAlt")]
        public DateTime? dataAlt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tributacao> tributacoes { get; set; }
    }
}