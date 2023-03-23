using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("crt")]
    public class Crt
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int ID { get; set; }

        [Column("Descricao")]
        public string DESCRICAO { get; set; }

        [Column("Obs")]
        public string OBSERVACAO { get; set; }


        [Column("DataCad")]
        public DateTime DATACAD { get; set; }

        [Column("DataAlt")]
        public DateTime DATAALT { get; set; }
 
       
    }
}