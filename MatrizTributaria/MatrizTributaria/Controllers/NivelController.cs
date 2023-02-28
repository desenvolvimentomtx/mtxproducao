using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class NivelController : Controller
    {
        //Objeto context
        readonly MatrizDbContext db;

        public NivelController()
        {
            db = new MatrizDbContext();
        }
        // GET: Nivel
        public ActionResult Index()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            var nivel = db.Niveis.ToList();
           
            return View(nivel);
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
            var model = new NivelViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NivelViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.DataCad = DateTime.Now;
            model.DataAlt = DateTime.Now;
            model.Ativo = 1; //ativando o registro no cadastro
            if (ModelState.IsValid)
            {

                var nivel = new Nivel()
                {
                    descricao = model.descricao,
                    Ativo = model.Ativo,
                    DataCad = model.DataCad,
                    DataAlt = model.DataAlt

                };
               

                db.Niveis.Add(nivel);
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
            Nivel nivel = db.Niveis.Find(id);
            if (nivel == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataCad = db.Niveis.Find(nivel.id).DataCad;
            ViewBag.DataAlt = db.Niveis.Find(nivel.id).DataAlt;

            return View(nivel);

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
            Nivel nivel = db.Niveis.Find(id);
            if (nivel == null)
            {
                return HttpNotFound();
            }

            return View(nivel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descricao,ativo")] Nivel model)
        {
            if (ModelState.IsValid)
            {
                var nivel = db.Niveis.Find(model.id);
                if (nivel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                model.DataAlt = DateTime.Now;
                nivel.DataAlt = model.DataAlt;
                nivel.descricao = model.descricao;
                nivel.Ativo = model.Ativo;


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
            Nivel nivel = db.Niveis.Find(id);
            if (nivel == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = db.Niveis.Find(nivel.id).DataCad;
            ViewBag.DataAlt = db.Niveis.Find(nivel.id).DataAlt;
            return View(nivel);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nivel nivel = db.Niveis.Find(id);
            db.Niveis.Remove(nivel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //parado
        public JsonResult LerGravar()
        {

            List<Nivel> listaNivel = db.Niveis.ToList(); //lista com todos os niveis

            //faz a seleção para evitar erro de referencia circular
            var niveis = listaNivel.Select(S => new
            {
                id = S.id,
                descricao = S.descricao,
                ativo = S.Ativo

            });

            //serializa o objeto de lista
            string json = JsonConvert.SerializeObject(niveis.ToArray(), Formatting.Indented);

            //var arqui = JsonConvert.DeserializeObject(json);


            //aplica o caminho de salvar o arquivo json
            var dataFile = Server.MapPath("~/Jsons/niveis.json");

            //Escreve no arquivo
            System.IO.File.WriteAllText(@dataFile, json);

            //retorna o json pelo get
            return Json(niveis, JsonRequestBehavior.AllowGet);

        }

        /*Chama o metodo LerGravar atraves do jquery e ajax
         na abertura da view ele chama*/
        public ActionResult Retorno()
        {
            return View();
        }

        //public ActionResult LerArquivo()
        //{
        //    //aplica o caminho de salvar o arquivo json
        //    var dataFile = Server.MapPath("~/Jsons/niveis.json");



        //    return View();

        //}

        public ActionResult LerArquivoJson()
        {
            return View();
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