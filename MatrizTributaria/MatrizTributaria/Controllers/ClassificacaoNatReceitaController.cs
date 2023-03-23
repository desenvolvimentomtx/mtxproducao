
using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class ClassificacaoNatReceitaController : Controller
    {
        //Objego context
       readonly MatrizDbContext db;

        public ClassificacaoNatReceitaController()
        {
            db = new MatrizDbContext();
        }


        // GET: ClassificacaoNatReceita
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var classificacaoNatRec = db.ClassificacaoNatReceitas.ToList();
            return View(classificacaoNatRec);
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
            ClassificacaoNatReceita classificacao = db.ClassificacaoNatReceitas.Find(id);
            if (classificacao == null)
            {
                return HttpNotFound();
            }

            return View(classificacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Codigo, Descricao")] ClassificacaoNatReceita model)
        {

            if (ModelState.IsValid)
            {
                var classificacao = db.ClassificacaoNatReceitas.Find(model.Codigo);
                if (classificacao == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                classificacao.Descricao = model.Descricao;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
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
            ClassificacaoNatReceita classificacao = db.ClassificacaoNatReceitas.Find(id);
            if (classificacao == null)
            {
                return HttpNotFound();
            }



            return View(classificacao);
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
            ClassificacaoNatReceita classificacao = db.ClassificacaoNatReceitas.Find(id);
            if (classificacao == null)
            {
                return HttpNotFound();
            }

            return View(classificacao);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassificacaoNatReceita categoria = db.ClassificacaoNatReceitas.Find(id);
            db.ClassificacaoNatReceitas.Remove(categoria);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            var model = new ClassificacaoNatReceitaViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassificacaoNatReceitaViewModel model)
        {
            //iformando a data do dia da criação do registro

            if (ModelState.IsValid)
            {

                var classificacao = new ClassificacaoNatReceita() {

                    Codigo = model.Codigo,
                    Descricao = model.Descricao

            };
               

                db.ClassificacaoNatReceitas.Add(classificacao);
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