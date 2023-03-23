using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MatrizTributaria.Areas.Cliente.Models
{
    [DataContract]
    public class DataPoint
    {
        
        public DataPoint(string rotulo, double valor)
        {
            this.Rotulo = rotulo;
            this.Valor = valor;
        }

       

        [DataMember(Name = "rotulo")]
        public string Rotulo = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "valor")]
        public Nullable<double> Valor = null;
    }
}