using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class SimpleHomeController : Controller
    {
        // GET: Cliente/SimpleHome
        public ActionResult Index()
        {
            return View();
        }
    }
}