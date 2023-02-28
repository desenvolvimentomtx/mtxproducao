using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{

    public class TokenController : Controller
    {
        //Objego context
        readonly MatrizDbContext db;
        List<SoftwareHouse> listSofwareHouse = new List<SoftwareHouse>();
        List<Token> listToken = new List<Token>();
        //Construtor da classe
        public TokenController()
        {
            db = new MatrizDbContext();
        }
        // GET: Token
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


            //this.listSofwareHouse = db.SoftwareHouses.ToList();
            this.listToken = db.Tokens.ToList();
            //procura
            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.listToken = this.listToken.Where(s => s.SoftwareHouse.Cnpj.Contains(procurarPor)).ToList();
            }
            if (!String.IsNullOrEmpty(procuraSoftwareHouse))
            {
                this.listToken = this.listToken.Where(s => s.SoftwareHouse.Nome.ToString().Equals(procuraSoftwareHouse)).ToList();
            }

            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
            int numeroPagina = (page ?? 1);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";

           
            return View(this.listToken.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }
        public ActionResult GerarToken()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            ////Busca a softwareHaouse
            //SoftwareHouse softh = db.SoftwareHouses.Find(id);


            //if(param != null)
            //{
            //    ViewBag.Mensagem = param;
            //    return RedirectToAction("Index", new { param = "Data não pode ser nula"});

            //}
            //else
            //{
            //    //verificar se a softwarehouse ja possui token
            //    if (softh.Chave != "")
            //    {
            //        var tok = db.Tokens.ToList();
            //        tok = tok.Where(m => m.idSofwareHouse.Equals(id) && m.Tokens.Equals(softh.Chave)).ToList();

            //        if (tok.Count() > 0)
            //        {
            //            ViewBag.MensagemChave = "A Chave será trocada caso continue";
            //            return View(softh);
            //        }


            //    }

            //}




            ////procurar a software house: se estiver nula nao existe, se nao procurar seu token na tabela de token
            //if (softh == null)
            //{

            //        return RedirectToAction("Index", new { param = "Registro Não Encontrado" });



            //}
            //else //verificar se na tabela token tem esse id
            //{
            //    var tk = db.Tokens.ToList();
            //    tk = tk.Where(m => m.idSofwareHouse.Equals(id)).ToList();
            //    if(tk.Count() == 0)
            //    {
            //        return View(softh);
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", new { param = "Software House já possue Token" });
            //    }
            //}

            ViewBag.SofwareHouse = db.SoftwareHouses.Where(m=>m.Chave ==null || m.Chave == "");
           
            var model = new TokenViewModel();
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GerarToken(TokenViewModel model)
        {
            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            model.DataCad = DateTime.Now;
            model.Ativo = 1;
            if (ModelState.IsValid)
            {
                //Buscar cnpj da softwareRouse
                var softwH = db.SoftwareHouses.Find(model.idSofwareHouse);



                string cnpjSH = softwH.Cnpj.ToString();
                string chaveCript = cnpjSH + "MTX" + model.Vencimento.ToString(); //token criptografado

                var token = new Token()
                {
                    Tokens = Criptografia.Encrypt(chaveCript),
                    Vencimento = model.Vencimento,
                    DataCad = model.DataCad,
                    Ativo = model.Ativo,
                    idSofwareHouse = model.idSofwareHouse

                };



                softwH.Chave = token.Tokens;

               

                db.Tokens.Add(token);
                regSalvos++;
                resultado = "Registro Salvo com Sucesso!!";
                db.SaveChanges();
               
            }
            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });

        }

        public ActionResult GerarTokenSalvar(int Id, DateTime? dataVenc, Token tkNovo)
        {
            int regSalvos = 0;
            string resultado = "";
            string vencimentoToken = "";
            if(dataVenc != null)
            {
                DateTime dt = (DateTime)dataVenc;
                vencimentoToken = dt.ToString("d"); //vencimento do token
            }
            else
            {
                return RedirectToAction("GerarToken", new { param = "Data não pode ser nula", Id = Id });
            }
           
            
            //montar o objeto token se nao tiver o id da softwhouse
            //var tkNovo = new Token();

            
            //Busca a softwareHaouse
            SoftwareHouse softh = db.SoftwareHouses.Find(Id);

            
            //Buscar cnpj da softwareRouse
            string cnpjSH = softh.Cnpj.ToString(); //cnpj da softwarehouse
            string chaveCript = cnpjSH + "MTX" + vencimentoToken; //token criptografado

            //Token tkNovo = new Token();
            //montar objeto
            tkNovo.Vencimento = dataVenc;
            tkNovo.Ativo = 1;
            tkNovo.idSofwareHouse = Id;
            tkNovo.Tokens = Criptografia.Encrypt(chaveCript); //token criptografado

            //atualizar campo chave da softwarehouse
            softh.Chave = tkNovo.Tokens.ToString();
            //add ao contexo

            var novoToken = new Token()
            {

                Tokens = tkNovo.Tokens,
                Vencimento = tkNovo.Vencimento,
                DataCad = DateTime.Now,
                Ativo = tkNovo.Ativo,
                idSofwareHouse = tkNovo.idSofwareHouse,
                
        };
            db.Tokens.Add(novoToken);
            db.SaveChanges();

           
          
          

            regSalvos++;
            resultado = "Registro Salvo com Sucesso!!";
            //string descripto = Criptografia.Decrypt(tkNovo.Tokens.ToString());

            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });
        }
    }
}