using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("nivel")]
    public class Nivel

    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Column("descricao")]
        public string descricao { get; set; }

        [Column("datacad")]
        public DateTime DataCad { get; set; }

        [Column("dataalt")]
        public DateTime DataAlt { get; set; }

        [Column("ativo")]
        public sbyte Ativo { get; set; }

        //public virtual ICollection<Usuario> usuario { get; set; }
        [JsonIgnore]
        public virtual List<Usuario> usuario { get; set; }

        

       


    }
}