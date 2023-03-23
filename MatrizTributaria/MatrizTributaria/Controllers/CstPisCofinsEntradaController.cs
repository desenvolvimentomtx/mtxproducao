using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class CstPisCofinsEntradaController : Controller
    {
        readonly MatrizDbContext db;

        public CstPisCofinsEntradaController()
        {
            db = new MatrizDbContext();
        }
        // GET: CstPisCofinsEntrada
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var cstPisConfinsEntrada = db.CstPisCofinsEntradas.ToList();
            return View(cstPisConfinsEntrada);
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
            CstPisCofinsEntrada cstE = db.CstPisCofinsEntradas.Find(id);
            if (cstE == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataAlt = DateTime.Now;

            return View(cstE);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codigo, descricao, datacad, dataalt")] CstPisCofinsEntrada model)
        {
            if (ModelState.IsValid)
            {
                var cstE = db.CstPisCofinsEntradas.Find(model.codigo);
                if (cstE == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                model.dataAlt = DateTime.Now;
                cstE.codigo = model.codigo;
                cstE.descricao = model.descricao;
                cstE.dataAlt = model.dataAlt;


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
            CstPisCofinsEntrada cstPCE = db.CstPisCofinsEntradas.Find(id);
            if (cstPCE == null)
            {
                return HttpNotFound();
            }


            ViewBag.DataCad = cstPCE.dataCad;
            ViewBag.DataAlt = cstPCE.dataAlt;
            return View(cstPCE);

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
            CstPisCofinsEntrada cstE = db.CstPisCofinsEntradas.Find(id);
            if (cstE == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = cstE.dataCad;
            ViewBag.DataAlt = cstE.dataAlt;
            return View(cstE);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CstPisCofinsEntrada cstE = db.CstPisCofinsEntradas.Find(id);
            db.CstPisCofinsEntradas.Remove(cstE);
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
            var model = new CstPisCofinsEntradaViewModel();
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CstPisCofinsEntradaViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;

            if (ModelState.IsValid)
            {

                var cstE = new CstPisCofinsEntrada() 
                {
                    codigo = model.codigo,
                    descricao = model.descricao,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt

            };
                


                db.CstPisCofinsEntradas.Add(cstE);
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