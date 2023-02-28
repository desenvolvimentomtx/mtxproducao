using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int id { get; set; }

        [Required(ErrorMessage = "O nome do Usuário é obrigatório", AllowEmptyStrings = false)]
        [Column("nome")]
        public string nome { get; set; }

        [Index(IsUnique = true)]
        [Required(ErrorMessage = "O EMAIL é obrigatório", AllowEmptyStrings = false)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        [Column("email")]
        public string email { get; set; }

        //[Required(ErrorMessage = "O campo Sexo é obrigatório", AllowEmptyStrings = false)]
        [Required(ErrorMessage = "O campo Sexo é obrigatório", AllowEmptyStrings = false)]
        [Column("sexo")]
        public string sexo { get; set; }

        //[Required(ErrorMessage = "O campo logradouro é obrigatório", AllowEmptyStrings = false)]
        [Column("logradouro")]
        public string logradouro { get; set; }

        //[Required(ErrorMessage = "O campo número é obrigatório, caso não exista digiar: 0", AllowEmptyStrings = false)]
        [Column("numero")]
        public string numero { get; set; }

        //[Required(ErrorMessage = "O campo CEP é obrigatório", AllowEmptyStrings = false)]
        [Column("cep")]
        public string cep { get; set; }

        [Required(ErrorMessage = "Campo senha é obrigatório")]
        [Column("senha")]
        public string senha { get; set; }

        [Column("ativo")]
        public sbyte ativo { get; set; }

        [Column("datacad")]
        public DateTime dataCad { get; set; }

        [Column("dataalt")]
        public DateTime dataAlt { get; set; }

        [Required(ErrorMessage = "Selecione um nível", AllowEmptyStrings = false)]
        [Column("idnivel")]
        [ForeignKey("nivel")]
        public int idNivel { get; set; }

        [Column("telefone")]
        public string telefone { get; set; }

        //[Required(ErrorMessage = "Campo Cidade é obrigatório")]
        [Column("cidade")]
        public string cidade { get; set; }

        //[Required(ErrorMessage = "Campo Estado é obrigatório")]
        [Column("estado")]
        public string estado { get; set; }

        [Required(ErrorMessage = "Campo Empresa é obrigatório")]
        [ForeignKey("empresa")]
        [Column("idempresa")]
        public int idEmpresa { get; set; }

        [Required(ErrorMessage = "Informar esse campo para alteração de senha")]
        [Column("primeiro_acesso")]
        public sbyte primeiro_acesso { get; set; }

        [Required(ErrorMessage = "Informar esse campo para acesso a outras empresas")]
        [Column("acesso_empresas")]
        public sbyte acesso_empresas { get; set; }


        [JsonIgnore]
        public virtual Empresa empresa { get; set; }

        [JsonIgnore]
        public virtual Nivel nivel { get; set; }



    }
}