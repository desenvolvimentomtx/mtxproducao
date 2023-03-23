using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
//alteração de teste
namespace MatrizTributaria.Controllers
{
    public class ProdutoController : Controller
    {
        //Objeto context
        readonly MatrizDbContext db;
        List<Produto> prod;
        List<Produto> prodMTX = new List<Produto>();
        List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>(); //TESTE COM A VIEW DA TRIBUTAÇÃO
        List<Produto> produtosMTX = new List<Produto>();
        IQueryable<TributacaoGeralView> lstCli;
        List<TributacaoNCM> tribNCM = new List<TributacaoNCM>();
        //origem e destino
        string ufOrigem = "";
        string ufDestino = "";

        string ufOrigemNCM = "";
        string ufDestinoNCM = "";
        public ProdutoController()
        {
            db = new MatrizDbContext();
        }
        public ActionResult Index(string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procuraNCM, string procuraCEST,  string filtroCorrente,  string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
           
            procurarPor = (procurarPor != null) ? procurarPor.Trim() : procurarPor;
            //variavel auxiliar
            string resultado = param;
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //colocar a categoria no tempdata
            if(procuraCate != null)
            {
                if (TempData["categoria"] != null)
                {
                    if (!procuraCate.Equals(TempData["categoria"].ToString()))
                    {
                        TempData["categoria"] = procuraCate;
                        TempData.Keep("categoria");
                    }
                }
                else
                {
                    TempData["categoria"] = procuraCate;
                    TempData.Keep("categoria");
                }

                

            }
            else
            {
                if(TempData["categoria"] != null)
                {
                    if(filtroCate != null)
                    {
                        procuraCate = null;
                        TempData.Keep("categoria");
                    }
                    else
                    {
                        procuraCate = TempData["categoria"].ToString();
                        TempData.Keep("categoria");
                    }
                  
                  

                }
            }

            TempData.Keep("categoria");


            //numero de linhas
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) ||  (procuraCEST != null) || (procuraNCM != null)  || (procuraCate != null)  ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
            
            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
           
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
           
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

          
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //converter o valor da procura por setor ou categoria em inteiro
           
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }


            //aqui vai a lista
            var produtos = from s in db.Produtos select s;


            if (!String.IsNullOrEmpty(procurarPor))
            {
                produtos = ((codBarrasL != 0) ? (produtos.Where(s => s.codBarras.ToString().StartsWith(codBarrasL.ToString()))) : produtos = (produtos.Where(s => s.descricao.ToUpper().StartsWith(procurarPor.ToUpper()))));

            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                produtos = produtos.Where(s => s.cest == procuraCEST);
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                produtos = produtos.Where(s => s.ncm == procuraNCM);

            }
                       
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                produtos = produtos.Where(s => s.idCategoria.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    produtos = produtos.OrderByDescending(s => s.descricao);
                    break;
                case "Produto_asc":
                    produtos = produtos.OrderBy(s => s.descricao);
                    break;
                case "Id_desc":
                    produtos = produtos.OrderBy(s => s.Id);
                    break;
                default:
                    produtos =produtos.OrderBy(s => s.descricao);
                    break;


            }
            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
          
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
           

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(produtos.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist




        }


        private List<Produto> GetProduto()
        {
            List<Produto> model = new List<Produto>();
            model = (from s in db.Produtos select s).ToList();

            return model;

        }
        // GET: Produto
        //public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string linhasNum)
        //{
        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("../Home/Login");
        //    }

        //    // ViewBag.NumLinhas = linhasNum;
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
        //    ViewBag.CatProduto = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }
        //    ViewBag.CurrentFilter = searchString;

        //    var produtos = from s in db.Produtos select s;
        //    if (!String.IsNullOrEmpty(searchString))
        //    {

        //        produtos = produtos.Where(s => s.descricao.ToString().ToUpper().Contains(searchString.ToUpper()) || s.categoriaProduto.descricao.ToString().ToUpper().Contains(searchString.ToUpper()));


        //    }
        //    switch (sortOrder)
        //    {
        //        case "Produto_desc":
        //            produtos = produtos.OrderByDescending(s => s.Id);
        //            break;
        //        case "CatProd":
        //            produtos = produtos.OrderBy(s => s.idCategoria);
        //            break;
        //        case "CatProd_desc":
        //            produtos = produtos.OrderByDescending(s => s.idCategoria);
        //            break;
        //        default:
        //            produtos = produtos.OrderBy(s => s.Id);
        //            break;
        //    }
        //    int pageSize = 0;

        //    if (String.IsNullOrEmpty(linhasNum))
        //    {
        //        pageSize = 10;
        //    }
        //    else
        //    {

        //        ViewBag.Texto = linhasNum;
        //        pageSize = Int32.Parse(linhasNum);
        //    }


        //    int pageNumber = (page ?? 1);

        //    //var produtos = db.Produtos.ToList();
        //    return View(produtos.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho
        //}
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
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            // ViewBag.Categoria = db.Produtos.Find(produto.idCategoria).descricao; //categoria
            ViewBag.DataCad = produto.dataCad;
            ViewBag.DataAlt = produto.dataAlt;

            return View(produto);
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
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataCad = produto.dataCad;
            ViewBag.DataAlt = produto.dataAlt;
            return View(produto);

        }
        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Produto produto = db.Produtos.Find(id);
            db.Produtos.Remove(produto);
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
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }

            ViewBag.DataAlt = DateTime.Now;
            ViewBag.Categorias = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            //ViewBag.Categorias = db.CategoriaProdutos;
            return View(produto);
        }


        [HttpGet]
        public ActionResult EditVarios(string array)
        {
            string[] dadosDoCadastro = array.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            ViewBag.DataAlt = DateTime.Now;
            ViewBag.Categorias = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();


            return View();
            
        }

        [HttpGet]
        public ActionResult EdtVariosPost(string strDados, string ncm, string cest, int? categ)
        {
            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;

            string resultado = "";


            //varivael para recebe o novo ncm
            string ncmMudar = "";
            string cestMudar = "";
            int? catgMudar = (categ == null ? 0 : categ);

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            //ternario para remover eventuais espaços
            ncmMudar = ncm != "" ? ncm.Trim() : null; 
            cestMudar = cest != "" ? cest.Trim() : null;

            //tirar os pontos da string (NO CASO DO NCM)
            if (ncmMudar != null)
            {
                if (ncmMudar != "")
                {
                    ncmMudar = ncmMudar.Replace(".", ""); 
                }

            }

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                if (prod != null)
                {
                    if (cestMudar != null)
                    {
                        if (cestMudar != "")
                        {
                            if (prod.cest != cestMudar)
                            {
                                prod.cest = cestMudar;
                                //SALVAR
                                prod.auditadoNCM = 1;
                                prod.dataAlt = DateTime.Now; //data da alteração
                            }
                           
                        }

                    }

                    if (ncmMudar != null)
                    {
                        if (ncmMudar != "")
                        {
                            if (prod.ncm != ncmMudar)
                            {
                                prod.ncm = ncmMudar;
                                //SALVAR
                                prod.auditadoNCM = 1;
                                prod.dataAlt = DateTime.Now; //data da alteração
                            }
                        }

                    }
                    if (catgMudar != 0)
                    {
                        if (catgMudar != null)
                        {
                            if (prod.idCategoria != catgMudar)
                            {
                                prod.idCategoria = (int)catgMudar;
                                //SALVAR
                                prod.auditadoNCM = 1;
                                prod.dataAlt = DateTime.Now; //data da alteração
                            }
                        }

                    }
                   
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                    }
                    catch (Exception e)
                    {
                        string ex = e.ToString();
                        regNSalvos++;
                    }
                }




            }
            if (regSalvos > 0)
            {
                resultado = "Registro Salvo com Sucesso!!";

            }
            else
            {
                resultado = "Problemas ao salvar o registro";
                //Redirecionar para registros
                return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
            }

            //Redirecionar para registros
            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, codinterno, codbarras, descricao, cest, ncm, datacad, dataalt, idcategoria, status")] Produto model)
        {
            //Aa aliquotas podem estar com a virgula, na hora de salvar o sistema passa para decimal
            string buscaNCM = model.ncm != null ? model.ncm.Trim() : null; //ternario para remover eventuais espaços
            if(model.ncm != null)
            {
                model.ncm = buscaNCM.Replace(".", ""); //tirar os pontos da string
            }
            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            if (ModelState.IsValid)
            {
                var produto = db.Produtos.Find(model.Id);
                if (produto == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                

                model.dataAlt = DateTime.Now;

                produto.Id = model.Id;
                produto.cest = model.cest;
                produto.codBarras = model.codBarras;
                produto.codInterno = model.codInterno;
                produto.descricao = model.descricao.ToUpper();
                produto.ncm = model.ncm;
                produto.idCategoria = model.idCategoria;
                produto.status = model.status;
                produto.dataAlt = model.dataAlt;

                try{
                    db.SaveChanges();
                    TempData["tributacaoMTX"] = null;
                    regSalvos++;
                    resultado = "Registro Salvo com Sucesso!!";

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                }
                
                //return RedirectToAction("Index");
            }
            //ViewBag.Categorias = db.CategoriaProdutos;
            ViewBag.Categorias = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            //Redirecionar para a tela de graficos
            return RedirectToAction("Index", new { param = resultado, qtdSalvos = regSalvos });
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
            ViewBag.Categoria = db.CategoriaProdutos;
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;
            var model = new ProdutoViewModel();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdutoViewModel model)
        {
            //iformando a data do dia da criação do registro
            model.dataCad = DateTime.Now;
            model.dataAlt = DateTime.Now;
            model.status = 1; //ativando o registro no cadastro
            if (ModelState.IsValid)
            {

                var produto = new Produto()
                {

                    codInterno = model.codInterno,
                    codBarras = model.codBarras,
                    descricao = model.descricao,
                    cest = model.cest,
                    ncm = model.ncm,
                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt,
                    idCategoria = model.idCategoria,
                    status = model.status


            };
              


                db.Produtos.Add(produto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categoria = db.CategoriaProdutos;

            return View(model);
        }

        //produto nao tem origem e destino, é apenas o cadastro do produto
        [HttpGet]
        public ActionResult GraficoAnaliseProdutos()
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            //montar select estado origem e destino
            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");

            TempData["procuraCAT"] = null;

            //verifica carregamento da tabela
            VerificaTempDataProd();



            ViewBag.CodBarras = this.prodMTX.Count(a => a.codBarras.ToString() != "0");
            ViewBag.CodBarrasNull = this.prodMTX.Count(a => a.codBarras.ToString() == "0");


            ViewBag.Cest = this.prodMTX.Count(a => a.cest != null);
            ViewBag.CestNull = this.prodMTX.Count(a => a.cest == null);



            

            ViewBag.Ncm = this.prodMTX.Count(a => a.ncm != null);
            ViewBag.NcmNull = this.prodMTX.Count(a => a.ncm == null);


            return View();

        }


        //EditNCM
        [HttpGet]
        public ActionResult EditMassa(string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
            string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
            string filtroCorrenteAudNCM) 
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

           
            //variavel auxiliar
            string resultado = param;

            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //auditadosNCM = (auditadosNCM != null) ? auditadosNCM : "0";



            filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria

            if(filtraPor != "Setor")
            {
               
                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

           
            

            if(procuraCate == null || procuraCate == "" || procuraCate == "null")
            {
                if(TempData["procuraCAT"] != null)
                {
                    procuraCate = TempData["procuraCAT"].ToString();
                }
                else
                {
                    procuraCate = null;
                    TempData["procuraCAT"] = null;
                }
               
            }
            else
            {
                if(TempData["procuraCAT"] != null)
                {
                    if (procuraCate != (TempData["procuraCAT"].ToString()))
                    {
                        TempData["procuraCAT"] = procuraCate;
                    }


                }
                else
                {
                    TempData["procuraCAT"] = procuraCate;
                }
                
                
                
            }

           



            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
           
            ViewBag.FiltroCorrenteAuditado =  (auditadosNCM != null) ? auditadosNCM : "0";



            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

           
           

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");
            TempData.Keep("procuraCAT");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

            auditadosNCM = (auditadosNCM == null) ? filtroCorrenteAudNCM : auditadosNCM; //todos os que não foram auditados

            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;

            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;
            //ViewBag.FiltroCorrenteAuditado = auditadosNCM; 
            ViewBag.FiltroCorrenteSetor = procuraSetor;
            if (TempData["procuraCAT"] == null)
            {
                ViewBag.FiltroCorrenteCate = procuraCate;
            }
            else
            {
                ViewBag.FiltroCorrenteCate = TempData["procuraCAT"].ToString();
            }
           
            ViewBag.FiltroFiltraPor = filtraPor;

            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 1);
                    break;
                case "2": //TODOS
                    this.lstCli = this.lstCli.Where(s => s.ID !=null);
                    break;
            }


            switch (opcao)
            {
                case "Com NCM":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //COM NCM
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO != null);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == null);
                            break;
                    }
                    break;
                case "Sem NCM":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //SEM NCM
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO != null);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == null);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           //  this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //verificar isso - setor categoria
            //Busca por setor
            if (!String.IsNullOrEmpty(procuraSetor))
            {
                this.lstCli = this.lstCli.Where(s => s.ID_SETOR.ToString() == procuraSetor);
              

            }
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.lstCli = this.lstCli.OrderByDescending(s => s.DESCRICAO_PRODUTO);
                    break;
                case "Produto_asc":
                    this.lstCli = this.lstCli.OrderBy(s => s.DESCRICAO_PRODUTO);
                    break;
                case "Id_desc":
                    this.lstCli = this.lstCli.OrderBy(s => s.ID);
                    break;
                default:
                    this.lstCli = this.lstCli.OrderBy(s => s.DESCRICAO_PRODUTO);
                    break;


            }


            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().ToList().OrderBy(s => s.descricao).ToList();
          


            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();


            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist



        }

    
        [HttpGet]
        public ActionResult EditMassaModal(string array)
        {

            string[] dadosDoCadastro = array.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            /*ViewBagDiferente para pins cofins*/
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;

            /*ViewBags com os dados necessários para preencher as dropbox na view*/
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;

            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;


            return View();
        }

        [HttpGet]
        public ActionResult EditMassaNCMModal(string id, string ncm)
        {
            //receber os dados do ncm para alterar
            string ncmAlterar = ncm;
            ViewBag.NCM = ncmAlterar;

            /*ViewBagDiferente para pins cofins*/
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;

            /*ViewBags com os dados necessários para preencher as dropbox na view*/
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;

            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            return View();
        }

        //proximo
        [HttpGet]
        public ActionResult EditMassaNCMModalPost(string ncm, string fecp, string CodReceita, string CstSaidaPisCofins, string aliqSaidaPis, string aliqSaidaCofins,
            string IdFundamentoLegal, string CstVendaVarejoConsFinal, string alVeVarCF, string alVeVarCFSt, string rBcVeVarCF, string rBcSTVeVarCF, 
            string CstVendaVarejoCont, string alVeVarCont, string alVeVarContSt, string rBcVeVarCont, string rBcSTVeVarCont, string CstVendaAtaCont,
            string aliqIcmsVendaAtaCont, string aliqIcmsSTVendaAtaCont, string redBaseCalcIcmsVendaAtaCont, string redBaseCalcIcmsSTVendaAtaCont, 
            string CstVendaAtaSimpNacional, string alVSN, string alVSNSt, string rBcVSN, string rBcSTVSN, string IdFundLegalSaidaICMS, string cest, string ufOrigem,
            string ufDestino)
        {
            int regSalvos = 0;
            int regNSalvos = 0;
            int regParaSalvar = 0;

            string retorno = "";
            //buscar os cst pela descrição
            int? cstSaidaPisCofins       = (CstSaidaPisCofins       == "") ? null : (int?)(long)(from a in db.CstPisCofinsSaidas where a.descricao == CstSaidaPisCofins select a.codigo).FirstOrDefault();
            int? cstVendaVarejoConsFinal = (CstVendaVarejoConsFinal == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            int? cstVendaVarejoCont      = (CstVendaVarejoCont      == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoCont select a.codigo).FirstOrDefault();
            int? cstVendaAtaCont         = (CstVendaAtaCont         == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaAtaCont select a.codigo).FirstOrDefault();
            int? cstVendaAtaSimpNacional = (CstVendaAtaSimpNacional == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            //natureza da reeita
            int? codNatRec = 0;

            if (CodReceita == "")
            {
                codNatRec = null;
            }
            else
            {
                codNatRec = int.Parse(CodReceita);
            }

            //Fundamento Legal cofins
            int? fundLegalCofins = 0;
            if(IdFundamentoLegal == "")
            {
                fundLegalCofins = null;
            }
            else
            {
                fundLegalCofins = int.Parse(IdFundamentoLegal);
            }

            //fundamento legal icms saida
            int? fundLegalIcmsSaida = 0;
            if(IdFundLegalSaidaICMS == "")
            {
                fundLegalIcmsSaida = null;
            }
            else
            {
                fundLegalIcmsSaida = int.Parse(IdFundLegalSaidaICMS);
            }

            
            
            //Aa aliquotas podem estar com a virgula, na hora de salvar o sistema passa para decimal
            string buscaNCM = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços

            buscaNCM = buscaNCM.Replace(".", ""); //tirar os pontos da string

            VerificaTempData();
            //busca o nmc pelo sua origem e destino
            if (!String.IsNullOrEmpty(buscaNCM))
            {
                this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == buscaNCM && s.UF_ORIGEM == ufOrigem && s.UF_DESTINO == ufDestino) ;

            }
            
            //foreach para atualizar os produtos
            //retira o elemento vazio do array
           //objeto produto
            Tributacao tributaCao = new Tributacao();

            List<TributacaoGeralView> tribMts = this.lstCli.ToList();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < tribMts.Count(); i++)
            {
                int? idTrib = (tribMts[i].ID); //pega o id do registro da tributação a ser alterada
                tributaCao = db.Tributacoes.Find(idTrib); //busca a tributação pelo seu id


                if (ufOrigem != "null" && ufDestino != "null")
                {
                    tributaCao.UF_Origem = ufOrigem;
                    tributaCao.UF_Destino = ufDestino;
                  
                    //verifica se os parametros vieram preenchidos, caso true ele atribui ao objeto e conta um registro para salvar
                    if (cstSaidaPisCofins != null)
                    {
                        tributaCao.cstSaidaPisCofins = cstSaidaPisCofins;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (cstVendaVarejoConsFinal != null)
                    {
                        tributaCao.cstVendaVarejoConsFinal = cstVendaVarejoConsFinal;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (cstVendaVarejoCont != null)
                    {

                        tributaCao.cstVendaVarejoCont = cstVendaVarejoCont;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (cstVendaAtaCont != null)
                    {
                        tributaCao.cstVendaAtaCont = cstVendaAtaCont;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (cstVendaAtaSimpNacional != null)
                    {
                        tributaCao.cstVendaAtaSimpNacional = cstVendaAtaSimpNacional;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (fecp != "")
                    {
                        tributaCao.fecp = decimal.Parse(fecp);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (codNatRec != null)
                    {
                        tributaCao.codNatReceita = codNatRec;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (aliqSaidaPis != "")
                    {
                        tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (aliqSaidaCofins != "")
                    {
                        tributaCao.aliqSaidaCofins = decimal.Parse(aliqSaidaCofins);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (fundLegalCofins != null)
                    {
                        tributaCao.idFundamentoLegal = (fundLegalCofins);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (alVeVarCF != "")
                    {
                        tributaCao.aliqIcmsVendaVarejoConsFinal = decimal.Parse(alVeVarCF);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (alVeVarCFSt != "")
                    {
                        tributaCao.aliqIcmsSTVendaVarejoConsFinal = decimal.Parse(alVeVarCFSt);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (rBcVeVarCF != "")
                    {
                        tributaCao.redBaseCalcIcmsVendaVarejoConsFinal = decimal.Parse(rBcVeVarCF);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (rBcSTVeVarCF != "")
                    {
                        tributaCao.redBaseCalcIcmsSTVendaVarejoConsFinal = decimal.Parse(rBcSTVeVarCF);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (alVeVarCont != "")
                    {
                        tributaCao.aliqIcmsVendaVarejoCont = decimal.Parse(alVeVarCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (alVeVarContSt != "")
                    {
                        tributaCao.aliqIcmsSTVendaVarejo_Cont = decimal.Parse(alVeVarContSt);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (rBcVeVarCont != "")
                    {
                        tributaCao.redBaseCalcVendaVarejoCont = decimal.Parse(rBcVeVarCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (rBcSTVeVarCont != "")
                    {
                        tributaCao.RedBaseCalcSTVendaVarejo_Cont = decimal.Parse(rBcSTVeVarCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (aliqIcmsVendaAtaCont != "")
                    {
                        tributaCao.aliqIcmsVendaAtaCont = decimal.Parse(aliqIcmsVendaAtaCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (aliqIcmsSTVendaAtaCont != "")
                    {
                        tributaCao.aliqIcmsSTVendaAtaCont = decimal.Parse(aliqIcmsSTVendaAtaCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (redBaseCalcIcmsVendaAtaCont != "")
                    {
                        tributaCao.redBaseCalcIcmsVendaAtaCont = decimal.Parse(redBaseCalcIcmsVendaAtaCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (redBaseCalcIcmsSTVendaAtaCont != "")
                    {
                        tributaCao.redBaseCalcIcmsSTVendaAtaCont = decimal.Parse(redBaseCalcIcmsSTVendaAtaCont);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }

                    if (alVSN != "")
                    {
                        tributaCao.aliqIcmsVendaAtaSimpNacional = decimal.Parse(alVSN);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (alVSNSt != "")
                    {
                        tributaCao.aliqIcmsSTVendaAtaSimpNacional = decimal.Parse(alVSNSt);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (rBcVSN != "")
                    {
                        tributaCao.redBaseCalcIcmsVendaAtaSimpNacional = decimal.Parse(rBcVSN);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (rBcSTVSN != "")
                    {
                        tributaCao.redBaseCalcIcmsSTVendaAtaSimpNacional = decimal.Parse(rBcSTVSN);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }
                    if (fundLegalIcmsSaida != null)
                    {
                        tributaCao.idFundLegalSaidaICMS = (fundLegalIcmsSaida);
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }


                }


                //if(ufOrigem != "")
                //{
                //    if(ufDestino != "")
                //    {

                //    }
                //}
               

                if(cest != "")
                {
                    tributaCao.produtos.cest = cest;
                    regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                }


                if(regParaSalvar != 0)
                {
                    tributaCao.auditadoPorNCM = 1; //marca como auditado
                    tributaCao.produtos.auditadoNCM = 1; //marca o produto como auditado tb
                    tributaCao.dataAlt = DateTime.Now; //data da alteração
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                        retorno = "Registro Salvo com Sucesso!!";
                    }
                    catch (Exception e)
                    {
                        string exC = e.ToString();
                        regNSalvos++;
                    }
                }
                else
                {
                    retorno = "Nenhum item do  registro alterado";
                    //Redirecionar para registros
                    return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }

                
               
            }
            //zera a tempdata caso tenha salvo algum registros
            if(regSalvos > 0)
            {
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
            }
            
            //Redirecionar para registros
            return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }
       

        [HttpGet]
        public ActionResult EditTributacaoMassaNCMModal(string id, string ncm, string uf_origem, string uf_destino)
        {
            //entrou na action, mostrar a tributação desse ncm na tela
            //receber os dados do ncm para alterar

            string ncmAlterar = ncm;
            ncmAlterar = ncmAlterar.Replace(".", "");
            ViewBag.NCM = ncmAlterar;
            //buscar na tablea pelo ncm
           // this.tribNCM = db.TributacoesNcm.AsNoTracking().ToList();

           


            //agora um objeto de tributação de produtos
            //this.tribMTX = db.Tributacao_GeralView.AsNoTracking().ToList();
            this.lstCli = from a in db.Tributacao_GeralView select a;


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

           

            //verifica estados origem e destino
            VerificaOriDestNCM(uf_origem, uf_destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigemNCM;
            ViewBag.UfDestino = this.ufDestinoNCM;






            ////pegar os ncm existenstes em cada tabela
            //this.tribNCM = db.TributacoesNcm.AsNoTracking().ToList(); //instancia o objeto.

            ////tributacao pelo estado
            //if(this.ufOrigemNCM != null && this.ufDestinoNCM != null)
            //{
            //    this.tribNCM = this.tribNCM.Where(item => item.ncm.Equals(ncm) && item.UF_Origem.Equals(this.ufOrigemNCM) && item.UF_Destino.Equals(this.ufDestinoNCM)).ToList();

            //}
            //objeto para mostrar os dados
            TributacaoNCM tibNCMObj = (from a in db.TributacoesNcm where a.ncm.Equals(ncmAlterar) && a.UF_Origem.Equals(this.ufOrigemNCM) && a.UF_Destino.Equals(this.ufDestinoNCM) select a).FirstOrDefault();

            if (tibNCMObj != null)
            {
                //piscofins E e S
                int? cst = tibNCMObj.cstEntradaPisCofins;
                ViewBag.CstEntradaPis = cst;
                //int cst = ViewBag.CstEntradaPis.;
                ViewBag.CstEntraPiscofinsDesc = (from a in db.CstPisCofinsEntradas where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqPisE = tibNCMObj.aliqEntPis.ToString();
                ViewBag.AliqCofinsE = tibNCMObj.aliqEntCofins.ToString();

                cst = tibNCMObj.cstSaidaPisCofins;
                ViewBag.CstSaidaPis = cst;
                ViewBag.CstSaidaPiscofinsDesc = (from a in db.CstPisCofinsSaidas where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqPisS = tibNCMObj.aliqSaidaPis.ToString();
                ViewBag.AliqCofinsS = tibNCMObj.aliqSaidaCofins.ToString();

                //venda atacado contribinte
                cst = tibNCMObj.cstVendaAtaCont;
                ViewBag.CstVendaAtaContr = cst;
                ViewBag.CstAtaContrDesc = (from a in db.CstIcmsGerais where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqIcmsVenAtaCont = tibNCMObj.aliqIcmsVendaAtaCont.ToString();
                ViewBag.AliqImcsStVenAtaCont = tibNCMObj.aliqIcmsSTVendaAtaCont.ToString();
                ViewBag.AliqRedBasCalcVenAtaCont = tibNCMObj.redBaseCalcIcmsVendaAtaCont.ToString();
                ViewBag.AliqRedBasCalcSTVenAtaCont = tibNCMObj.redBaseCalcIcmsSTVendaAtaCont.ToString();

                //Venda Ata Simples Nacional
                cst = tibNCMObj.cstVendaAtaSimpNacional;
                ViewBag.CstVendaAtaSimplesNacional = cst;
                ViewBag.CstVendaAtaSimplesNacionalDesc = (from a in db.CstIcmsGerais where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqIcmsVenAtaSn = tibNCMObj.aliqIcmsVendaAtaSimpNacional.ToString();
                ViewBag.AliqImcsStVenAtaSn = tibNCMObj.aliqIcmsSTVendaAtaSimpNacional.ToString();
                ViewBag.AliqRedBasCalcVenAtaSn = tibNCMObj.redBaseCalcIcmsVendaAtaSimpNacional.ToString();
                ViewBag.AliqRedBasCalcSTVenAtaSn = tibNCMObj.redBaseCalcIcmsSTVendaAtaSimpNacional.ToString();

                //Venda varejo para contribuinte
                cst = tibNCMObj.cstVendaVarejoCont;
                ViewBag.CstVendaVarCont = cst;
                ViewBag.CstVendaVarContDesc = (from a in db.CstIcmsGerais where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqIcmsVenVarCont = tibNCMObj.aliqIcmsVendaVarejoCont.ToString();
                ViewBag.AliqImcsStVenVarCont = tibNCMObj.aliqIcmsSTVendaVarejo_Cont.ToString();
                ViewBag.AliqRedBasCalcVenVarCont = tibNCMObj.redBaseCalcVendaVarejoCont.ToString();
                ViewBag.AliqRedBasCalcSTVenAtaSn = tibNCMObj.RedBaseCalcSTVendaVarejo_Cont.ToString();

                //Venda varejo para consumidor final
                cst = tibNCMObj.cstVendaVarejoCont;
                ViewBag.CstVendaVarCf = cst;
                ViewBag.CstVendaVarCfDesc = (from a in db.CstIcmsGerais where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqIcmsVenVarCf = tibNCMObj.aliqIcmsVendaVarejoConsFinal.ToString();
                ViewBag.AliqImcsStVenVarCf = tibNCMObj.aliqIcmsSTVendaVarejoConsFinal.ToString();
                ViewBag.AliqRedBasCalcVenVarCf = tibNCMObj.redBaseCalcIcmsVendaVarejoConsFinal.ToString();
                ViewBag.AliqRedBasCalcSTVenAtaCf = tibNCMObj.redBaseCalcIcmsSTVendaVarejoConsFinal.ToString();

                //compra de inústria
                cst = tibNCMObj.cstCompraDeInd;
                ViewBag.CstCompraInd = cst;
                ViewBag.CstCompraIndDesc = (from a in db.CstIcmsGerais where a.codigo == cst select a.descricao).FirstOrDefault();
                ViewBag.AliqIcmsCompraInd = tibNCMObj.aliqIcmsCompDeInd.ToString();
                ViewBag.AliqImcsStCompraInd = tibNCMObj.aliqIcmsSTCompDeInd.ToString();
                ViewBag.AliqRedBasCalcCompraInd = tibNCMObj.redBaseCalcIcmsCompraDeInd.ToString();
                ViewBag.AliqRedBasCalcSTCompraInd = tibNCMObj.redBaseCalcIcmsSTCompraDeInd.ToString();



            }


            //pega os ncm iguais
            this.lstCli = this.lstCli.Where(item => item.NCM_PRODUTO == ncmAlterar).OrderBy(item => item.DESCRICAO_PRODUTO);
                        

            ViewBag.TributacaoNCM = this.lstCli;
           


            ViewBag.TributacaoNCM = this.tribNCM;
            ViewBag.TributacaoMTX = this.lstCli;
            return View();
        }



        public ActionResult EditMassaModalPost(string strDados, string ncm, string cest)
        {
            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;
            
            string retorno = "";
                     

            //varivael para recebe o novo ncm
            string ncmMudar = "";
            string cestMudar = "";

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            ncmMudar = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços
            cestMudar = cest != "" ? cest.Trim() : null;
            if(ncmMudar != null)
            {
                if(ncmMudar != "")
                {
                    ncmMudar = ncmMudar.Replace(".", ""); //tirar os pontos da string
                }
               
            }
          

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                if (prod !=null)
                {
                    
                    if(cestMudar != null)
                    {
                        if(cestMudar != "") 
                        {
                            prod.cest = cestMudar;
                        }
                       
                    }
                    //verificar se veio nulo
                    if(ncmMudar != null)
                    {
                        if(ncmMudar != "")
                        {
                            if (prod.ncm != ncmMudar)
                            {
                                prod.ncm = ncmMudar;
                            }
                        }
                       
                    }
                    
                    prod.auditadoNCM = 1;
                    prod.dataAlt = DateTime.Now; //data da alteração
                    try
                    {
                        db.SaveChanges();
                        regSalvos++;
                    }
                    catch(Exception e)
                    {
                        string ex = e.ToString();
                        regNSalvos++;
                    }
                   
                }
                
            }
            if(regSalvos > 0)
            {
                retorno = "Registro Salvo com Sucesso!!";
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
            }
            else
            {
                retorno = "Nenhum item do  registro alterado";
                //Redirecionar para registros
                return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
            }

            //Redirecionar para registros
            return RedirectToAction("EditMassa", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }


        //Editar CEST
        [HttpGet]
        public ActionResult EditCestMassa(string opcao, string ordenacao, string procurarPor, string procurarPorCest, string filtroCorrente, string filtroCorrenteCest, int? page, int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);



            //ViewBag para número de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //Ordenação
            ViewBag.Ordenacao = ordenacao;
            ViewBag.ParametroProduto = String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : ""; //Se nao vier nula a ordenacao aplicar por produto decrescente


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            if (opcao != null)
            {
                TempData["opcao"] = opcao;
            }
            else
            {
                opcao = TempData["opcao"].ToString();
            }

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");


            if (procurarPor != null || procurarPorCest != null)
            {
                page = 1;
            }
            else
            {
                procurarPor = filtroCorrente;
                procurarPorCest = filtroCorrenteCest;
            }

            //Atribui o filtro corrente
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrente2 = procurarPorCest;

            /*Para tipar */
            var prod1 = from s in db.Produtos select s; //variavel carregado de produtos

            if (opcao == "Com Cest")
            {
                prod1 = prod1.Where(s => s.cest != null);

                if (!String.IsNullOrEmpty(procurarPor))
                {
                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().StartsWith(codBarrasL.ToString())) : prod1 = prod1.Where(s => s.descricao.ToString().ToUpper().StartsWith(procurarPor.ToUpper()));

                }
                if (!String.IsNullOrEmpty(procurarPorCest))
                {
                    prod1 = prod1.Where(s => s.cest.Contains(procurarPorCest));

                }

            }
            else
            {
                prod1 = prod1.Where(s => s.cest == null);


                //ViewBag.NCMTipado = prod1;
                if (!String.IsNullOrEmpty(procurarPor))
                {
                    prod1 = (codBarrasL != 0) ? prod1.Where(s => s.codBarras.ToString().StartsWith(codBarrasL.ToString())) : prod1 = prod1.Where(s => s.descricao.ToString().ToUpper().StartsWith(procurarPor.ToUpper()));

                }
                if (!String.IsNullOrEmpty(procurarPorCest))
                {
                    prod1 = prod1.Where(s => s.cest.Contains(procurarPorCest));

                }

            }

            //Aplicar ordenação
            switch (ordenacao)
            {
                case "Produto_desc":
                    prod1 = prod1.OrderByDescending(s => s.descricao);
                    break;
                default:
                    prod1 = prod1.OrderBy(s => s.Id);
                    break;


            }
            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);


            int numeroPagina = (page ?? 1);

            return View(prod1.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }

        [HttpGet]
        public ActionResult EditCestMassaModal(string array) 
        {
            string[] dadosDoCadastro = array.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            return View();
        }
               

        [HttpGet]
        public ActionResult EditCestMassaModalPost(string strDados, string cest)
        {

            
            //varivael para recebe o novo cest
            string cestMudar = "";

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            cestMudar = cest != "" ? cest.Trim() : null; //ternario para remover eventuais espaços

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                prod.dataAlt = DateTime.Now; //data da alteração
                prod.cest = cestMudar; //novo ceste
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAnaliseProdutos", "Produto");


           
        }

        //Alterar Codigo de Barras
        [HttpGet]
        public ActionResult EditCodBarrasMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraCate,
            string filtroCate,
            string procuraNCM,
            string procuraCEST,
            string procurarPor, 
            string filtroCorrente,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtraPor,
            string filtroFiltraPor,
            int? page,
            int? numeroLinhas)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            //variavel auxiliar
            string resultado = param;
            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;
            TempData["opcao"] = (opcao != null) ? opcao : TempData["opcao"];

            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraCate != null)  ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }


            VerificaTempDataProd();

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com Cod. Barras":
                    this.prodMTX = this.prodMTX.Where(s => s.codBarras.ToString() != "0").ToList();
                    break;
                case "Sem Cod. Barras":
                    this.prodMTX = this.prodMTX.Where(s => s.codBarras.ToString() == "0").ToList();
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.prodMTX = ProcurarPorII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraCate, prodMTX);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.prodMTX = this.prodMTX.OrderByDescending(s => s.descricao).ToList();
                    break;
                case "Produto_asc":
                    this.prodMTX = this.prodMTX.OrderBy(s => s.descricao).ToList();
                    break;
                case "Id_desc":
                    this.prodMTX = this.prodMTX.OrderBy(s => s.Id).ToList();
                    break;
                default:
                    this.prodMTX = this.prodMTX.OrderBy(s => s.descricao).ToList();
                    break;


            }


            //montar a pagina
            int tamanhoPagina = 0;

            //Ternario para tamanho da pagina
            tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);



            int numeroPagina = (page ?? 1);
            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();


            return View(prodMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }


        [HttpGet]
        public ActionResult EditCodBarrasMassaModal(string strDados)
        {
            string[] dadosDoCadastro = strDados.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            prod = new List<Produto>();

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                prod.Add(db.Produtos.Find(aux));


            }
            ViewBag.Produtos = prod;

            return View();
        }

        [HttpGet]
        public ActionResult EditCodBarrasMassaModalPost(string strDados, string codBarras)
        {
            //varivael para recebe o novo ncm
            string codBarrasMudar = "";
            //separar a String em um array
            string[] idProdutos = strDados.Split(',');
            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();
            codBarrasMudar = codBarras != "" ? codBarras.Trim() : "0"; //ternario para remover eventuais espaços

            //objeto produto
            Produto prod = new Produto();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idProdutos.Length; i++)
            {
                int idProd = Int32.Parse(idProdutos[i]);
                prod = db.Produtos.Find(idProd);
                prod.dataAlt = DateTime.Now; //data da alteração
                prod.codBarras = Int64.Parse(codBarrasMudar);
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAnaliseProdutos", "Produto");

        }


       
        //Actions auxiliar
        public EmptyResult VerificaTempDataProd()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoProdMTX"] == null)
            {
                //this.prodMTX = (from a in db.Produtos where a.Id.ToString() != null select a).ToList();
                this.prodMTX = db.Produtos.ToList();
               
                TempData["tributacaoProdMTX"] = this.prodMTX; //cria a temp data e popula
                TempData.Keep("tributacaoProdMTX"); //persiste
            }
            else
            {
                this.prodMTX = (List<Produto>)TempData["tributacaoProdMTX"];//atribui a lista os valores de tempdata
                TempData.Keep("tributacaoProdMTX"); //persiste
            }

            return new EmptyResult();
        }
        public EmptyResult VerificaTempData()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTX"] == null)
            {
                ////this.tribMTX = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a).ToList();
                //this.tribMTX = db.Tributacao_GeralView.AsNoTracking().ToList();
                //TempData["tributacaoMTX"] = this.tribMTX; //cria a temp data e popula
                //TempData.Keep("tributacaoMTX"); //persiste
                this.lstCli = from a in db.Tributacao_GeralView select a;
                TempData["tributacaoMTX"] = this.lstCli; //cria a temp data e popula
                TempData.Keep("tributacaoMTX"); //persiste
            }
            else
            {
                //this.tribMTX = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                //TempData.Keep("tributacaoMTX"); //persiste
                this.lstCli = (IQueryable<TributacaoGeralView>)TempData["tributacaoMTX"];
                TempData.Keep("tributacaoMTX"); //persiste
            }

            return new EmptyResult();
        }


        private EmptyResult VerificaOriDest(string origem, string destino)
        {

            if (origem == null || origem == "")
            {
                TempData["UfOrigem"] = (TempData["UfOrigem"] == null) ? "TO" : TempData["UfOrigem"].ToString();
                TempData.Keep("UfOrigem");
            }
            else
            {
                TempData["UfOrigem"] = origem;
                TempData.Keep("UfOrigem");

            }

            if (destino == null || destino == "")
            {
                TempData["UfDestino"] = (TempData["UfDestino"] == null) ? "TO" : TempData["UfDestino"].ToString();
                TempData.Keep("UfDestino");
            }
            else
            {
                TempData["UfDestino"] = destino;
                TempData.Keep("UfDestino");
            }





            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            return new EmptyResult();
        }
        //actions auxiliares // ponto de ajuste: busca por aliquota
        private List<TributacaoGeralView> ProcurarPor(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, List<TributacaoGeralView> tribMTX)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                tribMTX = (codBarrasL != 0) ? (tribMTX.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))).ToList() : tribMTX = (tribMTX.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tribMTX = tribMTX.Where(s => s.CEST_PRODUTO == procuraCEST).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                tribMTX = tribMTX.Where(s => s.NCM_PRODUTO == procuraNCM).ToList();

            }

            return tribMTX;
        }




        private IQueryable<TributacaoGeralView> ProcurarPorIII(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, IQueryable<TributacaoGeralView> lstCli)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                lstCli = (codBarrasL != 0) ? (lstCli.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))) : lstCli = (lstCli.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                lstCli = lstCli.Where(s => s.CEST_PRODUTO == procuraCEST);
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                lstCli = lstCli.Where(s => s.NCM_PRODUTO == procuraNCM);

            }

            return lstCli;
        }



        private List<Produto> ProcurarPorII(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, string procuraCate, List<Produto>prodMTX)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.prodMTX = (codBarrasL != 0) ? (prodMTX.Where(s => s.codBarras.ToString().StartsWith(codBarrasL.ToString()))).ToList() : prodMTX = (prodMTX.Where(s => s.descricao.ToString().ToUpper().StartsWith(procurarPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                this.prodMTX = prodMTX.Where(s => s.cest == procuraCEST).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                this.prodMTX = prodMTX.Where(s => s.ncm == procuraNCM).ToList();

            }

            
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.prodMTX = this.prodMTX.Where(s => s.idCategoria.ToString() == procuraCate).ToList();


            }

            return this.prodMTX;
        }

        //verifica origem e destino so do ncm
        private EmptyResult VerificaOriDestNCM(string origem, string destino)
        {

            if (origem == null || origem == "")
            {
                TempData["UfOrigemNCM"] = (TempData["UfOrigemNCM"] == null) ? "TO" : TempData["UfOrigemNCM"].ToString();
                TempData.Keep("UfOrigemNCM");
            }
            else
            {
                TempData["UfOrigemNCM"] = origem;
                TempData.Keep("UfOrigemNCM");

            }

            if (destino == null || destino == "")
            {
                TempData["UfDestinoNCM"] = (TempData["UfDestinoNCM"] == null) ? "TO" : TempData["UfDestinoNCM"].ToString();
                TempData.Keep("UfDestinoNCM");
            }
            else
            {
                TempData["UfDestinoNCM"] = destino;
                TempData.Keep("UfDestinoNCM");
            }

           


            this.ufOrigemNCM = TempData["UfOrigemNCM"].ToString();
            this.ufDestinoNCM = TempData["UfDestinoNCM"].ToString();

            return new EmptyResult();
        }
        public EmptyResult VerificarLinhas(int? numeroLinhas)
        {
            if (numeroLinhas != null)
            {

                if (TempData["linhas"] != null)
                {
                    if (!numeroLinhas.Equals(int.Parse(TempData["linhas"].ToString())))
                    {
                        TempData["linhas"] = numeroLinhas;
                        TempData.Keep("linhas");
                        ViewBag.NumeroLinhas = numeroLinhas;
                    }
                    else
                    {
                        ViewBag.NumeroLinhas = numeroLinhas;
                        TempData.Keep("linhas");
                    }

                }
                else
                {
                    TempData["linhas"] = numeroLinhas;
                    TempData.Keep("linhas");
                    ViewBag.NumeroLinhas = numeroLinhas;

                }


            }
            else
            {
                if (TempData["linhas"] == null)
                {
                    TempData["linhas"] = 10;
                    TempData.Keep("linhas");
                    ViewBag.NumeroLinhas = 10;
                }
                else
                {
                    ViewBag.NumeroLinhas = TempData["linhas"];
                    TempData.Keep("linhas");
                }
            }
            return new EmptyResult();
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


    }
}