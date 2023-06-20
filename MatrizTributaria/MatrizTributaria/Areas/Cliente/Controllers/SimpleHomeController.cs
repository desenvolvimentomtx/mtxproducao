using MatrizTributaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class SimpleHomeController : Controller
    {
        readonly private MatrizDbContext db = new MatrizDbContext();
        Empresa empresa;
        // GET: Cliente/SimpleHome
        public ActionResult Index(string cnpj)
        {
            //usar esse link
            //https://localhost:44324/Cliente/SimpleHome/Index?cnpj=15202475000219

            //formatando o cnpj
            string cnpjRecebido = FormatCNPJ(cnpj); 
    
            //pegando a empresa
            this.empresa = (from a in db.Empresas where a.cnpj == cnpjRecebido select a).FirstOrDefault(); 


            //passando o nome fantasia para a view
            ViewBag.Empresa = this.empresa.fantasia;

            //pegando o estado da empresa
            ViewBag.UfOrigem = this.empresa.estado;
            ViewBag.UfDestino = this.empresa.estado;





            return View();
        }


        public static string FormatCNPJ(string CNPJ)
        {
            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}