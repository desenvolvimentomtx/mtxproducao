using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models
{
    [Table("token")]
    public class Token
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }


        [Column("token")]
        public string Tokens { get; set; }

        [Column("vencimento")]
        public DateTime? Vencimento { get; set; }

        [Column("dataCad")]
        public DateTime? DataCad { get; set; }

        [Column("dataBloq")]
        public DateTime? DataBloq { get; set; }

        //ativo vai ser automatico no momento da gravação do registro
        [Column("status")]
        public sbyte Ativo { get; set; }

        [Required(ErrorMessage = "Selecione uma Software House", AllowEmptyStrings = false)]
        [Column("id_softwarehouse")]
        [ForeignKey("SoftwareHouse")]
        public int idSofwareHouse { get; set; }

        [JsonIgnore]
        public virtual SoftwareHouse SoftwareHouse { get; set; }


    }
}