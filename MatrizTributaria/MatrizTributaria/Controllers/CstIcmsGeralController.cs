using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class CstIcmsGeralController : Controller
    {
        //Objego context
       readonly MatrizDbContext db;

        //Construtor
        public CstIcmsGeralController()
        {
            db = new MatrizDbContext();
        }
        // GET: CstIcmsGeral
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var cstIcms = db.CstIcmsGerais.ToList();
            return View(cstIcms);
        }

        //detalhes
        public ActionResult Detalhes(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CstIcmsGeral cstIcmGeral = db.CstIcmsGerais.Find(id);
            if (cstIcmGeral == null)
            {
                return HttpNotFound();
            }


            ViewBag.DataCad = cstIcmGeral.dataCad;
            ViewBag.DataAlt = cstIcmGeral.dataAlt;
            return View(cstIcmGeral);

        }

        //editar Nível
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 2;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CstIcmsGeral cstIcms = db.CstIcmsGerais.Find(id);
            if (cstIcms == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataAlt = DateTime.Now;
            return View(cstIcms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Codigo, Descricao, DataCad, DataAlt")] CstIcmsGeral model)
        {
            if (ModelState.IsValid)
            {
                var cstIcms = db.CstIcmsGerais.Find(model.codigo);
                if (cstIcms == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                model.dataAlt = DateTime.Now;
                cstIcms.dataAlt = model.dataAlt;
                cstIcms.descricao = model.descricao;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Produtos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 3;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            CstIcmsGeral cstIcms = db.CstIcmsGerais.Find(id);
            if (cstIcms == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = cstIcms.dataCad;
            ViewBag.DataAlt = cstIcms.dataAlt;
            return View(cstIcms);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CstIcmsGeral cstIcms = db.CstIcmsGerais.Find(id);
            db.CstIcmsGerais.Remove(cstIcms);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        //Create
        public ActionResult Create()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 1;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }
            var model = new CstIcmsGeralViewModel();
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CstIcmsGeralViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;

            if (ModelState.IsValid)
            {

                var cstIcms = new CstIcmsGeral() {
                    codigo = model.codigo,
                    descricao = model.descricao,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt


            };
                

                db.CstIcmsGerais.Add(cstIcms);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
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