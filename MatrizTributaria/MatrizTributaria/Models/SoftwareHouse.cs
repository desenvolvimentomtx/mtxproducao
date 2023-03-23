using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("softwarehouse")]
    public class SoftwareHouse
    {
        [Key]
        public int Id { get; set; }


        [Column("nome")]
        public string Nome { get; set; }

        [Column("razao_social")]
        public string RazaoSocial { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("logradouro")]
        public string Logradouro { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("cep")]
        public string CEP { get; set; }

        [Column("complemento")]
        public string Complemento { get; set; }

        [Column("cidade")]
        public string Cidade { get; set; }

        //inserir um combobox com os estado
        [Column("estado")]
        public string Estado { get; set; }


        [Column("telefone")]
        public string Telefone { get; set; }

        //ativo vai ser automatico no momento da gravação do registro
        [Column("ativo")]
        public sbyte Ativo { get; set; }

        [Column("email")]
        public string Email { get; set; }


        [Column("chave")]
        public string Chave { get; set; }

        /*As datas serão informadas automaticamente no momento da criação do registro
         e quando houver alteração*/
        [Column("datacad")]
        public System.DateTime? DataCad { get; set; }

        [Column("dataalt")]
        public System.DateTime? DataAlt { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Empresa> Empresa { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Token> Token { get; set; }
    }
}