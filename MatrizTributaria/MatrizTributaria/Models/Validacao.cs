using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("VALIDACAO")]
    public class Validacao
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        
        [Column("EMAIL")]
        public string Email { get; set; }


        [Column("SENHA")]
        public string Senha { get; set; }
    }
}