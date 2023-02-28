using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using System;
using System.Collections.Generic;
using PagedList;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class SoftwareHouseController : Controller
    {
        //Objego context
        readonly MatrizDbContext db;
        List<SoftwareHouse> listSofwareHouse = new List<SoftwareHouse>();
        //Construtor da classe
        public SoftwareHouseController()
        {
            db = new MatrizDbContext();
        }



        // GET: SoftwareHouse
        public ActionResult Index(string param, string ordenacao, string qtdSalvos, string procurarPor, string procuraSoftwareHouse,
            string filtroCorrente, string filtroSoftwareHouse, int? page, int? numeroLinhas)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //variavel auxiliar
            string resultado = param;

            procurarPor = (filtroCorrente != null) ? filtroCorrente : procurarPor; //procura por nome
            procuraSoftwareHouse = (procuraSoftwareHouse != null) ? procuraSoftwareHouse : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroNome = String.IsNullOrEmpty(ordenacao) ? "Nome_desc" : ""; //Se nao vier nula a ordenacao aplicar por nome decrescente

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraSoftwareHouse != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraSoftwareHouse = (procuraSoftwareHouse == null) ? filtroSoftwareHouse : procuraSoftwareHouse;

            ViewBag.FiltroCorrente = procurarPor; //cnpj
          
            ViewBag.FiltroCorrenteSoftwareHouse = procuraSoftwareHouse;

;
            this.listSofwareHouse = db.SoftwareHouses.ToList();
            //procura
            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.listSofwareHouse = this.listSofwareHouse.Where(s => s.Cnpj.Contains(procurarPor)).ToList();
            }
            if (!String.IsNullOrEmpty(procuraSoftwareHouse))
            {
                this.listSofwareHouse = this.listSofwareHouse.Where(s => s.Nome.ToString().Equals(procuraSoftwareHouse)).ToList();
            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
            int numeroPagina = (page ?? 1);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";

            ViewBag.Estados = db.Estados.ToList();
            return View(this.listSofwareHouse.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

            
        }

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
            ViewBag.Estados = new SelectList(db.Estados, "id", "uf");
            //ViewBag.Estados = db.Estados;
         
            var model = new SoftwareHouseViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SoftwareHouseViewModel model)
        {
            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;
            var hash = new Hash(SHA512.Create());
            //iformando a data do dia da criação do registro
            model.DataCad = DateTime.Now;
            model.DataAlt = DateTime.Now;
            model.Ativo = 1; //ativando o registro no cadastro
            
            //ativando o registro no cadastro
            if (ModelState.IsValid)
            {

                //verificar se existe a sofware house no banco
                var softhouse = from s in db.SoftwareHouses select s; //select na tabela
                                                                      //where: Pegar somente o registro do cnpj da empresa do usuario (da sessão)
                softhouse = softhouse.Where(s => s.Cnpj.Contains(model.Cnpj));

                if (softhouse.Count() > 0)
                {
                    int par = 8;

                    return RedirectToAction("../Erro/Erro", new { param = par });
                }
                var softw = new SoftwareHouse()
                {

                    Nome = model.Nome,
                    RazaoSocial = model.RazaoSocial,
                    Cnpj = model.Cnpj,
                    Logradouro = model.Logradouro,
                    Numero = model.Numero,
                    CEP = model.CEP,
                    Complemento = model.Complemento,
                    Cidade = model.Cidade,
                    Estado = model.Estado,
                    Telefone = model.Telefone,
                    Ativo = model.Ativo,
                    Email = model.Email,


                };

                try
                {
                    db.SoftwareHouses.Add(softw);
                    db.SaveChanges();
                    regSalvos++;
                    resultado = "Registro Salvo com Sucesso!!";



                }
                catch (Exception e)
                {
                    string ex = e.ToString();
                    regSalvos = 0;
                    resultado = "Não foi possivel salvar o registro!!";

                }

            }
            else
            {
               
                ViewBag.Estados = db.Estados;
                return View();
            }
            ViewBag.Estados = db.Estados;

            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });
        }
    }
}