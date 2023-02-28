using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class EmpresaViewModel
    {

        public int id { get; set; }


        [Required(ErrorMessage = "A Razão Social é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string razacaosocial { get; set; }

        [Required(ErrorMessage = "A Fantasia é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string fantasia { get; set; }

        [Required(ErrorMessage = "O CNPJ é campo obrigatório", AllowEmptyStrings = false)]
        public string cnpj { get; set; }

        [Required(ErrorMessage = "A Logradouro é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string logradouro { get; set; }


        public string numero { get; set; }

        [Required(ErrorMessage = "O CEP campo obrigatório", AllowEmptyStrings = false)]
        public string cep { get; set; }



        public string complemento { get; set; }


        [Required(ErrorMessage = "A Cidade é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string cidade { get; set; }

        //inserir um combobox com os estado

        [Required(ErrorMessage = "O Estado é campo obrigatório", AllowEmptyStrings = false)]
        public string estado { get; set; }



        public string telefone { get; set; }

        //ativo vai ser automatico no momento da gravação do registro

        public sbyte ativo { get; set; }

        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        public string email { get; set; }

       
        public int idSofwareHouse { get; set; }

      
        public sbyte simples_nacional { get; set; }

        public int? id_superlogica { get; set; }


        //inserir um combobox com os estado
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        public string usuarioInicial { get; set; }

        public System.DateTime datacad { get; set; }


        public System.DateTime dataalt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> usuario { get; set; }

        public enum EstadosEnun
        {
            AC,
            AL,
            AP,
            AM,
            BA,
            CE,
            DF,
            ES,
            GO,
            MA,
            MT,
            MS,
            MG,
            PA,
            PB,
            PR,
            PE,
            PI,
            RJ,
            RN,
            RS,
            RO,
            RR,
            SC,
            SP,
            SE,
            TO

        }
    }
}