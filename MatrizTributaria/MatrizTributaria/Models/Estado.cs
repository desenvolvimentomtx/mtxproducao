using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models
{
    [Table("estado")]
    public class Estado
    {
        [Key]
        public int id { get; set; }

        [Column("estado")]
        public string descricao { get; set; }

        [Column("uf")]
        public string uf { get; set; }
    }
}