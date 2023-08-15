using MatrizTributaria.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class TribProdController : Controller
    {

        //Objego context
        readonly MatrizDbContext db = new MatrizDbContext();

        //clase util
        UtilController util = new UtilController();

        //origem e destino
        string ufOrigem = "";
        string ufDestino = "";

        //LISTA a view do banco
        IQueryable<TribProdView> TribProd_View;

        // GET: TribProd
        public ActionResult TribProd(
            string origem, 
            string destino, 
            string opcao, 
            string param, 
            string ordenacao, 
            string qtdSalvos, 
            string qtdNSalvos, 
            string procuraNCM, 
            string procuraCEST,
            string procurarPor, 
            string filtroCorrente, 
            string procuraSetor, 
            string filtroSetor, 
            string procuraCate, 
            string filtroCate, 
            string filtroCorrenteNCM,
            string filtroCorrenteCEST, 
            int? page, 
            int? numeroLinhas, 
            string filtroNulo, 
            string auditadosNCM, 
            string filtraPor, 
            string filtroFiltraPor,
            string filtroCorrenteAudNCM, 
            string crt, 
            string regime)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }
            ViewBag.TituloView = "TributacaoComNCM"; //titulo da página
            procuraCate = procuraCate == "null" ? null : procuraCate;

            //verificar se a tributação escolhida está ativa
            VerificaTributacao(crt, regime);

            //ViewBags para o regime e o crt
            ViewBag.Crt = TempData["crt"].ToString();
            ViewBag.Regime = TempData["regime"].ToString();

            //se nao vier nulo é pq houve pesquisa
            if (procurarPor != null)
            {
                TempData["procuraPor"] = procurarPor;

            }
            else
            {
                if (filtroCorrente != null)
                {
                    TempData["procuraPor"] = filtroCorrente;
                }
            }


            //se os dois estao nulos e o tempdata tem alguma coisa entao o friltro corrente deve receber tempdat
            if (procurarPor == null && filtroCorrente == null)
            {
                if (TempData["procuraPor"] != null)
                {
                    filtroCorrente = TempData["procuraPor"].ToString();
                }

            }



            //se nao vier nulo é pq houve pesquisa
            if (procuraNCM != null)
            {
                TempData["procuraPorNCM"] = procuraNCM;

            }
            else
            {
                if (filtroCorrenteNCM != null)
                {
                    TempData["procuraPorNCM"] = filtroCorrenteNCM;
                }
            }

            //se os dois estao nulos e o tempdata tem alguma coisa entao o friltro corrente deve receber tempdat
            if (procuraNCM == null && filtroCorrenteNCM == null)
            {
                if (TempData["procuraPorNCM"] != null)
                {
                    filtroCorrenteNCM = TempData["procuraPorNCM"].ToString();
                }

            }
            TempData.Keep("procuraPorNCM");
         


            //variavel auxiliar
            string resultado = param;


            //Auxilia na conversão para fazer a busca pelo codigo de barras
            /*A variavel codBarras vai receber o parametro de acordo com a ocorrencia, se o filtrocorrente estiver valorado
             ele será atribuido, caso contrario será o valor da variavel procurar por*/
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procurarPor;

            //converte em long caso seja possivel
            long codBarrasL = 0;
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            TempData.Keep("procuraPor");



            //filtro por categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria
            ViewBag.FiltrarPor = "Categoria";
            if (procuraCate != null)
            {
                TempData["procuraCAT"] = procuraCate;
                TempData.Keep("procuraCAT");
            }

            if (TempData["procuraCAT"] != null)
            {
                ViewBag.FiltroCorrenteCate = TempData["procuraCAT"].ToString();
            }


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;


            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

            auditadosNCM = (auditadosNCM == null) ? filtroCorrenteAudNCM : auditadosNCM; //todos os que não foram auditados


            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;
            ViewBag.FiltroCorrenteAuditado = (auditadosNCM != null) ? auditadosNCM : "0";

            ViewBag.FiltroFiltraPor = filtraPor;

            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            //criar o temp data da lista ou recupera-lo
            VerificaTempDataNCM(ViewBag.Regime, ViewBag.Crt);

            //montar select estado origem e destino: FAZER DINSTINC
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //Aplica a origem e destino selecionada a lista
            this.TribProd_View = this.TribProd_View.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.TribProd_View = this.TribProd_View.Where(s => s.AUDITADO == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.TribProd_View = this.TribProd_View.Where(s => s.AUDITADO == 1);
                    break;
                case "2": //TODOS
                    this.TribProd_View = this.TribProd_View.Where(s => s.ID != 0);
                    break;
            }

            //Procura, caso venha dados nos filtros
            this.TribProd_View = ProcurarPorNCM_CEST_PARA_TRIB_PROD(codBarrasL, procurarPor, procuraCEST, procuraNCM, TribProd_View);

            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.TribProd_View = this.TribProd_View.Where(s => s.ID_CATEGORIA_PRODUTO.ToString() == procuraCate);


            }

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.TribProd_View = this.TribProd_View.OrderByDescending(s => s.NCM_PRODUTO);
                    break;
                case "Produto_asc":
                    this.TribProd_View = this.TribProd_View.OrderBy(s => s.NCM_PRODUTO);
                    break;
                case "Id_desc":
                    this.TribProd_View = this.TribProd_View.OrderBy(s => s.ID);
                    break;
                default:
                    this.TribProd_View = this.TribProd_View.OrderBy(s => s.NCM_PRODUTO);
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


            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo).ToList();
            ViewBag.Opcao = "Com aliquota"; //sempre mostrar o campo de busca por aliquota



            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            ViewBag.CstPisCofins = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.descricao);//para montar a descrição da cst na view
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.descricao); //para montar a descrição da cst na view

            return View(this.TribProd_View.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

           
        }


        //Verifica a tributação e regime
        public EmptyResult VerificaTempDataNCM(string regime, string crt)
        {
            //vai filtrar pelo regime e pelo crt

            int regimeTrib = int.Parse(regime);
            int crtTrib = int.Parse(crt);

            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novamente*/
            if (TempData["tributacao_TRIB_PROD_VIEW"] == null)
            {
                //this.tributacaoMTX_NCMView = (IQueryable<TributacaoNCMView>)db.TributacoesNcmView.Where(a => a.CRT == crtTrib && a.REGIME_TRIBUTARIO == regimeTrib).ToList();
                this.TribProd_View = from a in db.TribProdViews where a.CRT == crtTrib && a.REGIME_TRIB == regimeTrib select a; //FILTRA O QUE FOR SIMPLES NACIONAL


                TempData["tributacao_TRIB_PROD_VIEW"] = this.TribProd_View; //cria a temp data e popula
                TempData.Keep("tributacao_TRIB_PROD_VIEW"); //persiste

            }
            else
            {

                this.TribProd_View = (IQueryable<TribProdView>)TempData["tributacao_TRIB_PROD_VIEW"];
                TempData.Keep("tributacao_TRIB_PROD_VIEW"); //persiste
            }



            return new EmptyResult();
        }

        private IQueryable<TribProdView> ProcurarPorNCM_CEST_PARA_TRIB_PROD(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, IQueryable<TribProdView> TribProd_View)
        {

            if (!String.IsNullOrEmpty(procurarPor))
            {
                TribProd_View = (codBarrasL != 0) ? (TribProd_View.Where(s => s.CODIGO_BARRAS.ToString().StartsWith(codBarrasL.ToString()))) : TribProd_View = (TribProd_View.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }



            if (!String.IsNullOrEmpty(procuraCEST))
            {
                TribProd_View = TribProd_View.Where(s => s.CEST_PRODUTO.ToString().StartsWith(procuraCEST.ToString()));
            }


            if (procuraNCM != null)
            {
                if (procuraNCM != "")
                {
                    procuraNCM = procuraNCM != "" ? procuraNCM.Trim() : null; //ternario para remover eventuais espaços
                    procuraNCM = procuraNCM.Replace(".", ""); //tirar os pontos da string
                }

            }

            if (!String.IsNullOrEmpty(procuraNCM))
            {
                TribProd_View = TribProd_View.Where(s => s.NCM_PRODUTO.ToString().StartsWith(procuraNCM.ToString()));

            }

            return TribProd_View;

        }

        //ORIGEM E DESTINO
        public EmptyResult VerificaOriDest(string origem, string destino)
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

        public EmptyResult VerificaTributacao(string crt, string regime)
        {

            //crt
            if (crt == null || crt == "")
            {
                //vai atribuir o valor 1 caso nao venha nada na variavel (1 = REGIME NORMAL)
                TempData["crt"] = (TempData["crt"] == null) ? "3" : TempData["crt"].ToString();
                TempData.Keep("crt");
            }
            else
            {
                if (TempData["crt"].ToString() != crt) //se o valor da temp data for diferente da variavel ele vai zerar uma outra temp_data
                {
                    //quer dizer que mudou, logo devemos zerar a tempdata da tributação
                    TempData["tributacao_TRIB_PROD_VIEW"] = null;
                    TempData.Keep("tributacao_TRIB_PROD_VIEW");
                }
                TempData["crt"] = (TempData["crt"] == null) ? "3" : crt;
                TempData.Keep("crt");
            }

            //regime
            if (regime == null || regime == "")
            {
                //vai atribuir o valor 2 caso nao venha nada na variavel (2 = LUCRO REAL)
                TempData["regime"] = (TempData["regime"] == null) ? "2" : TempData["regime"].ToString();
                TempData.Keep("regime");
            }
            else
            {
                if (TempData["regime"].ToString() != regime) //se o valor da temp data for diferente da variavel ele vai zerar uma outra temp_data
                {
                    TempData["tributacao_TRIB_PROD_VIEW"] = null;
                    TempData.Keep("tributacao_TRIB_PROD_VIEW");
                }
                TempData["regime"] = (TempData["regime"] == null) ? "2" : regime;
                TempData.Keep("regime");
            }

            return new EmptyResult();
        }
    }
}