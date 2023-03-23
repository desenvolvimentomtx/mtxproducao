using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class NaturezaReceitaController : Controller
    {
        //Objego context
       readonly MatrizDbContext db;

        public NaturezaReceitaController()
        {
            db = new MatrizDbContext();
        }
        // GET: NaturezaReceita
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var naturezaReceita = db.NaturezaReceitas.ToList();
            return View(naturezaReceita);
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
                int par = 1;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NaturezaReceita natRec = db.NaturezaReceitas.Find(id);
            if (natRec == null)
            {
                return HttpNotFound();
            }
            ViewBag.Classificacao = db.ClassificacaoNatReceitas;
            return View(natRec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, descricao, idClassificacao")] NaturezaReceita model)
        {

            if (ModelState.IsValid)
            {
                var natRec = db.NaturezaReceitas.Find(model.id);
                if (natRec == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                natRec.descricao = model.descricao;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Classificacao = db.ClassificacaoNatReceitas;
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
            NaturezaReceita natRec = db.NaturezaReceitas.Find(id);
            if (natRec == null)
            {
                return HttpNotFound();
            }


            return View(natRec);

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
            NaturezaReceita natRec = db.NaturezaReceitas.Find(id);
            if (natRec == null)
            {
                return HttpNotFound();
            }

            return View(natRec);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NaturezaReceita natRec = db.NaturezaReceitas.Find(id);
            db.NaturezaReceitas.Remove(natRec);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //Chamando a view para criar a natureza da receita
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
            ViewBag.Classificacao = db.ClassificacaoNatReceitas;
            var model = new NaturezaReceitaViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NaturezaReceitaViewModel model)
        {
            
            //iformando a data do dia da criação do registro

            if (ModelState.IsValid)
            {

                var natRece = new NaturezaReceita()
                {

                    id = model.id,
                    descricao = model.descricao,
                    idClassificacao = model.idClassificacao

                };
                
                db.NaturezaReceitas.Add(natRece);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Classificacao = db.ClassificacaoNatReceitas;
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