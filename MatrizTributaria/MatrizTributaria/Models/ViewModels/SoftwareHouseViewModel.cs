using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models.ViewModels
{
    public class SoftwareHouseViewModel
    {

        
        public int id { get; set; }


        [Required(ErrorMessage = "O Nome é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A razão social é campo obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "O CNPJ é campo obrigatório", AllowEmptyStrings = false)]
        public string Cnpj { get; set; }

        
        public string Logradouro { get; set; }

        
        public string Numero { get; set; }

       
        public string CEP { get; set; }

       
        public string Complemento { get; set; }

       
        public string Cidade { get; set; }

        //inserir um combobox com os estado
       
        public string Estado { get; set; }


      
        public string Telefone { get; set; }

       
        public sbyte Ativo { get; set; }
        [Required(ErrorMessage = "O EMAIL é campo obrigatório", AllowEmptyStrings = false)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        public string Email { get; set; }


       
        public string Chave { get; set; }

        /*As datas serão informadas automaticamente no momento da criação do registro
         e quando houver alteração*/
      
        public System.DateTime DataCad { get; set; }

       
        public System.DateTime DataAlt { get; set; }

       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Empresa> Empresa { get; set; }
    }
}