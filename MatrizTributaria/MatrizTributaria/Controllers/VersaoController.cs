using MatrizTributaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class VersaoController : Controller
    {
        //Objego context
        readonly MatrizDbContext db;

        public VersaoController()
        {
            db = new MatrizDbContext();
        }
        // GET: Versao
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var versao = db.Versoes.ToList();
            return View(versao);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}