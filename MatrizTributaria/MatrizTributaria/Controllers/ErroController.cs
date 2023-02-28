using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class ErroController : Controller
    {
        // GET: Erro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Erro(int? param)
        {
            if (param == 1)
            {
                ViewBag.Mensagem = "Usuário " + Session["usuario"] + " não autorizado a INSERIR novos registros";
            }
            if (param == 2)
            {
                ViewBag.Mensagem = "Usuário " + Session["usuario"] + " não autorizado a ALTERAR registros";
            }
            if (param == 3)
            {
                ViewBag.Mensagem = "Usuário " + Session["usuario"] + " não autorizado a EXCLUIR registros";
            }

            if (param == 4)
            {
                ViewBag.Mensagem = "Registro já inserido no sistema. Tente novamente";
            }
            if(param == 5)
            {
                ViewBag.Mensagem = "E-mail já cadastrado para outro usuário";
            }
            if (param == 6)
            {
                ViewBag.Mensagem = "Cnpj já cadastrado para outra empresa";
            }
            if(param == 7)
            {
                ViewBag.Mensagem = "Erro ao acessar o banco de dados, verifique sua conexão e tente novamente";
            }
            if (param == 8)
            {
                ViewBag.Mensagem = "Cnpj já cadastrado para uma software house";
            }

            return View("Index");
        }
        public ActionResult ErroLogin(int? param)
        {
            if (param == 7)
            {
                ViewBag.Mensagem = "Erro ao acessar o banco de dados, verifique sua conexão e tente novamente ";
            }
            return View();
        }
    }
}