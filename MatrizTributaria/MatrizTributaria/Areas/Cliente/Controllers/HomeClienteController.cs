using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class HomeClienteController : Controller
    {
        // GET: Cliente/HomeCliente
        public ActionResult HomeCliente()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login","../Home");
            }
            return View();
           
        }

        public ActionResult LogOut()
        {
            if (Session["usuario"] != null)
            {
                Session["usuario"]         = null;
                Session["empresa"]         = null;
                Session["email"]           = null;
                TempData["usuarioEmpresa"] = null;
                TempData["analise"] = null;
                Session["usuarios"] = null;
                Session["empresas"] = null;
                TempData["analise2"] = null;
                TempData["prdInexistente"] = null;
                TempData["analiseSN"]      = null;
                TempData["UfOrigem"]       = null;
                TempData["UfDestino"]      = null;
                TempData["tributacao"]     = null;
                TempData["analise_NCM"] = null;
                TempData["linhas"] = null;
                return RedirectToAction("HomeCliente");
            }
            else
            {
                return RedirectToAction("HomeCliente");
            }


        }
        
    }
}