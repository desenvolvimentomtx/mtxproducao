using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class CstPisCofinsSaidaController : Controller
    {

        //contexto do banco de dados
        readonly MatrizDbContext db;

        //construtor da classe
        public CstPisCofinsSaidaController()
        {
            db = new MatrizDbContext();
        }

        // GET: CstPisCofinsSaida
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var cstPisCofinsSaida = db.CstPisCofinsSaidas.ToList();
            return View(cstPisCofinsSaida);
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
            CstPisCofinsSaida cstPCS = db.CstPisCofinsSaidas.Find(id);
            if (cstPCS == null)
            {
                return HttpNotFound();
            }


            ViewBag.DataCad = cstPCS.dataCad;
            ViewBag.DataAlt = cstPCS.dataAlt;
            return View(cstPCS);

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
            CstPisCofinsSaida cstS = db.CstPisCofinsSaidas.Find(id);
            if (cstS == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = cstS.dataCad;
            ViewBag.DataAlt = cstS.dataAlt;
            return View(cstS);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CstPisCofinsSaida cstS = db.CstPisCofinsSaidas.Find(id);
            db.CstPisCofinsSaidas.Remove(cstS);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            CstPisCofinsSaida cstS = db.CstPisCofinsSaidas.Find(id);
            if (cstS == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataAlt = DateTime.Now;

            return View(cstS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codigo, descricao, datacad, dataalt")] CstPisCofinsSaida model)
        {
            if (ModelState.IsValid)
            {
                var cstS = db.CstPisCofinsSaidas.Find(model.codigo);
                if (cstS == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                model.dataAlt = DateTime.Now;
                cstS.codigo = model.codigo;
                cstS.descricao = model.descricao;
                cstS.dataAlt = model.dataAlt;


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
            var model = new CstPisCofinsSaidaViewModel();
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CstPisCofinsSaidaViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;

            if (ModelState.IsValid)
            {

                var cstS = new CstPisCofinsSaida() {

                    codigo = model.codigo,
                    descricao = model.descricao,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt

            };
                


                db.CstPisCofinsSaidas.Add(cstS);
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