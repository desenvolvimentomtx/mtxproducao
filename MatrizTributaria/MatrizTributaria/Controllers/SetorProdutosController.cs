using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class SetorProdutosController : Controller
    {

        MatrizDbContext db;

        public SetorProdutosController()
        {
            db = new MatrizDbContext();
        }
        // GET: SetorProdutos
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            var setorProdutos = db.SetorProdutos.ToList();
            return View(setorProdutos);
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
            SetorProdutos setor = db.SetorProdutos.Find(id);
            if (setor == null)
            {
                return HttpNotFound();
            }



            return View(setor);

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
                return RedirectToAction("Erro", new { param = par });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SetorProdutos setor = db.SetorProdutos.Find(id);
            if (setor == null)
            {
                return HttpNotFound();
            }

            return View(setor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descricao")] SetorProdutos model)
        {

            if (ModelState.IsValid)
            {
                var setor = db.SetorProdutos.Find(model.id);
                if (setor == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                setor.descricao = model.descricao;


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
            SetorProdutos setor = db.SetorProdutos.Find(id);
            if (setor == null)
            {
                return HttpNotFound();
            }

            return View(setor);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SetorProdutos setor = db.SetorProdutos.Find(id);
            db.SetorProdutos.Remove(setor);
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
            var model = new SetorProdutosViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SetorProdutosViewModel model)
        {


            if (ModelState.IsValid)
            {

                var setor = new SetorProdutos();
                setor.descricao = model.descricao;


                db.SetorProdutos.Add(setor);
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