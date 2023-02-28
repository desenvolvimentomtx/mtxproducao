using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MatrizTributaria.Models
{
    [Table("cst_sac_sas_svc_snc_ei_ed_es_nfi_nfd")]
    public class CstIcmsGeral
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



    }
}