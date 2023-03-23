using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models.ViewModels
{
    public class TokenViewModel
    {
        
        public int Id { get; set; }


        public string Tokens { get; set; }

       
        public DateTime? Vencimento { get; set; }

       
        public DateTime? DataCad { get; set; }

       
        public DateTime? DataBloq { get; set; }

       
        public sbyte Ativo { get; set; }

        
        public int idSofwareHouse { get; set; }

       
    }
}