using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class LegislacaoController : Controller
    {
        //Objego context
       readonly MatrizDbContext db;

        //Construtor
        public LegislacaoController()
        {
            db = new MatrizDbContext();
        }

        // GET: Legislacao
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var legislacao = db.Legislacoes.ToList();
            return View(legislacao);
        }

        //Chamando a view para criar o usuario
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
            var model = new LegislacaoViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LegislacaoViewModel model)
        {


            if (ModelState.IsValid)
            {

                var legislacao = new Legislacao() 
                {
                    fundLegal = model.fundLegal

                };
                



                db.Legislacoes.Add(legislacao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
            Legislacao legislacao = db.Legislacoes.Find(id);
            if (legislacao == null)
            {
                return HttpNotFound();
            }

            return View(legislacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, fundlegal")] Legislacao model)
        {
            if (ModelState.IsValid)
            {
                var legislacao = db.Legislacoes.Find(model.id);
                if (legislacao == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                legislacao.fundLegal = model.fundLegal;


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
            Legislacao legislacao = db.Legislacoes.Find(id);
            if (legislacao == null)
            {
                return HttpNotFound();
            }

            return View(legislacao);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Legislacao legislacao = db.Legislacoes.Find(id);
            db.Legislacoes.Remove(legislacao);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            Legislacao legislacao = db.Legislacoes.Find(id);
            if (legislacao == null)
            {
                return HttpNotFound();
            }


            return View(legislacao);

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
