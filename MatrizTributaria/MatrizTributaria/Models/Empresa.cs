using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{

    [Table("empresa")]
    public class Empresa
    {
        [Key]
        public int id { get; set; }


        [Column("razacaosocial")]
        public string razacaosocial { get; set; }

        [Column("fantasia")]
        public string fantasia { get; set; }

        [Column("cnpj")]
        public string cnpj { get; set; }

        [Column("logradouro")]
        public string logradouro { get; set; }

        [Column("numero")]
        public string numero { get; set; }

        [Column("cep")]
        public string cep { get; set; }

        [Column("complemento")]
        public string complemento { get; set; }

        [Column("cidade")]
        public string cidade { get; set; }

        //inserir um combobox com os estado
        [Column("estado")]
        public string estado { get; set; }


        [Column("telefone")]
        public string telefone { get; set; }

        //ativo vai ser automatico no momento da gravação do registro
        [Column("ativo")]
        public sbyte ativo { get; set; }

        //ativo vai ser automatico no momento da gravação do registro
        [Column("simples_nacional")]
        public sbyte simples_nacional { get; set; }

        [Column("email")]
        public string email { get; set; }

        [Required(ErrorMessage = "Selecione uma Software House", AllowEmptyStrings = false)]
        [Column("id_softwarehouse")]
        [ForeignKey("SoftwareHouse")]
        public int idSofwareHouse { get; set; }

        //vitor: 08112022
        [Column("id_superlogica")]
        public int? id_superlogica { get; set; }


        //inserir um combobox com os estado
        [Column("usuario_admin_inicial")]
        public string usuarioInicial { get; set; }

        /*As datas serão informadas automaticamente no momento da criação do registro
         e quando houver alteração*/
        [Column("datacad")]
        public System.DateTime datacad { get; set; }

        [Column("dataalt")]
        public System.DateTime dataalt { get; set; }


        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> usuario { get; set; }

        [JsonIgnore]
        public virtual SoftwareHouse SoftwareHouse { get; set; }

    }
}