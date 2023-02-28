using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models.ViewModels
{
    public class UsuarioViewModelCliente
    {


        public int id { get; set; }


        [Display(Name = "Nome")]
        public string nome { get; set; }

       
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Insira uma e-mail válido")]
        public string email { get; set; }


        public string sexo { get; set; }


        public string logradouro { get; set; }


        public string numero { get; set; }

        public string cep { get; set; }


        public string senha { get; set; }


        public sbyte ativo { get; set; }


        public DateTime dataCad { get; set; }


        public DateTime dataAlt { get; set; }


        public int idNivel { get; set; }


        public string telefone { get; set; }


        public string cidade { get; set; }


        public string estado { get; set; }


        public int idEmpresa { get; set; }


        public sbyte primeiro_acesso { get; set; }
    }
}