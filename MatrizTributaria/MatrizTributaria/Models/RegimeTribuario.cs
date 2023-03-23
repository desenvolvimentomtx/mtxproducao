using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("regime_trib")]
    public class RegimeTribuario
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int ID { get; set; }


        //[Column("Estado")]
        //public string estado { get; set; }

        [Column("Tipo")]
        public string TIPO { get; set; }


        [Column("DataCad")]
        public DateTime DataCad { get; set; }

        [Column("DataAlt")]
        public DateTime DataAlt { get; set; }

    }
}