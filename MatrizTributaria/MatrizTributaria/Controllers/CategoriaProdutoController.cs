using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class CategoriaProdutoController : Controller
    {
        //Objego context
        readonly MatrizDbContext db;

        //Construtor da classe
        public CategoriaProdutoController()
        {
            db = new MatrizDbContext();
        }
        // GET: CategoriaProduto
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            var categoriaProduto = db.CategoriaProdutos.ToList();
            return View(categoriaProduto);
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

            var model = new CategoriaProdutoViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoriaProdutoViewModel model)
        {
            //iformando a data do dia da criação do registro

            if (ModelState.IsValid)
            {

                var categoria = new CategoriaProduto()
                {
                    descricao = model.CategoriaDescricao

                };
                

                db.CategoriaProdutos.Add(categoria);
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
            CategoriaProduto categoria = db.CategoriaProdutos.Find(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, descricao")] CategoriaProduto model)
        {

            if (ModelState.IsValid)
            {
                var categoria = db.CategoriaProdutos.Find(model.id);
                if (categoria == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                categoria.descricao = model.descricao;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        // GET: Produtos/Delete/5
        public ActionResult Delete(int? id, string msg)
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
            if(msg != null)
            {
                ViewBag.Menssagem = "Categoria não pode ser excluída, pertence a um ou mais produtos!!";
            }
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            CategoriaProduto categoria = db.CategoriaProdutos.Find(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(categoria);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            CategoriaProduto categoria = db.CategoriaProdutos.Find(id);
           var prodCat = (from a in db.Produtos where a.idCategoria == id select a.Id).FirstOrDefault();
            if(prodCat != 0)
            {
               
                return RedirectToAction("Delete", new { msg = "Categoria não pode ser excluída, pertence a um ou mais produtos!!" });
            }
            db.CategoriaProdutos.Remove(categoria);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
            CategoriaProduto categoria = db.CategoriaProdutos.Find(id);
            if (categoria == null)
            {
                return HttpNotFound();
            }



            return View(categoria);
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