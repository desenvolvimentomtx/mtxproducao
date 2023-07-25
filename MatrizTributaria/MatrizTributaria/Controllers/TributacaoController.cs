using MatrizTributaria.Models;
using MatrizTributaria.Models.ViewModels;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;

namespace MatrizTributaria.Controllers
{
    public class TributacaoController : Controller
    {
        //Objego context
        readonly MatrizDbContext db = new MatrizDbContext();
        List<Produto> prod;
        List<TributacaoNCM> tribNCM;
        List<Tributacao> trib;
        List<Tributacao> tributacao = new List<Tributacao>();
        IQueryable<TributacaoGeralView> lstCli;
        IQueryable<TtributacaoGeralViewSN> lstCliSN;
        List<TributacaoGeralView> tribMTX = new List<TributacaoGeralView>();
       

        IQueryable<TributacaoNCMView> tribMTX_NCMView;

        IQueryable<TributacaoNCMView> tribPorUF;
        IQueryable<TributacaoNCM> tribMTX_NCM;
        //origem e destino
        string ufOrigem = "";
        string ufDestino = "";

        // GET: Tributacao - ANTERIOR A 27122021
        //public ActionResult Index(string sortOrder, string searchString, string currentFilter,  int? page)
        //{

        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("../Home/Login");
        //    }
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";


        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }
        //    ViewBag.CurrentFilter = searchString;

        //    //var tributacao = from s in db.Tributacoes select s;
        //    /*PAra tipar */
        //    /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
        //     na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
        //    //if (TempData["tributacaoMTX"] == null)
        //    //{
        //    //    //this.lstCli = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a);
        //    //    this.lstCli = db.Tributacao_GeralView;
        //    //    TempData["tributacaoMTX"] = this.lstCli; //cria a temp data e popula
        //    //    TempData.Keep("tributacaoMTX"); //persiste
        //    //}
        //    //else
        //    //{
        //    //    this.lstCli = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
        //    //    TempData.Keep("tributacaoMTX"); //persiste
        //    //}
        //    VerificaTempData();
        //    //lista de produtos
        //    //var listaProdutos = db.Produtos;
        //    //int paginaTamanho = 4;
        //    //int paginaNumero = 1;

        //    //Viewbag da lista de produtos
        //    //ViewBag.Produtos = listaProdutos.ToPagedList(paginaNumero, paginaTamanho);

        //    //var produtos = from s in db.Produtos select s; //variavel carregado de produtos
        //    //ViewBag.ddlProdutos = new SelectList(produtos, "ProdutoId", "NomeDoProduto");//lista

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        this.lstCli = this.lstCli.Where(s => s.DESCRICAO_PRODUTO.ToUpper().Contains(searchString.ToUpper()) || s.ID.ToString().ToUpper().Contains(searchString.ToUpper()));

        //    }

        //    switch (ViewBag.ProdutoParam)
        //    {
        //        case "Produto_desc":
        //            this.lstCli = this.lstCli.OrderBy(s => s.ID); //ordenar pelo id do registro da tabela
        //            break;
        //        case "NatRec":
        //            this.lstCli = this.lstCli.OrderBy(s => s.COD_NAT_RECEITA);
        //            break;
        //        case "NatRec_desc":
        //            this.lstCli = this.lstCli.OrderByDescending(s => s.COD_NAT_RECEITA);
        //            break;
        //        default:
        //            this.lstCli = this.lstCli.OrderByDescending(s => s.ID);
        //            break;
        //    }
        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);



        //    return View(this.lstCli.ToPagedList(pageNumber, pageSize));
        //}

        //detalhe
        public ActionResult Index(string origem, string destino, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM,
            string procuraCEST, string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate,
            string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtronulo, 
            string auditadosNCM, string filtraPor, string filtroCorrenteAudNCM, string procurarPorAliq, string filtroCorrenteAliq,
            string procuraCST, string filtraCST)
        {

            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");

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
            auditadosNCM = (auditadosNCM != null) ? auditadosNCM : "2";
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;
            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;
            ViewBag.FiltroCorrenteAuditado = auditadosNCM;
            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;
            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            VerificaTempData();

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //Aplica a origem e destino selecionada
            this.lstCli = this.lstCli.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

            //buscar os auditados
            switch (auditadosNCM)
            {
                case "0"://somente os não auditados
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 0);
                    break;
                case "1": //somente os auditados
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 1);
                    break;
                case "2": //todos
                    this.lstCli = this.lstCli.Where(s => s.ID !=null);
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);
            ViewBag.Opcao = "Com aliquota"; //sempre mostrar o campo de busca por aliquota

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
           
        }

        public ActionResult TributacaoNcm(string origem, string destino, string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
            string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
            string filtroCorrenteAudNCM, string tributacao)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //verficia se é simples nacional
            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa
            ViewBag.Tributacao = TempData["tributacao"].ToString();

            //se for simples nacional direciona para essa action
            if (ViewBag.Tributacao == "SIMPLES")
            {

                return RedirectToAction("TributacaoNcmSN", new {tributacao = tributacao, origem = origem, destino = destino });
            }


            //se nao vier nulo é pq houve pesquisa
            if (procurarPor != null)
            {
                TempData["procuraPor"] = procurarPor;

            }
            else
            {
                if(filtroCorrente != null)
                {
                    TempData["procuraPor"] = filtroCorrente;
                }
            }
            //se os dois estao nulos e o tempdata tem alguma coisa entao o friltro corrente deve receber tempdat
            if(procurarPor == null && filtroCorrente == null)
            {
                if(TempData["procuraPor"] != null)
                {
                    filtroCorrente = TempData["procuraPor"].ToString();
                }
               
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

                       

            TempData.Keep("procuraPor");


            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //auditadosNCM = (auditadosNCM != null) ? auditadosNCM : "0";



            filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }




            if (procuraCate == null || procuraCate == "" || procuraCate == "null")
            {
                if (TempData["procuraCAT"] != null)
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
                if (TempData["procuraCAT"] != null)
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
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;

           



            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;




            ///*Verifica a opção e atribui a uma tempdata para continuar salva*/
            //TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
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
            ViewBag.FiltroCorrenteAuditado = (auditadosNCM != null) ? auditadosNCM : "0";
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


            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //Aplica a origem e destino selecionada
            this.lstCli = this.lstCli.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.lstCli = this.lstCli.Where(s => s.AUDITADO_POR_NCM == 1);
                    break;
                case "2": //TODOS
                    this.lstCli = this.lstCli.Where(s => s.ID != null);
                    break;
            }


            //switch (opcao)
            //{
            //    case "Com NCM":
            //        //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
            //        ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //COM NCM
            //        //Switche do filtro
            //        switch (ViewBag.Filtro)
            //        {
            //            case "1":
            //                this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO != null);
            //                break;
            //            case "2":
            //                this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == null);
            //                break;
            //        }
            //        break;
            //    case "Sem NCM":
            //        ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //SEM NCM
            //        switch (ViewBag.Filtro)
            //        {
            //            case "1":
            //                this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO != null);
            //                break;
            //            case "2":
            //                this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == null);
            //                break;
            //        }
            //        break;
            //}

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            //  this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);
            this.lstCli = ProcurarPorV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

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
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo).ToList();
            ViewBag.Opcao = "Com aliquota"; //sempre mostrar o campo de busca por aliquota


            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            ViewBag.CstPisCofins = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
           
        }


        //PARA SIMPLES NACIONAL
        public ActionResult TributacaoNcmSN(string origem, string destino, string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
          string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
          string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
          string filtroCorrenteAudNCM, string tributacao)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }


            //verficia se é simples nacional
            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa
            ViewBag.Tributacao = TempData["tributacao"].ToString();

            //se for simples nacional direciona para essa action
            if (ViewBag.Tributacao == "OUTROS")
            {

                return RedirectToAction("TributacaoNcm");
            }


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


            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //auditadosNCM = (auditadosNCM != null) ? auditadosNCM : "0";



            filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }




            if (procuraCate == null || procuraCate == "" || procuraCate == "null")
            {
                if (TempData["procuraCAT"] != null)
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
                if (TempData["procuraCAT"] != null)
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
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;





            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;




            ///*Verifica a opção e atribui a uma tempdata para continuar salva*/
            //TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
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
            ViewBag.FiltroCorrenteAuditado = (auditadosNCM != null) ? auditadosNCM : "0";
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

            //verifica carregamento da tabela
            VerificaTempDataSN();


            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //Aplica a origem e destino selecionada
            this.lstCliSN = this.lstCliSN.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

         

            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.lstCliSN = this.lstCliSN.Where(s => s.AUDITADO_TRIB_POR_NCM == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.lstCliSN = this.lstCliSN.Where(s => s.AUDITADO_TRIB_POR_NCM == 1);
                    break;
                case "2": //TODOS
                    this.lstCliSN = this.lstCliSN.Where(s => s.ID != null);
                    break;
            }


           
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            //  this.tribMTX = ProcurarPor(codBarrasL, procurarPor, procuraCEST, procuraNCM, tribMTX);
            this.lstCliSN = ProcurarPorSN(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCliSN);

            //verificar isso - setor categoria
            //Busca por setor
            if (!String.IsNullOrEmpty(procuraSetor))
            {
                this.lstCliSN = this.lstCliSN.Where(s => s.ID_SETOR.ToString() == procuraSetor);


            }
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.lstCliSN = this.lstCliSN.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.lstCliSN = this.lstCliSN.OrderByDescending(s => s.DESCRICAO_PRODUTO);
                    break;
                case "Produto_asc":
                    this.lstCliSN = this.lstCliSN.OrderBy(s => s.DESCRICAO_PRODUTO);
                    break;
                case "Id_desc":
                    this.lstCliSN = this.lstCliSN.OrderBy(s => s.ID);
                    break;
                default:
                    this.lstCliSN = this.lstCliSN.OrderBy(s => s.DESCRICAO_PRODUTO);
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
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo).ToList();
            ViewBag.Opcao = "Com aliquota"; //sempre mostrar o campo de busca por aliquota



            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao).ToList();
            ViewBag.CstPisCofins = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view

            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(this.lstCliSN.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }





        /*Backup da action de tributacaoComNCM 23122022*/

        //public ActionResult TributacaoComNCM(string origem, string destino, string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
        //    string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
        //    string filtroCorrenteAudNCM)
        //{
        //    /*Verificar a sessão*/
        //    if (Session["usuario"] == null)
        //    {
        //        return RedirectToAction("../Home/Login");

        //    }

        //    //se nao vier nulo é pq houve pesquisa
        //    if (procuraNCM != null)
        //    {
        //        TempData["procuraPorNCM"] = procuraNCM;

        //    }
        //    else
        //    {
        //        if (filtroCorrenteNCM != null)
        //        {
        //            TempData["procuraPorNCM"] = filtroCorrenteNCM;
        //        }
        //    }
        //    //se os dois estao nulos e o tempdata tem alguma coisa entao o friltro corrente deve receber tempdat
        //    if (procuraNCM == null && filtroCorrenteNCM == null)
        //    {
        //        if (TempData["procuraPorNCM"] != null)
        //        {
        //            filtroCorrenteNCM = TempData["procuraPorNCM"].ToString();
        //        }

        //    }

        //    //variavel auxiliar
        //    string resultado = param;

        //    TempData.Keep("procuraPorNCM");


        //    filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria



        //    ViewBag.FiltrarPor = "Categoria";


        //    if (procuraCate == null || procuraCate == "" || procuraCate == "null")
        //    {
        //        if (TempData["procuraCAT"] != null)
        //        {
        //            procuraCate = TempData["procuraCAT"].ToString();
        //        }
        //        else
        //        {
        //            procuraCate = null;
        //            TempData["procuraCAT"] = null;
        //        }

        //    }
        //    else
        //    {
        //        if (TempData["procuraCAT"] != null)
        //        {
        //            if (procuraCate != (TempData["procuraCAT"].ToString()))
        //            {
        //                TempData["procuraCAT"] = procuraCate;
        //            }


        //        }
        //        else
        //        {
        //            TempData["procuraCAT"] = procuraCate;
        //        }



        //    }



        //    //numero de linhas
        //    ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;


        //    ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
        //    ViewBag.ParametroProduto = ordenacao;


        //    ///*Verifica a opção e atribui a uma tempdata para continuar salva*/
        //    //TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
        //    //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


        //    //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
        //    //TempData.Keep("opcao");
        //    TempData.Keep("procuraCAT");

        //    //atribui 1 a pagina caso os parametros nao sejam nulos
        //    page = (procuraCEST != null) || (procuraNCM != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

        //    //atrbui filtro corrente caso alguma procura esteja nulla

        //    procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
        //    procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

        //    auditadosNCM = (auditadosNCM == null) ? filtroCorrenteAudNCM : auditadosNCM; //todos os que não foram auditados


        //    procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

        //    //View pag para filtros

        //    ViewBag.FiltroCorrenteNCM = procuraNCM;
        //    ViewBag.FiltroCorrenteCEST = procuraCEST;
        //    ViewBag.FiltroCorrenteAuditado = (auditadosNCM != null) ? auditadosNCM : "0";


        //    if (TempData["procuraCAT"] == null)
        //    {
        //        ViewBag.FiltroCorrenteCate = procuraCate;
        //    }
        //    else
        //    {
        //        ViewBag.FiltroCorrenteCate = TempData["procuraCAT"].ToString();
        //    }

        //    ViewBag.FiltroFiltraPor = filtraPor;


        //    if (procuraCate != null)
        //    {
        //        ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
        //    }

        //    //criar o temp data da lista ou recupera-lo
        //    VerificaTempDataNCM();


        //    //montar select estado origem e destino
        //    ViewBag.EstadosOrigem = db.Estados;
        //    ViewBag.EstadosDestinos = db.Estados;



        //    //verifica estados origem e destino
        //    VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


        //    //aplica estado origem e destino
        //    ViewBag.UfOrigem = this.ufOrigem;
        //    ViewBag.UfDestino = this.ufDestino;


        //    //Aplica a origem e destino selecionada
        //    this.tribMTX_NCM = this.tribMTX_NCM.Where(s => s.UF_Origem == this.ufOrigem && s.UF_Destino == this.ufDestino);

        //    switch (ViewBag.FiltroCorrenteAuditado)
        //    {
        //        case "0": //SOMENTE OS NÃO AUDITADOS
        //            this.tribMTX_NCM = this.tribMTX_NCM.Where(s => s.auditadoPorNCM == 0);
        //            break;
        //        case "1": //SOMENTE OS AUDITADOS
        //            this.tribMTX_NCM = this.tribMTX_NCM.Where(s => s.auditadoPorNCM == 1);
        //            break;
        //        case "2": //TODOS
        //            this.tribMTX_NCM = this.tribMTX_NCM.Where(s => s.id != 0);
        //            break;
        //    }

        //    this.tribMTX_NCM = ProcurarPorNCM_CEST(procuraCEST, procuraNCM, tribMTX_NCM);

        //    //verificar isso - setor categoria

        //    //Busca por categoria
        //    if (!String.IsNullOrEmpty(procuraCate))
        //    {
        //        this.tribMTX_NCM = this.tribMTX_NCM.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


        //    }
        //    switch (ordenacao)
        //    {
        //        case "Produto_desc":
        //            this.tribMTX_NCM = this.lstCli.OrderByDescending(s => s.DESCRICAO_PRODUTO);
        //            break;
        //        case "Produto_asc":
        //            this.lstCli = this.lstCli.OrderBy(s => s.DESCRICAO_PRODUTO);
        //            break;
        //        case "Id_desc":
        //            this.lstCli = this.lstCli.OrderBy(s => s.ID);
        //            break;
        //        default:
        //            this.lstCli = this.lstCli.OrderBy(s => s.DESCRICAO_PRODUTO);
        //            break;


        //    }


        //    //montar a pagina
        //    int tamanhoPagina = 0;

        //    //Ternario para tamanho da pagina
        //    tamanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);
        //    int numeroPagina = (page ?? 1);
        //    //Mensagens de retorno
        //    ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
        //    ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
        //    ViewBag.RegNSalvos = (qtdNSalvos != null) ? qtdNSalvos : "";


        //    return null;
        //}


        public ActionResult TributacaoComNCM(string origem, string destino, string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
            string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
            string filtroCorrenteAudNCM, string tributacao)
        {
            /*Verificar a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");

            }

            procuraCate = procuraCate == "null" ? null : procuraCate;

            ViewBag.TituloView = "TributacaoComNCM";
            //verficia se é simples nacional
            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa
            ViewBag.Tributacao = TempData["tributacao"].ToString();



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

            //variavel auxiliar
            string resultado = param;

            TempData.Keep("procuraPorNCM");


            filtraPor = (filtraPor != null) ? filtraPor : "Categoria"; //padrão é por categoria

           

              ViewBag.FiltrarPor = "Categoria";
           
            if(procuraCate != null)
            {
                TempData["procuraCAT"] = procuraCate;
                TempData.Keep("procuraCAT");
            }

            if(TempData["procuraCAT"] != null)
            {
                ViewBag.FiltroCorrenteCate = TempData["procuraCAT"].ToString();
            }

            //if (procuraCate == null || procuraCate == "" || procuraCate == "null")
            //{



            //    if (TempData["procuraCAT"] != null && TempData["procCAT_ANTERIOR"] != null)
            //    {
            //        procuraCate = TempData["procuraCAT"].ToString();
            //    }
            //    else
            //    {
            //        procuraCate = null;
            //        TempData["procuraCAT"] = null;
            //        TempData["procCAT_ANTERIOR"] = null;
            //    }

            //}
            //else
            //{
            //    if (TempData["procuraCAT"] != null)
            //    {
            //        if(TempData["procCAT_ANTERIOR"] != null)
            //        {
            //            if (procuraCate != (TempData["procCAT_ANTERIOR"].ToString()))
            //            {
            //                TempData["procuraCAT"] = procuraCate;
            //            }
            //        }

                  


            //    }
            //    else
            //    {
            //        TempData["procuraCAT"] = procuraCate;
            //    }



            //}

           

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;


            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            ///*Verifica a opção e atribui a uma tempdata para continuar salva*/
            //TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
            TempData.Keep("procuraCAT");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procuraCEST != null) || (procuraNCM != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
           
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;

            auditadosNCM = (auditadosNCM == null) ? filtroCorrenteAudNCM : auditadosNCM; //todos os que não foram auditados

            
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //View pag para filtros
          
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;
            ViewBag.FiltroCorrenteAuditado = (auditadosNCM != null) ? auditadosNCM : "0";


            //if (TempData["procuraCAT"] == null)
            //{
            //    ViewBag.FiltroCorrenteCate = procuraCate;
            //}
            //else
            //{
            //    ViewBag.FiltroCorrenteCate = TempData["procuraCAT"].ToString();
            //}

            //ViewBag.FiltroFiltraPor = filtraPor;


            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            //criar o temp data da lista ou recupera-lo
            VerificaTempDataNCM(ViewBag.Tributacao);


            //montar select estado origem e destino: FAZER DINSTINC
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //ViewBag.EstadosOriDistinc = db.TributacoesNcmView.Where(c => c.ID > 0).Select(c => c.UF_ORIGEM).GroupBy(c => c.UF_ORIGEM);
            //ViewBag.EstadosDestDistinc = this.tribMTX_NCMView.Where(c => c.ID > 0).Select(c => new { c.UF_DESTINO }).GroupBy(c => c.UF_DESTINO);

            //ViewBag.EstadosOriDistinc = this.tribMTX_NCMView.Where(c => c.ID > 0).Select(c => new { c.UF_ORIGEM }).GroupBy(c => c.UF_ORIGEM);
            //ViewBag.EstadosDestDistinc = this.tribMTX_NCMView.Where(c => c.ID > 0).Select(c => new { c.UF_DESTINO }).GroupBy(c => c.UF_DESTINO);
            //ViewBag.EstadosOriDistinc = new SelectList(this.tribMTX_NCMView.Where(c => c.ID > 0).Select(c => new {c.UF_ORIGEM}).GroupBy(c=>c.UF_ORIGEM));
            //ViewBag.EstadosDestDistinc = new SelectList(this.tribMTX_NCMView.Where(c => c.ID > 0).Select(c => new { c.UF_DESTINO }).GroupBy(c => c.UF_DESTINO));

            //ViewBag.companies = new SelectList(oc_db.company.Where(c => c.status == "ACTIVE")
            //                                    .Select(c => new { c.name_1 })
            //                                    .Distinct()
            //                                    .OrderBy(c => c.name_1)
            //                       , "name_1", "name_1");



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //Aplica a origem e destino selecionada
            this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.AUDITADONCM == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.AUDITADONCM == 1);
                    break;
                case "2": //TODOS
                    this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.ID != 0);
                    break;
            }

            this.tribMTX_NCMView = ProcurarPorNCM_CEST_PARA_NCM(procuraCEST, procuraNCM, tribMTX_NCMView);

            //verificar isso - setor categoria
           
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tribMTX_NCMView = this.tribMTX_NCMView.OrderByDescending(s => s.NCM);
                    break;
                case "Produto_asc":
                    this.tribMTX_NCMView = this.tribMTX_NCMView.OrderBy(s => s.NCM);
                    break;
                case "Id_desc":
                    this.tribMTX_NCMView = this.tribMTX_NCMView.OrderBy(s => s.ID);
                    break;
                default:
                    this.tribMTX_NCMView = this.tribMTX_NCMView.OrderBy(s => s.NCM);
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
            ViewBag.CstPisCofins = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.descricao); ; //para montar a descrição da cst na view

            //VerificaTributacoesPorUF(tributacao);

            /*Verificar se ha tributação por uf*/
            //ArrayList origemUF = new ArrayList();
            //ArrayList destinoUF = new ArrayList();

            //origemUF.Add("AC");
            //origemUF.Add("AL");
            //origemUF.Add("AP");
            //origemUF.Add("AM");
            //origemUF.Add("BA");
            //origemUF.Add("CE");
            //origemUF.Add("DF");
            //origemUF.Add("ES");
            //origemUF.Add("GO");
            //origemUF.Add("MA");
            //origemUF.Add("MT");
            //origemUF.Add("MS");
            //origemUF.Add("MG");
            //origemUF.Add("PA");
            //origemUF.Add("PB");
            //origemUF.Add("PR");
            //origemUF.Add("PE");
            //origemUF.Add("PI");
            //origemUF.Add("RJ");
            //origemUF.Add("RN");
            //origemUF.Add("RS");
            //origemUF.Add("RO");
            //origemUF.Add("RR");
            //origemUF.Add("SC");
            //origemUF.Add("SP");
            //origemUF.Add("SE");
            //origemUF.Add("TO");

            //destinoUF.Add("AC");
            //destinoUF.Add("AL");
            //destinoUF.Add("AP");
            //destinoUF.Add("AM");
            //destinoUF.Add("BA");
            //destinoUF.Add("CE");
            //destinoUF.Add("DF");
            //destinoUF.Add("ES");
            //destinoUF.Add("GO");
            //destinoUF.Add("MA");
            //destinoUF.Add("MT");
            //destinoUF.Add("MS");
            //destinoUF.Add("MG");
            //destinoUF.Add("PA");
            //destinoUF.Add("PB");
            //destinoUF.Add("PR");
            //destinoUF.Add("PE");
            //destinoUF.Add("PI");
            //destinoUF.Add("RJ");
            //destinoUF.Add("RN");
            //destinoUF.Add("RS");
            //destinoUF.Add("RO");
            //destinoUF.Add("RR");
            //destinoUF.Add("SC");
            //destinoUF.Add("SP");
            //destinoUF.Add("SE");
            //destinoUF.Add("TO");

            //int[,] ori_destino = new int[27, 27];

            //for (int i = 0; i <= 26; i++)
            //{
            //    for(int a=0; a<=26; a++)
            //    {
            //        string origA = origemUF[i].ToString();
            //        string destA = destinoUF[a].ToString();
            //        ori_destino[i, a] = db.TributacoesNcm.Count(b => b.UF_Origem.Equals(origA) && b.UF_Destino.Equals(destA));
            //        if(ori_destino[i,a] > 0)
            //        {
            //            ViewData["ori"+origA+"Dest"+destA] = ori_destino[i, a];
                       
            //        }
            //       //ViewBag.valor = this.tribPorUF.Count(b => b.UF_ORIGEM.Equals(origemUF[i].ToString()) && b.UF_DESTINO.Equals(destinoUF[a].ToString()));
            //    }
            //}




            //ViewBag.CstGeral = db.CstIcmsGerais.ToList(); //para montar a descrição da cst na view
            return View(this.tribMTX_NCMView.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


           
        }

        [HttpGet]
        public ActionResult TributacaoNcmEditMassaModal(string array)
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
        
        //PEGAR OS DADOS DA SELEÇÃO COM NCM
        [HttpGet]
        public ActionResult TributacaoNcmEditMassaModalComNCM(string array)
        {
            string[] dadosDoCadastro = array.Split(',');

            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento

            tribNCM = new List<TributacaoNCM>();

           

            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                tribNCM.Add(db.TributacoesNcm.Find(aux));


            }
            ViewBag.TribNCM = tribNCM;

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
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);

            return View();
        }
        [HttpGet]
        public ActionResult TributacaoNcmEditMassaModalEditMassaModalPost(string strDados, string ncm, string cest)
        {
                            
            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;

            string retorno = "";


            //varivael para recebe o novo ncm
            string ncmMudar = "";
            string cestMudar = "";
            bool mudarCest = false;

            //separar a String em um array
            string[] idProdutos = strDados.Split(',');

            //retira o elemento vazio do array
            idProdutos = idProdutos.Where(item => item != "").ToArray();

            ncmMudar = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços
            cestMudar = cest != "" ? cest.Trim() : null;
            mudarCest = cest == "NULL" ? true : mudarCest;

            cestMudar = cestMudar == "NULL" ? null : cestMudar; //se for nullo ele atribui null;
            if (ncmMudar != null)
            {
                if (ncmMudar != "")
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
                if (prod != null)
                {

                    if (cestMudar != null)
                    {
                        if (cestMudar != "")
                        {
                            prod.cest = cestMudar;
                        }

                    }
                    else
                    {
                        if(mudarCest)
                        {
                            prod.cest = cestMudar;
                        }
                    }
                    //verificar se veio nulo
                    if (ncmMudar != null)
                    {
                        if (ncmMudar != "")
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
                    catch (Exception e)
                    {

                       
                        string ex = e.ToString();
                        regNSalvos++;
                    }

                }

            }
            if (regSalvos > 0)
            {
                retorno = "Registro Salvo com Sucesso!!";
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
                TempData["tributacaoMTXSN"] = null;

                TempData["tributacaoMTX_NCMView"] = null;
                TempData.Keep("tributacaoMTX"); //persiste
                TempData.Keep("tributacaoMTXSN"); //persiste
            }
            else
            {
                retorno = "Nenhum item do  registro alterado";
                //Redirecionar para registros
                return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
            }

            //Redirecionar para registros
            return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }


        public ActionResult TributacaoNcmEditMassaModalEditMassaModalComNCMPost(string strDados, string ncm, string cest)
        {

            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;
           
            string retorno = "";
            //PEGAR ESTADO ORIGEM E DESTINO E SE A EMPRESA É SIMPLES NACIONAL
            string uf_origem = TempData["UfOrigem"].ToString();
            string uf_Destino = TempData["UfDestino"].ToString();

            int tipoTrib = 0;
            tipoTrib = (TempData["tributacao"].Equals("OUTROS")) ? tipoTrib : 1;

            List<TributacaoNCM> tribNCM;
            
            if(ncm != "")
            {
                string ncmReplace = ncm.Replace(".", "");
                ncmReplace = ncmReplace.Trim();
                tribNCM = db.TributacoesNcm.Where(a => a.UF_Origem == uf_origem && a.UF_Destino == uf_Destino && a.simpNacional == tipoTrib && a.ncm.Equals(ncmReplace)).ToList();
               
                if (tribNCM.Count > 1) //tem que ser maior que um para caracterizar que ela ta inserindo outro
                {
                    int qtd = 0;
                    for(int a = 0; a > tribNCM.Count -1; a++) //dessa forma ele so deixa um registro, nao impota a quantidade, e esse registro que sera excluido
                    {
                        qtd++;
                    }

                    for (int i = 0; i < qtd; i++)
                    {
                        TributacaoNCM tribDel = db.TributacoesNcm.Find(tribNCM[i].id);
                        db.TributacoesNcm.Remove(tribDel);
                        db.SaveChanges();
                    }
                    //buscando novamente apos a exclusão
                    tribNCM = db.TributacoesNcm.Where(a => a.UF_Origem == uf_origem && a.UF_Destino == uf_Destino && a.simpNacional == tipoTrib && a.ncm.Equals(ncmReplace)).ToList();

                    strDados = tribNCM[0].id.ToString();

                }
                

            }


            //varivael para recebe o novo ncm
            string ncmMudar = "";
            string cestMudar = "";
            bool mudarCest = false;

            //separar a String em um array
            string[] idTribNCM = strDados.Split(',');

            //retira o elemento vazio do array
            idTribNCM = idTribNCM.Where(item => item != "").ToArray();

            ncmMudar = ncm != "" ? ncm.Trim() : null; //ternario para remover eventuais espaços
            cestMudar = cest != "" ? cest.Trim() : null;
            mudarCest = cest == "NULL" ? true : mudarCest;

            cestMudar = cestMudar == "NULL" ? null : cestMudar; //se for nullo ele atribui null;
            if (ncmMudar != null)
            {
                if (ncmMudar != "")
                {
                    ncmMudar = ncmMudar.Replace(".", ""); //tirar os pontos da string
                }

            }


            //objeto produto
            TributacaoNCM tNCM = new TributacaoNCM();
            
            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTribNCM.Length; i++)
            {
                int idTNCM = Int32.Parse(idTribNCM[i]);
                tNCM = db.TributacoesNcm.Find(idTNCM);
                if (tNCM != null)
                {

                    if (cestMudar != null)
                    {
                        if (cestMudar != "")
                        {
                            tNCM.cest = cestMudar;
                            //tNCM.auditadoPorNCM = 1;
                            tNCM.dataAlt = DateTime.Now; //data da alteração
                            regSalvos++;
                        }

                    }
                    else
                    {
                        if (mudarCest)
                        {
                            tNCM.cest = cestMudar;
                            //tNCM.auditadoPorNCM = 1;
                            tNCM.dataAlt = DateTime.Now; //data da alteração
                            regSalvos++;
                        }
                    }
                    //verificar se veio nulo
                    if (ncmMudar != null)
                    {
                        if (ncmMudar != "") //VERIFICAR SE VEIO VAZIO
                        {
                            //FAZER UMA BUSCA PARA VERIFICAR SE JA EXISTE O NCM NA TABELA COM ESSA ORIGEM E DESTINO
                            if (tNCM.ncm != ncmMudar)
                            {
                                tNCM.ncm = ncmMudar;
                                //tNCM.auditadoPorNCM = 1;
                                tNCM.dataAlt = DateTime.Now; //data da alteração
                                regSalvos++;
                            }
                        }

                    }

                  
                    try
                    {
                        db.SaveChanges();
                      
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
                retorno = "Registro Salvo com Sucesso!!";
                TempData["tributacaoMTX"] = null;//cria a temp data e popula
                TempData["tributacaoMTXSN"] = null;
                TempData["analise_NCM"] = null;
                TempData["tributacaoMTX_NCMView"] = null;
                TempData.Keep("tributacaoMTX"); //persiste
                TempData.Keep("tributacaoMTXSN"); //persiste
            }
            else
            {
                retorno = "Nenhum item do  registro alterado";
                //Redirecionar para registros
                return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
            }

            //Redirecionar para registros
            return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
        }



        [HttpGet]
        public ActionResult TributacaoNcmEditMassaNCMModal(string id, string ncm, string titulo)
        {
            //receber os dados do ncm para alterar
            string ncmAlterar = ncm;
            ViewBag.NCM = ncmAlterar;
            ViewBag.TituloPagina = titulo;
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

            VerificaOriDest();

            //aplica estado origem e destino
            ViewBag.UfOrigem = TempData["UfOrigem"].ToString();
            ViewBag.UfDestino = TempData["UfDestino"].ToString();
            return View();
        }



        //TributacaoNcmEditMassaNCMModalPost
        //proximo
        [HttpGet]
        public ActionResult TributacaoNcmEditMassaNCMModalPost(string ncm, string fecp, string CodReceita, string CstSaidaPisCofins,  string aliqSaidaCofins,
            string aliqSaidaPis, string IdFundamentoLegal, string CstVendaVarejoConsFinal, string alVeVarCF, string alVeVarCFSt, string rBcVeVarCF, string rBcSTVeVarCF,
            string CstVendaVarejoCont, string alVeVarCont, string alVeVarContSt, string rBcVeVarCont, string rBcSTVeVarCont, string CstVendaAtaCont,
            string aliqIcmsVendaAtaCont, string aliqIcmsSTVendaAtaCont, string redBaseCalcIcmsVendaAtaCont, string redBaseCalcIcmsSTVendaAtaCont,
            string CstVendaAtaSimpNacional, string alVSN, string alVSNSt, string rBcVSN, string rBcSTVSN, string IdFundLegalSaidaICMS, string cest, string ufOrigem,
            string ufDestino, string titulo)
        {

            //  decimal aliqSaidaPisDecimal = 0; //variavel tipada
            /*Neste momento o sistema verifica se é possivel converter o que o usuario digitou
             em um valor long, se for significa que ele digitou um codigo de barras e a variavel do tipo bool recebe TRUE*/
            //bool canConvert = decimal.TryParse(aliqSaidaPis, out aliqSaidaPisDecimal);

            // aliqSaidaPisDecimal =  System.Convert.ToDecimal(aliqSaidaPis);

            // //Passar de , para ponto na string
            // fecp = fecp.Replace(",", "."); //tirar os pontos da string
            //// aliqSaidaPis = aliqSaidaPis.Replace(",", "."); 
            // aliqSaidaCofins = aliqSaidaCofins.Replace(",", ".");

            // alVeVarCF = alVeVarCF.Replace(",", ".");
            // rBcVeVarCF = rBcVeVarCF.Replace(",", ".");
            // rBcSTVeVarCF = rBcSTVeVarCF.Replace(",", ".");
            // alVeVarCont = alVeVarCont.Replace(",", ".");
            // alVeVarContSt = alVeVarContSt.Replace(",", ".");
            // rBcVeVarCont = rBcVeVarCont.Replace(",", ".");
            // rBcSTVeVarCont = rBcSTVeVarCont.Replace(",", ".");
            // aliqIcmsVendaAtaCont = aliqIcmsVendaAtaCont.Replace(",", ".");

            // aliqIcmsSTVendaAtaCont = aliqIcmsSTVendaAtaCont.Replace(",", ".");
            // redBaseCalcIcmsVendaAtaCont = redBaseCalcIcmsVendaAtaCont.Replace(",", ".");
            // redBaseCalcIcmsSTVendaAtaCont = redBaseCalcIcmsSTVendaAtaCont.Replace(",", ".");
            // alVSN = alVSN.Replace(",", ".");
            // alVSNSt = alVSNSt.Replace(",", ".");
            // rBcVSN = rBcVSN.Replace(",", ".");

            // rBcSTVSN = rBcSTVSN.Replace(",", ".");

            VerificaOriDest();
            ufOrigem = this.ufOrigem;
            ufDestino = this.ufDestino;

            int regSalvos = 0;
            int regNSalvos = 0;
            int regParaSalvar = 0;

            string retorno = "";
            //buscar os cst pela descrição
            int? cstSaidaPisCofins = (CstSaidaPisCofins == "") ? null : (int?)(long)(from a in db.CstPisCofinsSaidas where a.descricao == CstSaidaPisCofins select a.codigo).FirstOrDefault();
            int? cstVendaVarejoConsFinal = (CstVendaVarejoConsFinal == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            int? cstVendaVarejoCont = (CstVendaVarejoCont == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaVarejoCont select a.codigo).FirstOrDefault();
            int? cstVendaAtaCont = (CstVendaAtaCont == "") ? null : (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CstVendaAtaCont select a.codigo).FirstOrDefault();
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
            if (IdFundamentoLegal == "")
            {
                fundLegalCofins = null;
            }
            else
            {
                fundLegalCofins = int.Parse(IdFundamentoLegal);
            }

            //fundamento legal icms saida
            int? fundLegalIcmsSaida = 0;
            if (IdFundLegalSaidaICMS == "")
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

            if(TempData["tributacao"].ToString() == "SIMPLES")
            {
                VerificaTempDataSN();

                /*Buscar na tabema de tributacao por NCM passando que é simples nacional*/
                VerificaTempDataNCM(TempData["tributacao"].ToString()); //retona a lista de tributacao por NCM


                //buscar os NCM dentro dessa lista
                if(!String.IsNullOrEmpty(buscaNCM))
                {
                    this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.NCM == buscaNCM && s.UF_ORIGEM == ufDestino && s.UF_DESTINO == ufDestino);
                }
                //Foreach para buscar os itens na tabela de NCM a serem alterados
                TributacaoNCM trib_NCM = new TributacaoNCM(); //objeto que será alterado

                List<TributacaoNCMView> trb_NMC_List = this.tribMTX_NCMView.ToList(); //lista com os itens selecionados

                for(int i = 0; i < trb_NMC_List.Count(); i++)
                {
                    int? idTRIB = (trb_NMC_List[i].ID); //PEGA O ID DO REGISTRO NA TABELA A SER ALTERADA
                    trib_NCM = db.TributacoesNcm.Find(idTRIB); //busca na tabela esse ID

                    //uforigem e destino
                    if(ufOrigem != "null" && ufDestino != "null")
                    {
                        trib_NCM.UF_Origem = ufOrigem;
                        trib_NCM.UF_Destino = ufDestino;

                        //verifica se os parametros vieram preenchidos, caso true ele atribui ao objeto e conta um registro para salvar
                        if (cstSaidaPisCofins != null)
                        {
                            trib_NCM.cstSaidaPisCofins = cstSaidaPisCofins;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (cstVendaVarejoConsFinal != null)
                        {
                            trib_NCM.cstVendaVarejoConsFinal = cstVendaVarejoConsFinal;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }


                        if (cstVendaVarejoCont != null)
                        {

                            trib_NCM.cstVendaVarejoCont = cstVendaVarejoCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaCont != null)
                        {
                            trib_NCM.cstVendaAtaCont = cstVendaAtaCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaSimpNacional != null)
                        {
                            trib_NCM.cstVendaAtaSimpNacional = cstVendaAtaSimpNacional;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fecp != "")
                        {
                            trib_NCM.fecp = Decimal.Parse(fecp, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (codNatRec != null)
                        {
                            trib_NCM.codNatReceita = codNatRec;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (!aliqSaidaPis.Equals(""))
                        {
                            trib_NCM.aliqSaidaPis = Decimal.Parse(aliqSaidaPis, System.Globalization.CultureInfo.InvariantCulture);
                            // tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqSaidaCofins != "")
                        {
                            trib_NCM.aliqSaidaCofins = Decimal.Parse(aliqSaidaCofins, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fundLegalCofins != null)
                        {
                            trib_NCM.idFundamentoLegal = (fundLegalCofins);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCF != "")
                        {
                            trib_NCM.aliqIcmsVendaVarejoConsFinal = Decimal.Parse(alVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCFSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaVarejoConsFinal = Decimal.Parse(alVeVarCFSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVeVarCF != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaVarejoConsFinal = Decimal.Parse(rBcVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVeVarCF != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaVarejoConsFinal = Decimal.Parse(rBcSTVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCont != "")
                        {
                            trib_NCM.aliqIcmsVendaVarejoCont = Decimal.Parse(alVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarContSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaVarejo_Cont = Decimal.Parse(alVeVarContSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcVeVarCont != "")
                        {
                            trib_NCM.redBaseCalcVendaVarejoCont = Decimal.Parse(rBcVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcSTVeVarCont != "")
                        {
                            trib_NCM.RedBaseCalcSTVendaVarejo_Cont = Decimal.Parse(rBcSTVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsVendaAtaCont != "")
                        {
                            trib_NCM.aliqIcmsVendaAtaCont = Decimal.Parse(aliqIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsSTVendaAtaCont != "")
                        {
                            trib_NCM.aliqIcmsSTVendaAtaCont = Decimal.Parse(aliqIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (redBaseCalcIcmsVendaAtaCont != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaAtaCont = Decimal.Parse(redBaseCalcIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (redBaseCalcIcmsSTVendaAtaCont != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaAtaCont = Decimal.Parse(redBaseCalcIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVSN != "")
                        {
                            trib_NCM.aliqIcmsVendaAtaSimpNacional = Decimal.Parse(alVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (alVSNSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaAtaSimpNacional = Decimal.Parse(alVSNSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVSN != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaAtaSimpNacional = Decimal.Parse(rBcVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVSN != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaAtaSimpNacional = Decimal.Parse(rBcSTVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (fundLegalIcmsSaida != null)
                        {
                            trib_NCM.idFundLegalSaidaICMS = (fundLegalIcmsSaida);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cest != "")
                        {
                            trib_NCM.cest = cest;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }


                        if (regParaSalvar != 0)
                        {
                            trib_NCM.auditadoPorNCM = 1; //marca como auditado
                            //trib_NCM.produtos.auditadoNCM = 1; //marca o produto como auditado tb
                            trib_NCM.dataAlt = DateTime.Now; //data da alteração
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
                           //NAO PODE RETORNAR POIS AINDA PRECISA PASSAR PELA OUTRA TABELA
                           // return RedirectToAction("TributacaoNcmSN", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }



                    }
                } //fim do for

                //busca o nmc pelo sua origem e destino
                if (!String.IsNullOrEmpty(buscaNCM))
                {
                    //busca na view de tributacção geral sn que é a tabela de tributacao de produto em sn
                    this.lstCliSN = this.lstCliSN.Where(s => s.NCM_PRODUTO == buscaNCM && s.UF_ORIGEM == ufOrigem && s.UF_DESTINO == ufDestino);

                }

                //foreach para atualizar os produtos
                //retira o elemento vazio do array
                //objeto produto
                TributacaoSN tributaCaoSN = new TributacaoSN();

                List<TtributacaoGeralViewSN> tribMtsSN = this.lstCliSN.ToList(); //lista de todos selecionados pelo ncm

                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < tribMtsSN.Count(); i++)
                {
                    int? idTrib = (tribMtsSN[i].ID); //pega o id do registro da tributação a ser alterada
                    tributaCaoSN = db.TributacoesSn.Find(idTrib); //busca a tributação pelo seu id


                    if (ufOrigem != "null" && ufDestino != "null")
                    {
                        tributaCaoSN.UF_Origem = ufOrigem;
                        tributaCaoSN.UF_Destino = ufDestino;

                        //verifica se os parametros vieram preenchidos, caso true ele atribui ao objeto e conta um registro para salvar
                        if (cstSaidaPisCofins != null)
                        {
                            tributaCaoSN.cstSaidaPisCofins = cstSaidaPisCofins;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (cstVendaVarejoConsFinal != null)
                        {
                            tributaCaoSN.cstVendaVarejoConsFinal = cstVendaVarejoConsFinal;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaVarejoCont != null)
                        {

                            tributaCaoSN.cstVendaVarejoCont = cstVendaVarejoCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaCont != null)
                        {
                            tributaCaoSN.cstVendaAtaCont = cstVendaAtaCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaSimpNacional != null)
                        {
                            tributaCaoSN.cstVendaAtaSimpNacional = cstVendaAtaSimpNacional;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fecp != "")
                        {
                            tributaCaoSN.fecp = Decimal.Parse(fecp, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (codNatRec != null)
                        {
                            tributaCaoSN.codNatReceita = codNatRec;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (!aliqSaidaPis.Equals(""))
                        {
                            tributaCaoSN.aliqSaidaPis = Decimal.Parse(aliqSaidaPis, System.Globalization.CultureInfo.InvariantCulture);
                            // tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqSaidaCofins != "")
                        {
                            tributaCaoSN.aliqSaidaCofins = Decimal.Parse(aliqSaidaCofins, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fundLegalCofins != null)
                        {
                            tributaCaoSN.idFundamentoLegal = (fundLegalCofins);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCF != "")
                        {
                            tributaCaoSN.aliqIcmsVendaVarejoConsFinal = Decimal.Parse(alVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCFSt != "")
                        {
                            tributaCaoSN.aliqIcmsSTVendaVarejoConsFinal = Decimal.Parse(alVeVarCFSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVeVarCF != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsVendaVarejoConsFinal = Decimal.Parse(rBcVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVeVarCF != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsSTVendaVarejoConsFinal = Decimal.Parse(rBcSTVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCont != "")
                        {
                            tributaCaoSN.aliqIcmsVendaVarejoCont = Decimal.Parse(alVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarContSt != "")
                        {
                            tributaCaoSN.aliqIcmsSTVendaVarejo_Cont = Decimal.Parse(alVeVarContSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcVeVarCont != "")
                        {
                            tributaCaoSN.redBaseCalcVendaVarejoCont = Decimal.Parse(rBcVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcSTVeVarCont != "")
                        {
                            tributaCaoSN.RedBaseCalcSTVendaVarejo_Cont = Decimal.Parse(rBcSTVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsVendaAtaCont != "")
                        {
                            tributaCaoSN.aliqIcmsVendaAtaCont = Decimal.Parse(aliqIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsSTVendaAtaCont != "")
                        {
                            tributaCaoSN.aliqIcmsSTVendaAtaCont = Decimal.Parse(aliqIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (redBaseCalcIcmsVendaAtaCont != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsVendaAtaCont = Decimal.Parse(redBaseCalcIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (redBaseCalcIcmsSTVendaAtaCont != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsSTVendaAtaCont = Decimal.Parse(redBaseCalcIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVSN != "")
                        {
                            tributaCaoSN.aliqIcmsVendaAtaSimpNacional = Decimal.Parse(alVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (alVSNSt != "")
                        {
                            tributaCaoSN.aliqIcmsSTVendaAtaSimpNacional = Decimal.Parse(alVSNSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVSN != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsVendaAtaSimpNacional = Decimal.Parse(rBcVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVSN != "")
                        {
                            tributaCaoSN.redBaseCalcIcmsSTVendaAtaSimpNacional = Decimal.Parse(rBcSTVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (fundLegalIcmsSaida != null)
                        {
                            tributaCaoSN.idFundLegalSaidaICMS = (fundLegalIcmsSaida);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }


                    }


                    
                    if (cest != "")
                    {
                        tributaCaoSN.produtos.cest = cest;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }


                    if (regParaSalvar != 0)
                    {
                        tributaCaoSN.auditadoPorNCM = 1; //marca como auditado
                        tributaCaoSN.produtos.auditadoNCM = 1; //marca o produto como auditado tb
                        tributaCaoSN.dataAlt = DateTime.Now; //data da alteração
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
                        //verifica qual pagina acessou para mudar os dados
                        if (titulo != "TributacaoNcmSN")
                        {
                            return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }
                        else
                        {
                            //Redirecionar para registros
                            return RedirectToAction("TributacaoNcmSN", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }
                       
                    }



                }
                //zera a tempdata caso tenha salvo algum registros
                if (regSalvos > 0)
                {
                   // TempData["tributacaoMTX"] = null;//cria a temp data e popula
                    TempData["tributacaoMTXSN"] = null;//cria a temp data e popula
                    TempData["tributacaoMTX_NCMView"] = null;
                   // TempData.Keep("tributacaoMTX"); //persiste
                    TempData.Keep("tributacaoMTXSN"); //persiste
                    TempData.Keep("tributacaoMTX_NCMView"); //persiste
                }



                //Redirecionar para registros
                // return RedirectToAction("TributacaoNcmSN", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                //Redirecionar para registros
                if (titulo != "TributacaoNcmSN")
                {
                    return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }
                else
                {
                    //Redirecionar para registros
                    return RedirectToAction("TributacaoNcmSN", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }


            }//se nao for simples
            else
            {
                VerificaTempData();


                /*Para tabela de Tributacao por ncm*/
                /*Buscar na tabema de tributacao por NCM passando que é simples nacional*/
                VerificaTempDataNCM(TempData["tributacao"].ToString()); //retona a lista de tributacao por NCM


                //buscar os NCM dentro dessa lista
                if (!String.IsNullOrEmpty(buscaNCM))
                {
                    this.tribMTX_NCMView = this.tribMTX_NCMView.Where(s => s.NCM == buscaNCM && s.UF_ORIGEM == ufDestino && s.UF_DESTINO == ufDestino);
                }
                //Foreach para buscar os itens na tabela de NCM a serem alterados
                TributacaoNCM trib_NCM = new TributacaoNCM(); //objeto que será alterado

                List<TributacaoNCMView> trb_NMC_List = this.tribMTX_NCMView.ToList(); //lista com os itens selecionados

                for (int i = 0; i < trb_NMC_List.Count(); i++)
                {
                    int? idTRIB = (trb_NMC_List[i].ID); //PEGA O ID DO REGISTRO NA TABELA A SER ALTERADA
                    trib_NCM = db.TributacoesNcm.Find(idTRIB); //busca na tabela esse ID

                    //uforigem e destino
                    if (ufOrigem != "null" && ufDestino != "null")
                    {
                        trib_NCM.UF_Origem = ufOrigem;
                        trib_NCM.UF_Destino = ufDestino;

                        //verifica se os parametros vieram preenchidos, caso true ele atribui ao objeto e conta um registro para salvar
                        if (cstSaidaPisCofins != null)
                        {
                            trib_NCM.cstSaidaPisCofins = cstSaidaPisCofins;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (cstVendaVarejoConsFinal != null)
                        {
                            trib_NCM.cstVendaVarejoConsFinal = cstVendaVarejoConsFinal;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }


                        if (cstVendaVarejoCont != null)
                        {

                            trib_NCM.cstVendaVarejoCont = cstVendaVarejoCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaCont != null)
                        {
                            trib_NCM.cstVendaAtaCont = cstVendaAtaCont;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cstVendaAtaSimpNacional != null)
                        {
                            trib_NCM.cstVendaAtaSimpNacional = cstVendaAtaSimpNacional;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fecp != "")
                        {
                            trib_NCM.fecp = Decimal.Parse(fecp, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (codNatRec != null)
                        {
                            trib_NCM.codNatReceita = codNatRec;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (!aliqSaidaPis.Equals(""))
                        {
                            trib_NCM.aliqSaidaPis = Decimal.Parse(aliqSaidaPis, System.Globalization.CultureInfo.InvariantCulture);
                            // tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqSaidaCofins != "")
                        {
                            trib_NCM.aliqSaidaCofins = Decimal.Parse(aliqSaidaCofins, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fundLegalCofins != null)
                        {
                            trib_NCM.idFundamentoLegal = (fundLegalCofins);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCF != "")
                        {
                            trib_NCM.aliqIcmsVendaVarejoConsFinal = Decimal.Parse(alVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCFSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaVarejoConsFinal = Decimal.Parse(alVeVarCFSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVeVarCF != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaVarejoConsFinal = Decimal.Parse(rBcVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVeVarCF != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaVarejoConsFinal = Decimal.Parse(rBcSTVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCont != "")
                        {
                            trib_NCM.aliqIcmsVendaVarejoCont = Decimal.Parse(alVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarContSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaVarejo_Cont = Decimal.Parse(alVeVarContSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcVeVarCont != "")
                        {
                            trib_NCM.redBaseCalcVendaVarejoCont = Decimal.Parse(rBcVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcSTVeVarCont != "")
                        {
                            trib_NCM.RedBaseCalcSTVendaVarejo_Cont = Decimal.Parse(rBcSTVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsVendaAtaCont != "")
                        {
                            trib_NCM.aliqIcmsVendaAtaCont = Decimal.Parse(aliqIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsSTVendaAtaCont != "")
                        {
                            trib_NCM.aliqIcmsSTVendaAtaCont = Decimal.Parse(aliqIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (redBaseCalcIcmsVendaAtaCont != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaAtaCont = Decimal.Parse(redBaseCalcIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (redBaseCalcIcmsSTVendaAtaCont != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaAtaCont = Decimal.Parse(redBaseCalcIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVSN != "")
                        {
                            trib_NCM.aliqIcmsVendaAtaSimpNacional = Decimal.Parse(alVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (alVSNSt != "")
                        {
                            trib_NCM.aliqIcmsSTVendaAtaSimpNacional = Decimal.Parse(alVSNSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVSN != "")
                        {
                            trib_NCM.redBaseCalcIcmsVendaAtaSimpNacional = Decimal.Parse(rBcVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVSN != "")
                        {
                            trib_NCM.redBaseCalcIcmsSTVendaAtaSimpNacional = Decimal.Parse(rBcSTVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (fundLegalIcmsSaida != null)
                        {
                            trib_NCM.idFundLegalSaidaICMS = (fundLegalIcmsSaida);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (cest != "")
                        {
                            trib_NCM.cest = cest;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }


                        if (regParaSalvar != 0)
                        {
                            trib_NCM.auditadoPorNCM = 1; //marca como auditado
                            //trib_NCM.produtos.auditadoNCM = 1; //marca o produto como auditado tb
                            trib_NCM.dataAlt = DateTime.Now; //data da alteração
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
                            //NAO PODE RETORNAR POIS AINDA PRECISA PASSAR PELA OUTRA TABELA
                            // return RedirectToAction("TributacaoNcmSN", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }



                    }
                } //fim do for



                //busca o nmc pelo sua origem e destino
                if (!String.IsNullOrEmpty(buscaNCM))
                {
                    this.lstCli = this.lstCli.Where(s => s.NCM_PRODUTO == buscaNCM && s.UF_ORIGEM == ufOrigem && s.UF_DESTINO == ufDestino);

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
                            tributaCao.fecp = Decimal.Parse(fecp, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (codNatRec != null)
                        {
                            tributaCao.codNatReceita = codNatRec;
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (!aliqSaidaPis.Equals(""))
                        {
                            tributaCao.aliqSaidaPis = Decimal.Parse(aliqSaidaPis, System.Globalization.CultureInfo.InvariantCulture);
                            // tributaCao.aliqSaidaPis = decimal.Parse(aliqSaidaPis);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqSaidaCofins != "")
                        {
                            tributaCao.aliqSaidaCofins = Decimal.Parse(aliqSaidaCofins, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (fundLegalCofins != null)
                        {
                            tributaCao.idFundamentoLegal = (fundLegalCofins);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCF != "")
                        {
                            tributaCao.aliqIcmsVendaVarejoConsFinal = Decimal.Parse(alVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCFSt != "")
                        {
                            tributaCao.aliqIcmsSTVendaVarejoConsFinal = Decimal.Parse(alVeVarCFSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVeVarCF != "")
                        {
                            tributaCao.redBaseCalcIcmsVendaVarejoConsFinal = Decimal.Parse(rBcVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVeVarCF != "")
                        {
                            tributaCao.redBaseCalcIcmsSTVendaVarejoConsFinal = Decimal.Parse(rBcSTVeVarCF, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarCont != "")
                        {
                            tributaCao.aliqIcmsVendaVarejoCont = Decimal.Parse(alVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVeVarContSt != "")
                        {
                            tributaCao.aliqIcmsSTVendaVarejo_Cont = Decimal.Parse(alVeVarContSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcVeVarCont != "")
                        {
                            tributaCao.redBaseCalcVendaVarejoCont = Decimal.Parse(rBcVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (rBcSTVeVarCont != "")
                        {
                            tributaCao.RedBaseCalcSTVendaVarejo_Cont = Decimal.Parse(rBcSTVeVarCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsVendaAtaCont != "")
                        {
                            tributaCao.aliqIcmsVendaAtaCont = Decimal.Parse(aliqIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (aliqIcmsSTVendaAtaCont != "")
                        {
                            tributaCao.aliqIcmsSTVendaAtaCont = Decimal.Parse(aliqIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (redBaseCalcIcmsVendaAtaCont != "")
                        {
                            tributaCao.redBaseCalcIcmsVendaAtaCont = Decimal.Parse(redBaseCalcIcmsVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (redBaseCalcIcmsSTVendaAtaCont != "")
                        {
                            tributaCao.redBaseCalcIcmsSTVendaAtaCont = Decimal.Parse(redBaseCalcIcmsSTVendaAtaCont, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }

                        if (alVSN != "")
                        {
                            tributaCao.aliqIcmsVendaAtaSimpNacional = Decimal.Parse(alVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (alVSNSt != "")
                        {
                            tributaCao.aliqIcmsSTVendaAtaSimpNacional = Decimal.Parse(alVSNSt, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcVSN != "")
                        {
                            tributaCao.redBaseCalcIcmsVendaAtaSimpNacional = Decimal.Parse(rBcVSN, System.Globalization.CultureInfo.InvariantCulture);
                            regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                        }
                        if (rBcSTVSN != "")
                        {
                            tributaCao.redBaseCalcIcmsSTVendaAtaSimpNacional = Decimal.Parse(rBcSTVSN, System.Globalization.CultureInfo.InvariantCulture);
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


                    if (cest != "")
                    {
                        tributaCao.produtos.cest = cest;
                        regParaSalvar++; //variavel auxiliar - conta os registros que poerão ser salvos
                    }


                    if (regParaSalvar != 0)
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
                        //retorno = "Nenhum item do  registro alterado";
                        ////Redirecionar para registros
                        //return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });


                        retorno = "Nenhum item do  registro alterado";
                        //verifica qual pagina acessou para mudar os dados
                        if (titulo != "TributacaoNcm")
                        {
                            return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }
                        else
                        {
                            //Redirecionar para registros
                            return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                        }
                    }



                } //fim do for
                  //zera a tempdata caso tenha salvo algum registros
                if (regSalvos > 0)
                {
                   
                    TempData["tributacaoMTX"] = null;//cria a temp data e popula
                  //  TempData["tributacaoMTXSN"] = null;//cria a temp data e popula
                    TempData["tributacaoMTX_NCMView"] = null;
                    TempData.Keep("tributacaoMTX"); //persiste
                   // TempData.Keep("tributacaoMTXSN"); //persiste
                    TempData.Keep("tributacaoMTX_NCMView"); //persiste

                }

                //Redirecionar para registros
                if (titulo != "TributacaoNcm")
                {
                    return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }
                else
                {
                    //Redirecionar para registros
                    return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });
                }
                // return RedirectToAction("TributacaoNcm", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });


            }

            return null;
           
        }

        public ActionResult Detalhes(string id)
        {
            int ident = int.Parse(id);

            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TributacaoNCM tributacaoNcm = db.TributacoesNcm.Find(ident);
            if (tributacaoNcm == null)
            {
                return HttpNotFound();
            }

            ViewBag.Cest =tributacaoNcm.cest;
            ViewBag.Ncm = tributacaoNcm.ncm;


            //Condição para montar a viewbag: se existir natureza da receita cadastrada; caso contrario null
            ViewBag.NaturezaReceita = (tributacaoNcm.codNatReceita == null)?null:db.NaturezaReceitas.Find(tributacaoNcm.codNatReceita).descricao;
            ViewBag.FundLegalPC = (tributacaoNcm.idFundamentoLegal == null)?null: db.Legislacoes.Find(tributacaoNcm.idFundamentoLegal).fundLegal;
            ViewBag.FundLegalSaida = (tributacaoNcm.idFundLegalSaidaICMS ==null)?null: db.Legislacoes.Find(tributacaoNcm.idFundLegalSaidaICMS).fundLegal;
            ViewBag.FundLegalEndrada = (tributacaoNcm.idFundLelgalEntradaICMS == null)?null: db.Legislacoes.Find(tributacaoNcm.idFundLelgalEntradaICMS).fundLegal;
            ViewBag.Categoria = (tributacaoNcm.categoria == null) ? null : db.CategoriaProdutos.Find(tributacaoNcm.categoria).descricao;


            //ViewBag.Legislacao = db.Legislacoes.Find(tributacao.idFundLelgalEntradaICMS).id;
            ViewBag.CreditoOutorgado = tributacaoNcm.creditoOutorgado;
            ViewBag.Regime2560 = tributacaoNcm.regime2560;
            ViewBag.DataInicio = tributacaoNcm.inicioVigenciaMVA;
            ViewBag.DataFim = tributacaoNcm.fimVigenciaMVA;

            ViewBag.UfOrigem = tributacaoNcm.UF_Origem;
            ViewBag.UfDestino = tributacaoNcm.UF_Destino;
            ViewBag.DtaAlt = tributacaoNcm.DataFormatada;
            return View(tributacaoNcm);
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
                return RedirectToAction("Erro", new { param = par });
            }

            /*Carregar as viewbags*/
            ViewBag.Produtos = db.Produtos;
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            var prod = from s in db.Produtos select s; //variavel carregado de produtos

            ViewBag.ProdList = prod;
            ViewBag.DataAlt = DateTime.Now;
            ViewBag.DataCad = DateTime.Now;

            
            ViewBag.ProdutosComplete = AutoComplete();

            var model = new TributacaoViewModel();

            return View(model);
        }

        /*Implementação motivada por não vir o codigo do cst e sim a descrição
         do elemento dropdawn, assim a descrição é passada como parametro da 
        busca no banco e a variavel é atualizada com o codigo da cst*/
        [HttpPost]
        public ActionResult Create(TributacaoViewModel model, string cstEntradaPisCofins, string cstSaidaPisCofins, string cstVendaAtaCont, string cstVendaAtaSimpNacional, string cstVendaVarejoCont,
            string cstVendaVarejoConsFinal, string cstCompraDeInd, string cstdaNfedaIndFORN, string cstCompradeAta, string cstdaNfedeAtaFORn, string cstCompradeSimpNacional, 
            string CsosntdaNfedoSnFOR, int? idProduto)
        {
            /*verificar os dados de CST no inicio: caso estejam em branco o valor no model permanece NULL
             evitando assim o erro de busca por valores inexistentes na tabela de cst uma vez que o campo é
            uma chave estrangeira*/
            model.cstEntradaPisCofins = (cstEntradaPisCofins == "") ? null : (int?)(long)(from a in db.CstPisCofinsEntradas where a.descricao == cstEntradaPisCofins select a.codigo).FirstOrDefault();
            model.cstSaidaPisCofins = (cstSaidaPisCofins == "")? null: (int?)(long)(from a in db.CstPisCofinsSaidas where a.descricao == cstSaidaPisCofins select a.codigo).FirstOrDefault();
            model.cstVendaAtaCont = (cstVendaAtaCont == "")? null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaAtaCont select a.codigo).FirstOrDefault();
            model.cstVendaAtaSimpNacional = (cstVendaAtaSimpNacional == "")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            model.cstVendaVarejoCont = (cstVendaVarejoCont == "")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoCont select a.codigo).FirstOrDefault();
            model.cstVendaVarejoConsFinal = (cstVendaVarejoConsFinal =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            model.cstCompraDeInd = (cstCompraDeInd =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompraDeInd select a.codigo).FirstOrDefault();
            model.cstdaNfedaIndFORN = (cstdaNfedaIndFORN =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            model.cstCompradeAta = (cstCompradeAta =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompradeAta select a.codigo).FirstOrDefault();
            model.cstdaNfedeAtaFORn = (cstdaNfedeAtaFORn =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstdaNfedeAtaFORn select a.codigo).FirstOrDefault();
            model.cstCompradeSimpNacional = (cstCompradeSimpNacional =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == cstCompradeSimpNacional select a.codigo).FirstOrDefault();
            model.CsosntdaNfedoSnFOR = (CsosntdaNfedoSnFOR =="")?null: (int?)(long)(from a in db.CstIcmsGerais where a.descricao == CsosntdaNfedoSnFOR select a.codigo).FirstOrDefault();


           
            //validando campos
            if ( (idProduto == null) || (model.UF_Destino ==null) || (model.UF_Origem == null) || (model.idSetor.Equals(0)))
            {
               
                ViewBag.Obrigatorio = "Produto, Estado ou Setor do Produto vazio!. Revise os dados informados!!!";
                /*Carregar as View Bag*/
                var prod2 = from s in db.Produtos select s; //variavel carregado de produtos

                ViewBag.ProdList = prod2;
                ViewBag.Produtos = db.Produtos;
                ViewBag.Setor = db.SetorProdutos;
                ViewBag.NatReceita = db.NaturezaReceitas;
                ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
                ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
                ViewBag.FundLegal = db.Legislacoes;
                ViewBag.CstIcms = db.CstIcmsGerais;
                ViewBag.FundLegalPC = db.Legislacoes;
                ViewBag.FundLegalSaida = db.Legislacoes;
                ViewBag.FundLegalEndrada = db.Legislacoes;
                ViewBag.Legislacao = db.Legislacoes;
                ViewBag.CstGeral = db.CstIcmsGerais;

                return View(model);

            }

            try
            {
                model.idProduto = (int)idProduto; //recebe o id do produto
                //iformando a data do dia da criação do registro
                model.dataCad = DateTime.Now;
                model.dataAlt = DateTime.Now;

                var tributacao = new Tributacao()
                {
                    //estado = model.estado,
                    UF_Origem = model.UF_Origem,
                    UF_Destino = model.UF_Destino,
                    idProduto = model.idProduto,
                    idSetor = model.idSetor,
                    fecp = model.fecp,
                    codNatReceita = model.codNatReceita,

                    cstEntradaPisCofins = model.cstEntradaPisCofins,
                    cstSaidaPisCofins = model.cstSaidaPisCofins,
                    aliqEntPis = model.aliqEntPis,
                    aliqSaidaPis = model.aliqSaidaPis,
                    aliqEntCofins = model.aliqEntCofins,
                    aliqSaidaCofins = model.aliqSaidaCofins,

                    idFundamentoLegal = model.idFundamentoLegal,

                    cstVendaAtaCont = model.cstVendaAtaCont,
                    aliqIcmsVendaAtaCont = model.aliqIcmsVendaAtaCont,
                    aliqIcmsSTVendaAtaCont = model.aliqIcmsSTVendaAtaCont,
                    redBaseCalcIcmsVendaAtaCont = model.redBaseCalcIcmsVendaAtaCont,
                    redBaseCalcIcmsSTVendaAtaCont = model.redBaseCalcIcmsSTVendaAtaCont,

                    cstVendaAtaSimpNacional = model.cstVendaAtaSimpNacional,
                    aliqIcmsVendaAtaSimpNacional = model.aliqIcmsVendaAtaSimpNacional,
                    aliqIcmsSTVendaAtaSimpNacional = model.aliqIcmsSTVendaAtaSimpNacional,
                    redBaseCalcIcmsVendaAtaSimpNacional = model.redBaseCalcIcmsVendaAtaSimpNacional,
                    redBaseCalcIcmsSTVendaAtaSimpNacional = model.redBaseCalcIcmsSTVendaAtaSimpNacional,

                    cstVendaVarejoCont = model.cstVendaVarejoCont,
                    aliqIcmsVendaVarejoCont = model.aliqIcmsVendaVarejoCont,
                    aliqIcmsSTVendaVarejo_Cont = model.aliqIcmsSTVendaVarejo_Cont,
                    redBaseCalcVendaVarejoCont = model.redBaseCalcVendaVarejoCont,
                    RedBaseCalcSTVendaVarejo_Cont = model.RedBaseCalcSTVendaVarejo_Cont,


                    cstVendaVarejoConsFinal = model.cstVendaVarejoConsFinal,
                    aliqIcmsVendaVarejoConsFinal = model.aliqIcmsVendaVarejoConsFinal,
                    aliqIcmsSTVendaVarejoConsFinal = model.aliqIcmsSTVendaVarejoConsFinal,
                    redBaseCalcIcmsVendaVarejoConsFinal = model.redBaseCalcIcmsVendaVarejoConsFinal,
                    redBaseCalcIcmsSTVendaVarejoConsFinal = model.redBaseCalcIcmsSTVendaVarejoConsFinal,


                    idFundLegalSaidaICMS = model.idFundLegalSaidaICMS,
                    idFundLelgalEntradaICMS = model.idFundLelgalEntradaICMS,

                    cstCompraDeInd = model.cstCompraDeInd,
                    aliqIcmsCompDeInd = model.aliqIcmsCompDeInd,
                    aliqIcmsSTCompDeInd = model.aliqIcmsSTCompDeInd,
                    redBaseCalcIcmsCompraDeInd = model.redBaseCalcIcmsCompraDeInd,
                    redBaseCalcIcmsSTCompraDeInd = model.redBaseCalcIcmsSTCompraDeInd,

                    cstCompradeAta = model.cstCompradeAta,
                    aliqIcmsCompradeAta = model.aliqIcmsCompradeAta,
                    aliqIcmsSTCompraDeAta = model.aliqIcmsSTCompraDeAta,
                    redBaseCalcIcmsCompraDeAta = model.redBaseCalcIcmsCompraDeAta,
                    redBaseCalcIcmsSTCompraDeAta = model.redBaseCalcIcmsSTCompraDeAta,

                    cstCompradeSimpNacional = model.cstCompradeSimpNacional,
                    aliqIcmsCompradeSimpNacional = model.aliqIcmsCompradeSimpNacional,
                    aliqIcmsSTCompradeSimpNacional = model.aliqIcmsSTCompradeSimpNacional,
                    redBaseCalcIcmsCompradeSimpNacional = model.redBaseCalcIcmsCompradeSimpNacional,
                    redBaseCalcIcmsSTCompradeSimpNacional = model.redBaseCalcIcmsSTCompradeSimpNacional,

                    cstdaNfedaIndFORN = model.cstdaNfedaIndFORN,
                    cstdaNfedeAtaFORn = model.cstdaNfedeAtaFORn,
                    CsosntdaNfedoSnFOR = model.CsosntdaNfedoSnFOR,
                    aliqIcmsNFE = model.aliqIcmsNFE,
                    aliqIcmsNfeAta = model.aliqIcmsNfeAta,
                    aliqIcmsNfeSN = model.aliqIcmsNfeSN,

                    tipoMVA = model.tipoMVA,
                    valorMVAInd = model.valorMVAInd,
                    inicioVigenciaMVA = model.inicioVigenciaMVA,
                    fimVigenciaMVA = model.fimVigenciaMVA,
                    creditoOutorgado = model.creditoOutorgado,
                    valorMVAAtacado = model.valorMVAAtacado,
                    regime2560 = model.regime2560,

                    dataCad = model.dataCad,
                    dataAlt = model.dataAlt

                };
                TempData["IDPRODUTO"] = null;

                db.Tributacoes.Add(tributacao);
                db.SaveChanges();
                TempData["tributacaoMTX"] = null; //anulando a temdata que monta o index
                return RedirectToAction("Index");//caso salve
            }
            catch(Exception e)
            {
               // Session["ERR_SALVAR"] = "ERRO AO SALVAR OS DADOS, POR FAVOR VERIFIQUE OS DADOS E TENTE NOVAMENTE: "+e.ToString();
                ViewBag.ErrSalvar = "ERRO AO SALVAR OS DADOS, POR FAVOR VERIFIQUE OS DADOS E TENTE NOVAMENTE: " + e.ToString();
            }


            /*Carregar as View Bag*/
            var prod = from s in db.Produtos select s; //variavel carregado de produtos

            ViewBag.ProdList = prod; //lista com os produtos
            ViewBag.Produtos = db.Produtos;
            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            ViewBag.FundLegal = db.Legislacoes;
            ViewBag.CstIcms = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.FundLegalSaida = db.Legislacoes;
            ViewBag.FundLegalEndrada = db.Legislacoes;
            ViewBag.Legislacao = db.Legislacoes;
            ViewBag.CstGeral = db.CstIcmsGerais;


            return View(model);
        }

        public ActionResult ProcuraProduto(string sortOrder, string searchString, string currentFilter, int? page, string linhasNum)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            // ViewBag.NumLinhas = linhasNum;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";
            ViewBag.CatProduto = sortOrder == "CatProd" ? "CatProd_desc" : "CatProd";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var produtos = from s in db.Produtos select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(s => s.descricao.ToString().ToUpper().StartsWith(searchString.ToUpper()) || s.categoriaProduto.descricao.ToString().ToUpper().StartsWith(searchString.ToUpper()));

            }
            switch (sortOrder)
            {
                case "Produto_desc":
                    produtos = produtos.OrderByDescending(s => s.descricao);
                    break;
                case "CatProd":
                    produtos = produtos.OrderBy(s => s.idCategoria);
                    break;
                case "CatProd_desc":
                    produtos = produtos.OrderByDescending(s => s.idCategoria);
                    break;
                default:
                    produtos = produtos.OrderBy(s => s.Id);
                    break;
            }
            int pageSize = 0;

            if (String.IsNullOrEmpty(linhasNum))
            {
                pageSize = 10;
            }
            else
            {

                ViewBag.Texto = linhasNum;
                pageSize = Int32.Parse(linhasNum);
            }


            int pageNumber = (page ?? 1);

            //var produtos = db.Produtos;
            return View(produtos.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho

        }

        /*Edição*/
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            /*Verificia a sessão  do usuário: caso nulo redireciona*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            /*Verifica o nível de acesso: passa um parametro caso seja igual USUARIO*/
            if (Session["nivel"].Equals("USUARIO"))
            {
                int par = 2;
                return RedirectToAction("../Erro/Erro", new { param = par });
            }

            /*Verifica se o id é nulo*/
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /*Instancia um objeto de acordo com o Id passado como parametro*/
            Tributacao tributacao = db.Tributacoes.Find(id);

            /*Verifica se o objeto foi instanciado*/
            if (tributacao == null)
            {
                return HttpNotFound();
            }

            /*Verificando cst na tabela e Preencher a descrição dos csts
             Obs.: Quando o registro não possui algun desses valores a
            consulta ao banco retorna null, dando exceção por estar nulo
            dessa forma ele valida na condição e não permite valores
            nulos sejam tratados
             */
            CstIcmsGeral cstGeral1 = db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont);
            CstIcmsGeral cstGeral2 = db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional);
            CstIcmsGeral cstGeral3 = db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont);
            CstIcmsGeral cstGeral4 = db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal);
            CstIcmsGeral cstGeral5 = db.CstIcmsGerais.Find(tributacao.cstCompraDeInd);
            CstIcmsGeral cstGeral6 = db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN);
            CstIcmsGeral cstGeral7 = db.CstIcmsGerais.Find(tributacao.cstCompradeAta);
            CstIcmsGeral cstGeral8 = db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn);
            CstIcmsGeral cstGeral9 = db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional);
            CstIcmsGeral cstGeral10 = db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR);

            if (cstGeral1 == null)
            {
                ViewBag.CstVendaAtaContDescricao = "";
            }
            else
            {
                ViewBag.CstVendaAtaContDescricao = cstGeral1.descricao;
            }

            if (cstGeral2 == null)
            {
                ViewBag.CstVendaSimpNacionalDescricao = "";
            }
            else
            {
                ViewBag.CstVendaSimpNacionalDescricao = cstGeral2.descricao;
            }

            if (cstGeral3 == null)
            {
                ViewBag.CstVendaVarCont = "";
            }
            else
            {
                ViewBag.CstVendaVarCont = cstGeral3.descricao;
            }

            if (cstGeral4 == null)
            {
                ViewBag.CstVendaVarejoCF = "";
            }
            else
            {
                ViewBag.CstVendaVarejoCF = cstGeral4.descricao;
            }

            if (cstGeral5 == null)
            {
                ViewBag.CstCompraDeInd = "";
            }
            else
            {
                ViewBag.CstCompraDeInd = cstGeral5.descricao;
            }

            if (cstGeral6 == null)
            {
                ViewBag.CstDaNfeIndForn = "";
            }
            else
            {
                ViewBag.CstDaNfeIndForn = cstGeral6.descricao;
            }

            if (cstGeral7 == null)
            {
                ViewBag.CstCompraATA = "";
            }
            else
            {
                ViewBag.CstCompraATA = cstGeral7.descricao;
            }

            if (cstGeral8 == null)
            {
                ViewBag.CstDaNfeAtaForn = "";
            }
            else
            {
                ViewBag.CstDaNfeAtaForn = cstGeral8.descricao;
            }

            if (cstGeral9 == null)
            {
                ViewBag.CstCompraSimNacional = "";
            }
            else
            {
                ViewBag.CstCompraSimNacional = cstGeral9.descricao;
            }
            if (cstGeral10 == null)
            {
                ViewBag.CstNfeSNfor = "";
            }
            else
            {
                ViewBag.CstNfeSNfor = cstGeral10.descricao;
            }

            ViewBag.Setor = db.SetorProdutos;
            ViewBag.NatReceita = db.NaturezaReceitas;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            ViewBag.CstGeral = db.CstIcmsGerais;
            ViewBag.FundLegalPC = db.Legislacoes;
            ViewBag.DataAlt = DateTime.Now;

            return View(tributacao);
        }


        //[HttpPost]
        //public ActionResult EditPost(string estado, string idSetor, string fecp, string codNatReceita, string aliqEntPis, string aliqSaidaPis, string aliqEntCofins, string aliqSaidaCofins, string idFundamentoLegal, string aliqIcmsVendaAtaCont,
        //    string redBaseCalcIcmsVendaAtaCont, string redBaseCalcIcmsSTVendaAtaCont, string aliqIcmsVendaAtaSimpNacional, string aliqIcmsSTVendaAtaSimpNacional, string redBaseCalcIcmsVendaAtaSimpNacional,
        //    string redBaseCalcIcmsSTVendaAtaSimpNacional , string aliqIcmsVendaVarejoCont, string aliqIcmsSTVendaAtaCont , string aliqIcmsSTVendaVarejo_Cont, string redBaseCalcVendaVarejoCont, string RedBaseCalcSTVendaVarejo_Cont , string aliqIcmsVendaVarejoConsFinal,
        //    string aliqIcmsSTVendaVarejoConsFinal , string redBaseCalcIcmsVendaVarejoConsFinal, string redBaseCalcIcmsSTVendaVarejoConsFinal , string idFundLegalSaidaICMS , string aliqIcmsCompDeInd , string aliqIcmsSTCompDeInd , string redBaseCalcIcmsCompraDeInd ,
        //    string redBaseCalcIcmsSTCompraDeInd , string aliqIcmsNFE, string redBaseCalcIcmsCompraDeAta, string redBaseCalcIcmsSTCompraDeAta, string aliqIcmsCompradeAta , string  aliqIcmsSTCompraDeAta, string aliqIcmsNfeAta , string aliqIcmsCompradeSimpNacional,
        //    string aliqIcmsSTCompradeSimpNacional , string redBaseCalcIcmsCompradeSimpNacional, string redBaseCalcIcmsSTCompradeSimpNacional, string aliqIcmsNfeSN , string idFundLelgalEntradaICMS , string regime2560 , string tipoMVA , string valorMVAInd , string inicioVigenciaMVA ,
        //    string valorMVAAtacado , string fimVigenciaMVA , string creditoOutorgado , string dataAlt, string cstEntradaPisCofins, string cstSaidaPisCofins, string cstVendaAtaCont, string cstVendaAtaSimpNacional,
        //    string cstVendaVarejoCont, string cstVendaVarejoConsFinal, string cstCompraDeInd, string cstdaNfedaIndFORN, string cstCompradeAta, string cstdaNfedeAtaFORn, string cstCompradeSimpNacional,
        //    string CsosntdaNfedoSnFOR) 
        //{


        //    return null;
        //}


        [HttpPost]
        public ActionResult Edit([Bind(Include = "id, estado, idProduto, idSetor, fecp, codNatReceita, aliqEntPis, aliqSaidaPis, aliqEntCofins, aliqSaidaCofins, idFundamentoLegal, aliqIcmsVendaAtaCont," +
            "aliqIcmsSTVendaAtaCont, redBaseCalcIcmsVendaAtaCont, redBaseCalcIcmsSTVendaAtaCont, aliqIcmsVendaAtaSimpNacional ,aliqIcmsSTVendaAtaSimpNacional ,redBaseCalcIcmsVendaAtaSimpNacional ," +
            "redBaseCalcIcmsSTVendaAtaSimpNacional ,aliqIcmsVendaVarejoCont ,aliqIcmsSTVendaVarejo_Cont, redBaseCalcVendaVarejoCont ,RedBaseCalcSTVendaVarejo_Cont ,aliqIcmsVendaVarejoConsFinal ," +
            "aliqIcmsSTVendaVarejoConsFinal ,redBaseCalcIcmsVendaVarejoConsFinal ,redBaseCalcIcmsSTVendaVarejoConsFinal ,idFundLegalSaidaICMS ,aliqIcmsCompDeInd ,aliqIcmsSTCompDeInd ,redBaseCalcIcmsCompraDeInd ," +
            "redBaseCalcIcmsSTCompraDeInd ,aliqIcmsNFE ,redBaseCalcIcmsCompraDeAta ,redBaseCalcIcmsSTCompraDeAta ,aliqIcmsCompradeAta ,aliqIcmsSTCompraDeAta ,aliqIcmsNfeAta ,aliqIcmsCompradeSimpNacional ," +
            "aliqIcmsSTCompradeSimpNacional ,redBaseCalcIcmsCompradeSimpNacional ,redBaseCalcIcmsSTCompradeSimpNacional ,aliqIcmsNfeSN ,idFundLelgalEntradaICMS ,regime2560 ,tipoMVA ,valorMVAInd ,inicioVigenciaMVA ," +
            "valorMVAAtacado ,fimVigenciaMVA ,creditoOutorgado ,dataAlt")] string cstEntradaPisCofins, string cstSaidaPisCofins, string cstVendaAtaCont, string cstVendaAtaSimpNacional,
            string cstVendaVarejoCont, string cstVendaVarejoConsFinal, string cstCompraDeInd, string cstdaNfedaIndFORN, string cstCompradeAta, string cstdaNfedeAtaFORn, string cstCompradeSimpNacional,
            string CsosntdaNfedoSnFOR, Tributacao model)
        {

            //como chega
            string fecpString      = model.fecp.ToString();
            string aliqEntPis_S    = model.aliqEntPis.ToString();
            string aliqSaidaPis_S  = model.aliqSaidaPis.ToString();
            string aliqEntCofins_S = model.aliqEntCofins.ToString();

            string aliqSaidaCofins_S = model.aliqSaidaCofins.ToString();
            string aliqIcmsVendaAtaCont_S = model.aliqIcmsVendaAtaCont.ToString();
            string aliqIcmsSTVendaAtaCont_S = model.aliqIcmsSTVendaAtaCont.ToString();
            string redBaseCalcIcmsVendaAtaCont_S = model.redBaseCalcIcmsVendaAtaCont.ToString();
            string redBaseCalcIcmsSTVendaAtaCont_S = model.redBaseCalcIcmsSTVendaAtaCont.ToString();
            string aliqIcmsVendaAtaSimpNacional_S = model.aliqIcmsVendaAtaSimpNacional.ToString();
            string aliqIcmsSTVendaAtaSimpNacional_S = model.aliqIcmsSTVendaAtaSimpNacional.ToString();
            string redBaseCalcIcmsVendaAtaSimpNacional_S = model.redBaseCalcIcmsVendaAtaSimpNacional.ToString();
            string redBaseCalcIcmsSTVendaAtaSimpNacional_S = model.redBaseCalcIcmsSTVendaAtaSimpNacional.ToString();
            string aliqIcmsVendaVarejoCont_S = model.aliqIcmsVendaVarejoCont.ToString();
            string aliqIcmsSTVendaVarejo_Cont_S = model.aliqIcmsSTVendaVarejo_Cont.ToString();
            string redBaseCalcVendaVarejoCont_S = model.redBaseCalcVendaVarejoCont.ToString();
            string RedBaseCalcSTVendaVarejo_Cont_S = model.RedBaseCalcSTVendaVarejo_Cont.ToString();
            string aliqIcmsVendaVarejoConsFinal_S = model.aliqIcmsVendaVarejoConsFinal.ToString();
            string aliqIcmsSTVendaVarejoConsFinal_S = model.aliqIcmsSTVendaVarejoConsFinal.ToString();
            string redBaseCalcIcmsVendaVarejoConsFinal_S = model.redBaseCalcIcmsVendaVarejoConsFinal.ToString();
            string redBaseCalcIcmsSTVendaVarejoConsFinal_S = model.redBaseCalcIcmsSTVendaVarejoConsFinal.ToString();

            string aliqIcmsCompDeInd_S = model.aliqIcmsCompDeInd.ToString();
            string aliqIcmsSTCompDeInd_S = model.aliqIcmsSTCompDeInd.ToString();
            string redBaseCalcIcmsCompraDeInd_S = model.redBaseCalcIcmsCompraDeInd.ToString();
            string redBaseCalcIcmsSTCompraDeInd_S = model.redBaseCalcIcmsSTCompraDeInd.ToString();
            string aliqIcmsNFE_S = model.aliqIcmsNFE.ToString();
            string redBaseCalcIcmsCompraDeAta_S = model.redBaseCalcIcmsCompraDeAta.ToString();
            string redBaseCalcIcmsSTCompraDeAta_S = model.redBaseCalcIcmsSTCompraDeAta.ToString();
            string aliqIcmsCompradeAta_S = model.aliqIcmsCompradeAta.ToString();
            string aliqIcmsSTCompraDeAta_S = model.aliqIcmsSTCompraDeAta.ToString();
            string aliqIcmsNfeAta_S = model.aliqIcmsNfeAta.ToString();

            string aliqIcmsCompradeSimpNacional_S = model.aliqIcmsCompradeSimpNacional.ToString();
            string aliqIcmsSTCompradeSimpNacional_S = model.aliqIcmsSTCompradeSimpNacional.ToString();

            string redBaseCalcIcmsCompradeSimpNacional_S = model.redBaseCalcIcmsCompradeSimpNacional.ToString();
            string redBaseCalcIcmsSTCompradeSimpNacional_S = model.redBaseCalcIcmsSTCompradeSimpNacional.ToString();
            string aliqIcmsNfeSN_S = model.aliqIcmsNfeSN.ToString();

            string valorMVAAtacado_S = model.valorMVAAtacado.ToString();
            string valorMVAInd_S = model.valorMVAInd.ToString();
            






            fecpString = fecpString.Replace(",", ".");


            aliqEntPis_S = aliqEntPis_S.Replace(",", ".");
            aliqSaidaPis_S = aliqSaidaPis_S.Replace(",", ".");
            aliqEntCofins_S = aliqEntCofins_S.Replace(",", ".");
            aliqSaidaCofins_S = aliqSaidaCofins_S.Replace(",", ".");
            aliqIcmsVendaAtaCont_S = aliqIcmsVendaAtaCont_S.Replace(",", ".");
            aliqIcmsSTVendaAtaCont_S = aliqIcmsSTVendaAtaCont_S.Replace(",", ".");
            redBaseCalcIcmsVendaAtaCont_S = redBaseCalcIcmsVendaAtaCont_S.Replace(",", ".");
            redBaseCalcIcmsSTVendaAtaCont_S = redBaseCalcIcmsSTVendaAtaCont_S.Replace(",", ".");
            aliqIcmsVendaAtaSimpNacional_S = aliqIcmsVendaAtaSimpNacional_S.Replace(",", ".");
            aliqIcmsSTVendaAtaSimpNacional_S = aliqIcmsSTVendaAtaSimpNacional_S.Replace(",", ".");
            redBaseCalcIcmsVendaAtaSimpNacional_S = redBaseCalcIcmsVendaAtaSimpNacional_S.Replace(",", ".");
            redBaseCalcIcmsSTVendaAtaSimpNacional_S = redBaseCalcIcmsSTVendaAtaSimpNacional_S.Replace(",", ".");
            aliqIcmsVendaVarejoCont_S = aliqIcmsVendaVarejoCont_S.Replace(",", ".");
            aliqIcmsSTVendaVarejo_Cont_S = aliqIcmsSTVendaVarejo_Cont_S.Replace(",", ".");
            redBaseCalcVendaVarejoCont_S = redBaseCalcVendaVarejoCont_S.Replace(",", ".");
            RedBaseCalcSTVendaVarejo_Cont_S = RedBaseCalcSTVendaVarejo_Cont_S.Replace(",", ".");
            aliqIcmsVendaVarejoConsFinal_S = aliqIcmsVendaVarejoConsFinal_S.Replace(",", ".");
            aliqIcmsSTVendaVarejoConsFinal_S = aliqIcmsSTVendaVarejoConsFinal_S.Replace(",", ".");
            redBaseCalcIcmsVendaVarejoConsFinal_S = redBaseCalcIcmsVendaVarejoConsFinal_S.Replace(",", ".");
            redBaseCalcIcmsSTVendaVarejoConsFinal_S = redBaseCalcIcmsSTVendaVarejoConsFinal_S.Replace(",", ".");

            aliqIcmsCompDeInd_S = aliqIcmsCompDeInd_S.Replace(",", ".");
            aliqIcmsSTCompDeInd_S = aliqIcmsSTCompDeInd_S.Replace(",", ".");
            redBaseCalcIcmsCompraDeInd_S = redBaseCalcIcmsCompraDeInd_S.Replace(",", ".");
            redBaseCalcIcmsSTCompraDeInd_S = redBaseCalcIcmsSTCompraDeInd_S.Replace(",", ".");
            aliqIcmsNFE_S = aliqIcmsNFE_S.Replace(",", ".");
            redBaseCalcIcmsCompraDeAta_S = redBaseCalcIcmsCompraDeAta_S.Replace(",", ".");
            redBaseCalcIcmsSTCompraDeAta_S = redBaseCalcIcmsSTCompraDeAta_S.Replace(",", ".");
            aliqIcmsCompradeAta_S = aliqIcmsCompradeAta_S.Replace(",", ".");
            aliqIcmsSTCompraDeAta_S = aliqIcmsSTCompraDeAta_S.Replace(",", ".");
            aliqIcmsNfeAta_S = aliqIcmsNfeAta_S.Replace(",", ".");
            aliqIcmsCompradeSimpNacional_S = aliqIcmsCompradeSimpNacional_S.Replace(",", ".");

            redBaseCalcIcmsCompradeSimpNacional_S = redBaseCalcIcmsCompradeSimpNacional_S.Replace(",", ".");

            redBaseCalcIcmsSTCompradeSimpNacional_S = redBaseCalcIcmsSTCompradeSimpNacional_S.Replace(",", ".");
            aliqIcmsNfeSN_S = aliqIcmsNfeSN_S.Replace(",", ".");
         




            valorMVAAtacado_S = valorMVAAtacado_S.Replace(",", ".");


            


            if (cstEntradaPisCofins != "")
            {
                model.cstEntradaPisCofins = (from a in db.CstPisCofinsEntradas where a.descricao == cstEntradaPisCofins select a.codigo).FirstOrDefault();

            }
            if (cstSaidaPisCofins != "")
            {
                model.cstSaidaPisCofins = (from a in db.CstPisCofinsSaidas where a.descricao == cstSaidaPisCofins select a.codigo).FirstOrDefault();

            }
            if (cstVendaAtaCont != "")
            {
                model.cstVendaAtaCont = (from a in db.CstIcmsGerais where a.descricao == cstVendaAtaCont select a.codigo).FirstOrDefault();
            }
            if (cstVendaAtaSimpNacional != "")
            {
                model.cstVendaAtaSimpNacional = (from a in db.CstIcmsGerais where a.descricao == cstVendaAtaSimpNacional select a.codigo).FirstOrDefault();
            }
            if (cstVendaVarejoCont != "")
            {
                model.cstVendaVarejoCont = (from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoCont select a.codigo).FirstOrDefault();
            }
            if (cstVendaVarejoConsFinal != "")
            {
                model.cstVendaVarejoConsFinal = (from a in db.CstIcmsGerais where a.descricao == cstVendaVarejoConsFinal select a.codigo).FirstOrDefault();
            }
            if (cstCompraDeInd != "")
            {
                model.cstCompraDeInd = (from a in db.CstIcmsGerais where a.descricao == cstCompraDeInd select a.codigo).FirstOrDefault();
            }
            if (cstdaNfedaIndFORN != "")
            {
                model.cstdaNfedaIndFORN = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            }
            if (cstCompradeAta != "")
            {
                model.cstCompradeAta = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedaIndFORN select a.codigo).FirstOrDefault();
            }
            if (cstdaNfedeAtaFORn != "")
            {
                model.cstdaNfedeAtaFORn = (from a in db.CstIcmsGerais where a.descricao == cstdaNfedeAtaFORn select a.codigo).FirstOrDefault();
            }
            if (cstCompradeSimpNacional != "")
            {
                model.cstCompradeSimpNacional = (from a in db.CstIcmsGerais where a.descricao == cstCompradeSimpNacional select a.codigo).FirstOrDefault();
            }
            if (CsosntdaNfedoSnFOR != "")
            {
                model.CsosntdaNfedoSnFOR = (from a in db.CstIcmsGerais where a.descricao == CsosntdaNfedoSnFOR select a.codigo).FirstOrDefault();
            }

            /*Instanciar objeto*/


            Tributacao tributacao = new Tributacao();


            tributacao = db.Tributacoes.Find(model.id); //busca a tributação pelo seu id

            //var tributacao = db.Tributacoes.Find(model.id);

            /*Atribuir data local para alteração*/
            model.dataAlt = DateTime.Now;

            /*Verificar se o objeto está nulo*/
            if (tributacao == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /*Atribuir valores ao objeto para alterações*/
           
            tributacao.idSetor = model.idSetor;
            
           
            tributacao.codNatReceita = model.codNatReceita;

            // tributacao.fecp = model.fecp;
            if(fecpString != "")
            {
                tributacao.fecp = Decimal.Parse(fecpString, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                if (model.fecp.Equals(null) && tributacao.fecp != null) 
                {
                    tributacao.fecp = null;
                }
            }
          

            /*Verificar estados de CSTs*/
            if (model.cstEntradaPisCofins != null)
            {
                tributacao.cstEntradaPisCofins = model.cstEntradaPisCofins;
            }

            if (model.cstSaidaPisCofins != null)
            {
                tributacao.cstSaidaPisCofins = model.cstSaidaPisCofins;
            }

            if (model.cstVendaAtaCont != null)
            {
                tributacao.cstVendaAtaCont = model.cstVendaAtaCont;
            }

            if (model.cstVendaAtaSimpNacional != null)
            {
                tributacao.cstVendaAtaSimpNacional = model.cstVendaAtaSimpNacional;
            }

            if (model.cstVendaVarejoCont != null)
            {
                tributacao.cstVendaVarejoCont = model.cstVendaVarejoCont;
            }

            if (model.cstVendaVarejoConsFinal != null)
            {
                tributacao.cstVendaVarejoConsFinal = model.cstVendaVarejoConsFinal;
            }

            if (model.cstCompraDeInd != null)
            {
                tributacao.cstCompraDeInd = model.cstCompraDeInd;
            }

            if (model.cstdaNfedaIndFORN != null)
            {
                tributacao.cstdaNfedaIndFORN = model.cstdaNfedaIndFORN;
            }

            if (model.cstCompradeAta != null)
            {
                tributacao.cstCompradeAta = model.cstCompradeAta;
            }

            if (model.cstdaNfedeAtaFORn != null)
            {
                tributacao.cstdaNfedeAtaFORn = model.cstdaNfedeAtaFORn;
            }

            if (model.cstCompradeSimpNacional != null)
            {
                tributacao.cstCompradeSimpNacional = model.cstCompradeSimpNacional;
            }

            if (model.CsosntdaNfedoSnFOR != null)
            {
                tributacao.CsosntdaNfedoSnFOR = model.CsosntdaNfedoSnFOR;
            }

            /*pis cofins*/
            if (aliqEntPis_S != "")
            {
                tributacao.aliqEntPis = Decimal.Parse(aliqEntPis_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (aliqSaidaPis_S != "")
            {
                tributacao.aliqSaidaPis = Decimal.Parse(aliqSaidaPis_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (aliqEntCofins_S != "")
            {
                tributacao.aliqEntCofins = Decimal.Parse(aliqEntCofins_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (aliqSaidaCofins_S != "")
            {
                tributacao.aliqSaidaCofins = Decimal.Parse(aliqSaidaCofins_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            tributacao.idFundamentoLegal = model.idFundamentoLegal;
            /*Venda atacado para contribuinte*/
            if (aliqIcmsVendaAtaCont_S != "")
            {
                tributacao.aliqIcmsVendaAtaCont = Decimal.Parse(aliqIcmsVendaAtaCont_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (aliqIcmsSTVendaAtaCont_S != "")
            {
                tributacao.aliqIcmsSTVendaAtaCont = Decimal.Parse(aliqIcmsSTVendaAtaCont_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (redBaseCalcIcmsVendaAtaCont_S != "")
            {
                tributacao.redBaseCalcIcmsVendaAtaCont = Decimal.Parse(redBaseCalcIcmsVendaAtaCont_S, System.Globalization.CultureInfo.InvariantCulture);
            }
            if (redBaseCalcIcmsSTVendaAtaCont_S != "")
            {
                tributacao.redBaseCalcIcmsSTVendaAtaCont = Decimal.Parse(redBaseCalcIcmsSTVendaAtaCont_S, System.Globalization.CultureInfo.InvariantCulture);
            }

            /*Venda atacado para simples nacional*/


            if (aliqIcmsVendaAtaSimpNacional_S != "")
            {
                tributacao.aliqIcmsVendaAtaSimpNacional = Decimal.Parse(aliqIcmsVendaAtaSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (aliqIcmsSTVendaAtaSimpNacional_S != "")
            {
                tributacao.aliqIcmsSTVendaAtaSimpNacional = Decimal.Parse(aliqIcmsSTVendaAtaSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (redBaseCalcIcmsVendaAtaSimpNacional_S != "")
            {
                tributacao.redBaseCalcIcmsVendaAtaSimpNacional = Decimal.Parse(redBaseCalcIcmsVendaAtaSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (redBaseCalcIcmsSTVendaAtaSimpNacional_S != "")
            {
                tributacao.redBaseCalcIcmsSTVendaAtaSimpNacional = Decimal.Parse(redBaseCalcIcmsSTVendaAtaSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            /*Venda varejo para contribuinte*/

            if (aliqIcmsVendaVarejoCont_S != "")
            {
                tributacao.aliqIcmsVendaVarejoCont = Decimal.Parse(aliqIcmsVendaVarejoCont_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (aliqIcmsSTVendaVarejo_Cont_S != "")
            {
                tributacao.aliqIcmsSTVendaVarejo_Cont = Decimal.Parse(aliqIcmsSTVendaVarejo_Cont_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (redBaseCalcVendaVarejoCont_S != "")
            {
                tributacao.redBaseCalcVendaVarejoCont = Decimal.Parse(redBaseCalcVendaVarejoCont_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (RedBaseCalcSTVendaVarejo_Cont_S != "")
            {
                tributacao.RedBaseCalcSTVendaVarejo_Cont = Decimal.Parse(RedBaseCalcSTVendaVarejo_Cont_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            /*Venda varejo para consumidor final*/

            if (aliqIcmsVendaVarejoConsFinal_S != "")
            {
                tributacao.aliqIcmsVendaVarejoConsFinal = Decimal.Parse(aliqIcmsVendaVarejoConsFinal_S, System.Globalization.CultureInfo.InvariantCulture);

            }


            if (aliqIcmsSTVendaVarejoConsFinal_S != "")
            {
                tributacao.aliqIcmsSTVendaVarejoConsFinal = Decimal.Parse(aliqIcmsSTVendaVarejoConsFinal_S, System.Globalization.CultureInfo.InvariantCulture);

            }


            if (redBaseCalcIcmsVendaVarejoConsFinal_S != "")
            {
                tributacao.redBaseCalcIcmsVendaVarejoConsFinal = Decimal.Parse(redBaseCalcIcmsVendaVarejoConsFinal_S, System.Globalization.CultureInfo.InvariantCulture);

            }


            if (redBaseCalcIcmsSTVendaVarejoConsFinal_S != "")
            {
                tributacao.redBaseCalcIcmsSTVendaVarejoConsFinal = Decimal.Parse(redBaseCalcIcmsSTVendaVarejoConsFinal_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            /*Fundamento legal vendas icms*/
            tributacao.idFundLegalSaidaICMS = model.idFundLegalSaidaICMS;

            /*Compra de industria*/

            if (aliqIcmsCompDeInd_S != "")
            {
                tributacao.aliqIcmsCompDeInd = Decimal.Parse(aliqIcmsCompDeInd_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (aliqIcmsSTCompDeInd_S != "")
            {
                tributacao.aliqIcmsSTCompDeInd = Decimal.Parse(aliqIcmsSTCompDeInd_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (redBaseCalcIcmsCompraDeInd_S != "")
            {
                tributacao.redBaseCalcIcmsCompraDeInd = Decimal.Parse(redBaseCalcIcmsCompraDeInd_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (redBaseCalcIcmsSTCompraDeInd_S != "")
            {
                tributacao.redBaseCalcIcmsSTCompraDeInd = Decimal.Parse(redBaseCalcIcmsSTCompraDeInd_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (aliqIcmsNFE_S != "")
            {
                tributacao.aliqIcmsNFE = Decimal.Parse(aliqIcmsNFE_S, System.Globalization.CultureInfo.InvariantCulture);

            }



            /*Compra de atacado*/

            if (redBaseCalcIcmsCompraDeAta_S != "")
            {
                tributacao.redBaseCalcIcmsCompraDeAta = Decimal.Parse(redBaseCalcIcmsCompraDeAta_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (redBaseCalcIcmsSTCompraDeAta_S != "")
            {
                tributacao.redBaseCalcIcmsSTCompraDeAta = Decimal.Parse(redBaseCalcIcmsSTCompraDeAta_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (aliqIcmsCompradeAta_S != "")
            {
                tributacao.aliqIcmsCompradeAta = Decimal.Parse(aliqIcmsCompradeAta_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (aliqIcmsSTCompraDeAta_S != "")
            {
                tributacao.aliqIcmsSTCompraDeAta = Decimal.Parse(aliqIcmsSTCompraDeAta_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (aliqIcmsNfeAta_S != "")
            {
                tributacao.aliqIcmsNfeAta = Decimal.Parse(aliqIcmsNfeAta_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            /*Compra de simples nacional*/


            if (aliqIcmsCompradeSimpNacional_S != "")
            {
                tributacao.aliqIcmsCompradeSimpNacional = Decimal.Parse(aliqIcmsCompradeSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }

            if (aliqIcmsSTCompradeSimpNacional_S != "")
            {
                tributacao.aliqIcmsSTCompradeSimpNacional = Decimal.Parse(aliqIcmsSTCompradeSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (redBaseCalcIcmsCompradeSimpNacional_S != "")
            {
                tributacao.redBaseCalcIcmsCompradeSimpNacional = Decimal.Parse(redBaseCalcIcmsCompradeSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (redBaseCalcIcmsSTCompradeSimpNacional_S != "")
            {
                tributacao.redBaseCalcIcmsSTCompradeSimpNacional = Decimal.Parse(redBaseCalcIcmsSTCompradeSimpNacional_S, System.Globalization.CultureInfo.InvariantCulture);

            }
            if (valorMVAInd_S != "")
            {
                tributacao.valorMVAInd = Decimal.Parse(valorMVAInd_S, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (valorMVAAtacado_S != "")
            {
                tributacao.valorMVAAtacado = Decimal.Parse(valorMVAAtacado_S, System.Globalization.CultureInfo.InvariantCulture);
            }







            /*Fundamento legal entrada(compras) icms*/
            tributacao.idFundLelgalEntradaICMS = model.idFundLelgalEntradaICMS;
            
            /*Outros atributos*/
            tributacao.regime2560 = model.regime2560;
            tributacao.tipoMVA = model.tipoMVA;
           
            tributacao.inicioVigenciaMVA = model.inicioVigenciaMVA;
           
            tributacao.fimVigenciaMVA = model.fimVigenciaMVA;
            tributacao.creditoOutorgado = model.creditoOutorgado;
            tributacao.dataAlt = model.dataAlt;

            try
            {
             
                db.SaveChanges();
                TempData["tributacaoMTX"] = null;

            }
            catch (Exception e)
            {
                string ex = e.GetType().ToString();

                return RedirectToAction("Index");

            }

            return RedirectToAction("Index");

        }

        ////delte

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
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tributacao tributacao = db.Tributacoes.Find(id);
            if (tributacao == null)
            {
                return HttpNotFound();
            }



            /*Verificando cst na tabela e Preencher a descrição dos csts
            Obs.: Quando o registro não possui algun desses valores a
            consulta ao banco retorna null, dando exceção por estar nulo
            dessa forma ele valida na condição e não permite valores
            nulos sejam tratados
            */
            ViewBag.CstVendaAtaContDescricao = (db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaAtaCont).descricao;
            ViewBag.CstVendaSimpNacionalDescricao = (db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaAtaSimpNacional).descricao;
            ViewBag.CstVendaVarCont = (db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaVarejoCont).descricao;
            ViewBag.CstVendaVarejoCF = (db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstVendaVarejoConsFinal).descricao;
            ViewBag.CstCompraDeInd = (db.CstIcmsGerais.Find(tributacao.cstCompraDeInd) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompraDeInd).descricao;
            ViewBag.CstDaNfeIndForn = (db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstdaNfedaIndFORN).descricao;
            ViewBag.CstCompraATA = (db.CstIcmsGerais.Find(tributacao.cstCompradeAta) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompradeAta).descricao;
            ViewBag.CstDaNfeAtaForn = (db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstdaNfedeAtaFORn).descricao;
            ViewBag.CstCompraSimNacional = (db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional) == null) ? "" : db.CstIcmsGerais.Find(tributacao.cstCompradeSimpNacional).descricao;
            ViewBag.CstNfeSNfor = (db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR) == null) ? "" : db.CstIcmsGerais.Find(tributacao.CsosntdaNfedoSnFOR).descricao;



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

            ViewBag.DataAlt = DateTime.Now;

            return View(tributacao);

        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tributacao tributacao = db.Tributacoes.Find(id);
            db.Tributacoes.Remove(tributacao);
            db.SaveChanges();
            TempData["tributacaoMTX"] = null;
            return RedirectToAction("Index");
        }

        public JsonResult AutoComplete()
        {
            //Carregar lista autocomplete
            List<Produto> produtos = db.Produtos.ToList();
            List<Object> resultado = new List<object>();


            produtos.ForEach(x => {
                resultado.Add(x.descricao);
          
            });

           // var prod = from s in db.Produtos select s.descricao; //variavel carregado de produtos


            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        //DASHBOARD
        [HttpGet]
        public ActionResult GraficoIcmsEntrada(string ufOrigem, string ufDestino)
        {
            
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;
            //Aplica a origem e destino selecionada
            this.tribMTX = this.lstCli.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino).ToList();

            ///*Aliquota ICMS Compra industria*/
            //ViewBag.AliqICMSEntradaInd = this.lstCli.Count(a => a.ALIQ_ICMS_COMP_DE_IND != null); //tirar o cst = 60 o cst 40 e a categoria 21);
            //ViewBag.AliqICMSEntradaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_COMP_DE_IND == null);



            ViewBag.AliqICMSEntradaInd = this.lstCli.Count(  a=>a.ALIQ_ICMS_COMP_DE_IND !=null && a.CST_COMPRA_DE_IND != 60 && a.CST_COMPRA_DE_IND != 40 && a.CST_COMPRA_DE_IND != 41 && a.ID_CATEGORIA != 21  ); //tirar o cst = 60 o cst 40 O CST 41 e a categoria 21);
            ViewBag.AliqICMSEntradaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_COMP_DE_IND == null && a.CST_COMPRA_DE_IND != 60 && a.CST_COMPRA_DE_IND != 40 && a.CST_COMPRA_DE_IND != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSEntradaIndIsenta = this.lstCli.Count(a => a.CST_COMPRA_DE_IND == 40 && a.ID_CATEGORIA != 21  );



            /*Aliquota ICMS st Compra industria*/
            ViewBag.AliqICMSSTEntradaInd   = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND != null && a.CST_COMPRA_DE_IND == 60 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSSTEntradaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMP_DE_IND == null && a.CST_COMPRA_DE_IND == 60 && a.ID_CATEGORIA != 21  );




            /*Aliquota ICMS compra de atacado*/
            ViewBag.AliqICMSCompraAta = this.lstCli.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA      != null && a.CST_COMPRA_DE_ATA != 60 && a.CST_COMPRA_DE_ATA != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSCompraAtaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_COMPRA_DE_ATA == null && a.CST_COMPRA_DE_ATA != 60 && a.CST_COMPRA_DE_ATA != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSCompraAtaIsenta = this.lstCli.Count(a => a.CST_COMPRA_DE_ATA == 40 && a.ID_CATEGORIA != 21  );


            /*Aliquota ICMS ST compra de atacado*/
            ViewBag.AliqICMSSTCompraAta      = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && a.CST_COMPRA_DE_ATA == 60 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSSTCompraAtaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_ATA == null && a.CST_COMPRA_DE_ATA == 60 && a.ID_CATEGORIA != 21  );



            /*Aliquota ICMS compra de Simples nacional*/
            ViewBag.AliqICMSCompraSN      = this.lstCli.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.CST_COMPRA_DE_SIMP_NACIONAL != 60 && a.CST_COMPRA_DE_SIMP_NACIONAL != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSCompraSNNulla = this.lstCli.Count(a => a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null && a.CST_COMPRA_DE_SIMP_NACIONAL != 60 && a.CST_COMPRA_DE_SIMP_NACIONAL != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSCompraSNIsenta = this.lstCli.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL == 40 && a.ID_CATEGORIA != 21  );

            /*Aliquota ICMS ST compra de Simples nacional*/
            ViewBag.AliqICMSSTCompraSN = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL      != null && a.CST_COMPRA_DE_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSSTCompraSNNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && a.CST_COMPRA_DE_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21  );


            /*Aliquota ICMS NFE Compra Industria*/
            ViewBag.AliqICMSNFEInd = this.lstCli.Count(a => a.ALIQ_ICMS_NFE      != null && a.CST_DA_NFE_DA_IND_FORN != 60 && a.CST_DA_NFE_DA_IND_FORN != 40 && a.CST_COMPRA_DE_ATA != 41  && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNfeIndNulla = this.lstCli.Count(a => a.ALIQ_ICMS_NFE == null && a.CST_DA_NFE_DA_IND_FORN != 60 && a.CST_DA_NFE_DA_IND_FORN != 40 && a.CST_COMPRA_DE_ATA != 41  && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNfeIndIsenta = this.lstCli.Count(a => a.CST_DA_NFE_DA_IND_FORN == 40 && a.ID_CATEGORIA != 21  );




            /*Aliquota ICMS NFE Compra Ata*/
            ViewBag.AliqICMSNFEAta = this.lstCli.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA      != null && a.CST_DA_NFE_DE_ATA_FORN != 60 && a.CST_DA_NFE_DE_ATA_FORN != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNFEAtaNulla = this.lstCli.Count(a => a.ALIQ_ICMS_NFE_FOR_ATA == null && a.CST_DA_NFE_DE_ATA_FORN != 60 && a.CST_DA_NFE_DE_ATA_FORN != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNFEAtaIsenta = this.lstCli.Count(a => a.CST_DA_NFE_DE_ATA_FORN == 40 && a.ID_CATEGORIA != 21  );


            /*Aliquota ICMS NFE Compra SN*/
            ViewBag.AliqICMSNFESN = this.lstCli.Count(a => a.ALIQ_ICMS_NFE_FOR_SN != null      && a.CSOSNTDANFEDOSNFOR != 60 && a.CSOSNTDANFEDOSNFOR != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNFESNNulla = this.lstCli.Count(a => a.ALIQ_ICMS_NFE_FOR_SN == null && a.CSOSNTDANFEDOSNFOR != 60 && a.CSOSNTDANFEDOSNFOR != 40 && a.CST_COMPRA_DE_ATA != 41 && a.ID_CATEGORIA != 21  );
            ViewBag.AliqICMSNFESNIsenta = this.lstCli.Count(a => a.CSOSNTDANFEDOSNFOR == 40 && a.ID_CATEGORIA != 21  );


            return View();
        }

        [HttpGet]
        public ActionResult GraficoIcmsSaida(string ufOrigem, string ufDestino, string tributacao) //se for simples nacional vem descrito nessa variavel
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            ViewBag.Tributacao = TempData["tributacao"].ToString();

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //se for simples nacional direciona para essa action
            if (ViewBag.Tributacao == "SIMPLES")
            {
                
                return RedirectToAction("GraficoIcmsSaidaSN", new { tributacao = tributacao });
            }
           

           
                //verifica carregamento da tabela
                VerificaTempData();

                //Aplica a origem e destino selecionada
                this.lstCli = this.lstCli.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


                /*Aliquota ICMS Venda Varejo Consumidor Final - atualizado 17/01/2022 - estados origem e destino*/
                //ViewBag.AliqICMSVendaVarCF = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino); //tirar o cst = 60
                //ViewBag.AliqICMSVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino); //tirar o cst = 60
                //ViewBag.AliqICMSVendaVarCFIsenta = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                //ViewBag.AliqICMSVendaVarCFUsoConsumo = this.lstCli.Count(a => a.ID_CATEGORIA == 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

                ViewBag.AliqICMSVendaVarCF = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.CST_VENDA_VAREJO_CONS_FINAL != 41 && a.ID_CATEGORIA != 21); //tirar o cst = 60

                ViewBag.AliqICMSVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.CST_VENDA_VAREJO_CONS_FINAL != 41 && a.ID_CATEGORIA != 21); //tirar o cst = 60
                ViewBag.AliqICMSVendaVarCFIsenta = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 40 && a.ID_CATEGORIA != 21);
                ViewBag.AliqICMSVendaVarCFUsoConsumo = this.lstCli.Count(a => a.ID_CATEGORIA == 21);
                ViewBag.AliqICMSVendaVarCFNaoTributada = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 41 && a.ID_CATEGORIA != 21);




                /*Aliquota ICMS ST Venda Varejo Consumidor Final*/ /*Alteração feita para filtrar por CST = 60*/
                //ViewBag.AliqICMSSTVendaVarCF      = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                //ViewBag.AliqICMSSTVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

                ViewBag.AliqICMSSTVendaVarCF = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);
                ViewBag.AliqICMSSTVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);




                /*Aliquota ICMS Venda Varejo Para Contribuinte atualizado 18/01/2022 - estados origem e destino**/
                ViewBag.AliqICMSVendaVarCont = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT != 60 && a.CST_VENDA_VAREJO_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaVarContNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT != 60 && a.CST_VENDA_VAREJO_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaVarContIsenta = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONT == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);


                /*Aliquota ICMS ST Venda Varejo Para Contribuinte*/
                ViewBag.AliqICMSSTVendaVarCont = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSSTVendaVarContNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



                /*Aliquota ICMS Venda Ata Para Contribuinte atualizado 18/01/2022 - estados origem e destino**/
                ViewBag.AliqICMSVendaAtaCont = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT != 60 && a.CST_VENDA_ATA_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaAtaContNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT != 60 && a.CST_VENDA_ATA_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaAtaContIsenta = this.lstCli.Count(a => a.CST_VENDA_ATA_CONT == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



                /*Aliquota ICMS ST Venda Ata Para Contribuinte  atualizado 18/01/2022 - estados origem e destino**/
                ViewBag.AliqICMSSTVendaAtaCont = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSSTVendaAtaContNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



                /*Aliquota ICMS Venda Ata Para Simples Nacional atualizado 18/01/2022 - estados origem e destino**/
                ViewBag.AliqICMSVendaAtaSN = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaAtaNSNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSVendaAtaSNIsenta = this.lstCli.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

                /*Aliquota ICMS ST Venda Ata Para Simples Nacional atualizado 18/01/2022 - estados origem e destino**/
                ViewBag.AliqICMSSTVendaAtaSN = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
                ViewBag.AliqICMSSTVendaAtaNSNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);


                return View();

            

            
        }


        [HttpGet]
        public ActionResult GraficoIcmsSaidaSN(string ufOrigem, string ufDestino, string tributacao) //se for simples nacional vem descrito nessa variavel
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            ViewBag.Tributacao = TempData["tributacao"].ToString();

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //se for simples nacional direciona para essa action
            if (ViewBag.Tributacao == "OUTROS")
            {
                
                    return RedirectToAction("GraficoIcmsSaida", new { tributacao = tributacao });
                
               
            }


            //verifica carregamento da tabela
            VerificaTempDataSN();

            //Aplica a origem e destino selecionada
            this.lstCliSN = this.lstCliSN.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


            /*Aliquota ICMS Venda Varejo Consumidor Final - atualizado 17/01/2022 - estados origem e destino*/
            //ViewBag.AliqICMSVendaVarCF = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino); //tirar o cst = 60
            //ViewBag.AliqICMSVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino); //tirar o cst = 60
            //ViewBag.AliqICMSVendaVarCFIsenta = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            //ViewBag.AliqICMSVendaVarCFUsoConsumo = this.lstCli.Count(a => a.ID_CATEGORIA == 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

            ViewBag.AliqICMSVendaVarCF = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.CST_VENDA_VAREJO_CONS_FINAL != 41 && a.ID_CATEGORIA != 21); //tirar o cst = 60

            ViewBag.AliqICMSVendaVarCFNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL != 60 && a.CST_VENDA_VAREJO_CONS_FINAL != 40 && a.CST_VENDA_VAREJO_CONS_FINAL != 41 && a.ID_CATEGORIA != 21); //tirar o cst = 60
            ViewBag.AliqICMSVendaVarCFIsenta = this.lstCliSN.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 40 && a.ID_CATEGORIA != 21);
            ViewBag.AliqICMSVendaVarCFUsoConsumo = this.lstCliSN.Count(a => a.ID_CATEGORIA == 21);
            ViewBag.AliqICMSVendaVarCFNaoTributada = this.lstCliSN.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == 41 && a.ID_CATEGORIA != 21);




            /*Aliquota ICMS ST Venda Varejo Consumidor Final*/ /*Alteração feita para filtrar por CST = 60*/
            //ViewBag.AliqICMSSTVendaVarCF      = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            //ViewBag.AliqICMSSTVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

            ViewBag.AliqICMSSTVendaVarCF = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);
            ViewBag.AliqICMSSTVendaVarCFNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);




            /*Aliquota ICMS Venda Varejo Para Contribuinte atualizado 18/01/2022 - estados origem e destino**/
            ViewBag.AliqICMSVendaVarCont = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT != 60 && a.CST_VENDA_VAREJO_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaVarContNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT != 60 && a.CST_VENDA_VAREJO_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaVarContIsenta = this.lstCliSN.Count(a => a.CST_VENDA_VAREJO_CONT == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);


            /*Aliquota ICMS ST Venda Varejo Para Contribuinte*/
            ViewBag.AliqICMSSTVendaVarCont = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSSTVendaVarContNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



            /*Aliquota ICMS Venda Ata Para Contribuinte atualizado 18/01/2022 - estados origem e destino**/
            ViewBag.AliqICMSVendaAtaCont = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT != 60 && a.CST_VENDA_ATA_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaAtaContNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT != 60 && a.CST_VENDA_ATA_CONT != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaAtaContIsenta = this.lstCliSN.Count(a => a.CST_VENDA_ATA_CONT == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



            /*Aliquota ICMS ST Venda Ata Para Contribuinte  atualizado 18/01/2022 - estados origem e destino**/
            ViewBag.AliqICMSSTVendaAtaCont = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSSTVendaAtaContNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);



            /*Aliquota ICMS Venda Ata Para Simples Nacional atualizado 18/01/2022 - estados origem e destino**/
            ViewBag.AliqICMSVendaAtaSN = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaAtaNSNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL != 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSVendaAtaSNIsenta = this.lstCliSN.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == 40 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);

            /*Aliquota ICMS ST Venda Ata Para Simples Nacional atualizado 18/01/2022 - estados origem e destino**/
            ViewBag.AliqICMSSTVendaAtaSN = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);
            ViewBag.AliqICMSSTVendaAtaNSNulla = this.lstCliSN.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.ID_CATEGORIA != 21 && a.UF_ORIGEM == this.ufOrigem && a.UF_DESTINO == this.ufDestino);


            return View();

        }

        [HttpGet]
        public ActionResult GfRedBCalcIcmsEntrada(string ufOrigem, string ufDestino)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //chmar action auxiliar para verificar e carregar a tempdata com a lista
            VerificaTempData();


            /*Redução base calc icms compra de industria*/
            ViewBag.RedBasCalIcmsCompInd     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && a.CST_COMPRA_DE_IND == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsCompIndNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null && a.CST_COMPRA_DE_IND == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
         
            /*Redução base calc icms ST compra de industria*/
            ViewBag.RedBasCalIcmsSTCompInd     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && a.CST_COMPRA_DE_IND == 60  && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTCompIndNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null && a.CST_COMPRA_DE_IND == 60  && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
          
            /*Redução base calc icms compra de atacado*/
            ViewBag.RedBasCalIcmsCompAta     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && a.CST_COMPRA_DE_ATA == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsCompAtaNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null && a.CST_COMPRA_DE_ATA == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Redução base calc icms st compra de atacado*/
            ViewBag.RedBasCalIcmsSTCompAta     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && a.CST_COMPRA_DE_ATA == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTCompAtaNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null && a.CST_COMPRA_DE_ATA == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Redução base calc icms compra de  SIMPLES NACIONAL*/
            ViewBag.RedBasCalIcmsCompSN     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL != null && a.CST_COMPRA_DE_SIMP_NACIONAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsCompSNNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL == null && a.CST_COMPRA_DE_SIMP_NACIONAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Redução base calc icms compra de  SIMPLES NACIONAL*/
            ViewBag.RedBasCalIcmsSTCompSN     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && a.CST_COMPRA_DE_SIMP_NACIONAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTCompSNNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && a.CST_COMPRA_DE_SIMP_NACIONAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            return View();
        }

        [HttpGet]
        public ActionResult GfRedBCalcIcmsSaida(string ufOrigem, string ufDestino)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            /*Redução base calc icms venda varejo cf*/
            ViewBag.RedBasCalIcmsVendaVarCF        = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsVendaVarCFNull    = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Redução base calc icms ST venda varejo cf*/
            ViewBag.RedBasCalIcmsSTVendaVarCF     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTVendaVarCFNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Redução base de calculo ICMS venda varejo para Contribuinte*/
            ViewBag.RedBasCalIcmsVendaVarCont     = this.lstCli.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsVendaVarContNull = this.lstCli.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Redução base de calculo ICMS ST venda varejo para Contribuinte*/
            ViewBag.RedBasCalIcmsSTVendaVarCont     = this.lstCli.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTVendaVarContNull = this.lstCli.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            
            /*Red Base Calc ICMS Venda Atacado para Contribuinte*/
            ViewBag.RedBasCalIcmsVendaAtaCont     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsVendaAtaContNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Red Base Calc ICMS ST Venda Atacado para Contribuinte*/
            ViewBag.RedBasCalIcmsSTVendaAtaCont =     this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT != null && a.CST_VENDA_ATA_CONT == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTVendaAtaContNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT == null && a.CST_VENDA_ATA_CONT == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Red Base Calc ICMS Venda Atacado para Simples Nacional*/
            ViewBag.RedBasCalIcmsVendaAtaSN     = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsVendaAtaSNNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Red Base Calc ICMS ST Venda Atacado para Simples Nacional*/
            ViewBag.RedBasCalIcmsSTVendaAtaSN = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBasCalIcmsSTVendaAtaSNNull = this.lstCli.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            return View();
        }
        [HttpGet]
        public ActionResult GraficoAliPisCofins(string ufOrigem, string ufDestino, string tributacao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            VerificaTributacao(tributacao); //verificar se a tributação escolhida está ativa

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            ViewBag.Tributacao = TempData["tributacao"].ToString();

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            /*Aliq Entrada Pis*/
            ViewBag.AliEntradaPis     = this.lstCli.Count(a => a.ALIQ_ENT_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliEntradaPisNull = this.lstCli.Count(a => a.ALIQ_ENT_PIS == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Aliq Saída Pis*/
            ViewBag.AliSaidaPis     = this.lstCli.Count(a => a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliSaidaPisNull = this.lstCli.Count(a => a.ALIQ_SAIDA_PIS == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Ali Entrada Cofins*/
            ViewBag.AliEntradaCofins     = this.lstCli.Count(a => a.ALIQ_ENT_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliEntradaCofinsNull = this.lstCli.Count(a => a.ALIQ_ENT_COFINS == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*Ali Saída Cofins*/
            ViewBag.AliSaidaCofins     = this.lstCli.Count(a => a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliSaidaCofinsNull = this.lstCli.Count(a => a.ALIQ_SAIDA_COFINS == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            return View();
        }
        [HttpGet]
        public ActionResult GraficoCstEntrada(string ufOrigem, string ufDestino) 
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            /*CST Entrada Pis/cofins */
            ViewBag.CstEntradaPisCofins     = this.lstCli.Count(a => a.CST_ENTRADA_PISCOFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstEntradaPisCofinsNull = this.lstCli.Count(a => a.CST_ENTRADA_PISCOFINS == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST Compra de industria */
            ViewBag.CstCompraInd     = this.lstCli.Count(a => a.CST_COMPRA_DE_IND != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstCompraIndNull = this.lstCli.Count(a => a.CST_COMPRA_DE_IND == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST Compra de Atacado */ 
            ViewBag.CstCompraAta     = this.lstCli.Count(a => a.CST_COMPRA_DE_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstCompraAtaNull = this.lstCli.Count(a => a.CST_COMPRA_DE_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST Compra de Simpes Nacional */
            ViewBag.CstCompraSN     = this.lstCli.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstCompraSNNull = this.lstCli.Count(a => a.CST_COMPRA_DE_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*CST NFe Industria*/
            ViewBag.CstNfeInd     = this.lstCli.Count(a => a.CST_DA_NFE_DA_IND_FORN != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstNfeIndNull = this.lstCli.Count(a => a.CST_DA_NFE_DA_IND_FORN == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST NFe Atacado*/
            ViewBag.CstNfeAta     = this.lstCli.Count(a => a.CST_DA_NFE_DE_ATA_FORN != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstNfeAtaNull = this.lstCli.Count(a => a.CST_DA_NFE_DE_ATA_FORN == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST NFe Simples Nacional*/
            ViewBag.CstNfeSN     = this.lstCli.Count(a => a.CSOSNTDANFEDOSNFOR != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstNfeSNNull = this.lstCli.Count(a => a.CSOSNTDANFEDOSNFOR == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            return View();
        }

        [HttpGet]
        public ActionResult GraficoCstSaida(string ufOrigem, string ufDestino) 
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            TempData["procuraPor"] = null; //anulando variavel de procura
            TempData.Keep("procuraPor");
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            /*Cst Pis Cofins Saida*/
            ViewBag.CstSaidaPisCofins     = this.lstCli.Count(a => a.CST_SAIDA_PISCOFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstSaidaPisCofinsNull = this.lstCli.Count(a => a.CST_SAIDA_PISCOFINS == null);


            /*Cst Venda Var Cons Final*/
            ViewBag.CstVendaVarCF     = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarCFNull = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == null);

            /*CST Venda Varejo para Contribuinte*/
            ViewBag.CstVendaVarCont     = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarContNull = this.lstCli.Count(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*CST Venda Atacado para Contribuinte*/
            ViewBag.CstVendaAtaCont     = this.lstCli.Count(a => a.CST_VENDA_ATA_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaContNull = this.lstCli.Count(a => a.CST_VENDA_ATA_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*CST Venda Atacado para Simples nacional*/
            ViewBag.CstVendaAtaSN     = this.lstCli.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaSNNull = this.lstCli.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            return View();


        }//fim da action


        /*Controller para edição em massa CStPisCofinsMassa - VERSÃO FINAL*/
        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassa(
            string origem,
            string destino,
            string opcao, 
            string param, 
            string ordenacao, 
            string qtdNSalvos, 
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM, 
            string procuraCEST, 
            string filtroCorrente, 
            string filtroCorrenteCST, 
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
      
            //Variavel auxiliar para procurar por cst
            //string cst = filtroCorrenteCST ?? procurarCST;

            ////converte em int, caso o valor seja um numero
            //bool cstConvert = int.TryParse(cst, out int cstInt); //verfica se pode ser convertido

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
         

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_SAIDA_PISCOFINS != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_SAIDA_PISCOFINS == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           // this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_SAIDA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstPisCofinsSaidas;



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist



        }//fim EditCstPisCofinsSaidaMassa

        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));


            }
            //ViewBag.Tributos = trib;
            ViewBag.CstSaidaPisCofins = db.CstPisCofinsSaidas;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstPisCofinsSaidaMassaModalPost(string strDados, string cstPCSaida)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstSaidaPisCofins = (int)(from a in db.CstPisCofinsSaidas where a.descricao == cstPCSaida select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstSaidaPisCofins = cstSaidaPisCofins; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }



            //Redirecionar para a tela de graficos
            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstPisCofinsSaidaMassa", new { param = resultado, qtdSalvos = regSalvos });
           

        }

        //Edit em massa Venda Varejo Consumidor final - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaVarCFMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;


            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            //this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;

         

            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }

        [HttpGet]
        public ActionResult EditCstVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));


            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);

        }
        [HttpGet]
        public ActionResult EditCstVendaVarCFMassaModalPost(string strDados, string cstVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenVarCF = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendVarCF select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaVarejoConsFinal = cstVenVarCF; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e) {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit masss venda varejo contribuinte - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaVarContMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                   this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;


            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            // this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
           
            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;



            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstVendaVarContMassaModalPost(string strDados, string cstVendVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenVarCont = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendVarCont select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaVarejoCont = cstVenVarCont; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }





            //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit massa venda atacado contribuinte -   VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaAtaContMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;

            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            // this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = lstCli.Where(s => s.CST_VENDA_ATA_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;



            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

                        
        }

        [HttpGet]
        public ActionResult EditCstVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);

        }

        [HttpGet]
        public ActionResult EditCstVendaAtaContMassaModalPost(string strDados, string cstVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenAtaCont = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendAtaCont select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;
            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaAtaCont = cstVenAtaCont; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstVendaAtaCont = cstVenAtaCont; //novo cst
                db.SaveChanges();
            }
            
           //Redirecionar para a tela de graficos
            return RedirectToAction("EditCstVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }




        //Edit em massa venda atacado para simples nacional - VERSÃO FINAL
        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

    

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            

            ViewBag.CstGeral = db.CstIcmsGerais;

            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

        }

        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassaModal(string strDados, int? origem)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            ViewBag.Origem = origem;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstVendaAtaSNMassaModalPost(string strDados, string cstVendAtaSN, int? origem)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstVenAtaSN = (int)(from a in db.CstIcmsGerais where a.descricao == cstVendAtaSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.cstVendaAtaSimpNacional = cstVenAtaSN; //novo cst
                    db.SaveChanges();
                    regSalvos++;
                }
                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {

                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }

            
            //Redirecionar para a tela de graficos
            //Redirecionar para a tela de graficos
            if(origem == 1)
            {
                return RedirectToAction("EditAliqIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
            }
            else
            {
                return RedirectToAction("EditCstVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
            }

        }


        /* Cst de Entrada */
        //Entrada Pis/cofins
        [HttpGet]
        public ActionResult EditCstPisCofinsEntradaMassa(
             string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_ENTRADA_PISCOFINS != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_ENTRADA_PISCOFINS == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
          //  this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_ENTRADA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstPisCofinsEntradas;



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }//fim action
        [HttpGet]
        public ActionResult EditCstPisCofinsEntradaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstEntradaPisCofins = db.CstPisCofinsEntradas;
            return View(trib);
        }

        [HttpGet]           
        public ActionResult EditCstPisCofinsEntradaMassaModalPost(string strDados, string cstPisCofinsE)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstPCEntrada = (int)(from a in db.CstPisCofinsEntradas where a.descricao == cstPisCofinsE select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstEntradaPisCofins = cstPCEntrada; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra de Industria
        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            //this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view



            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

           

            
        }

        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }
        [HttpGet]
        public ActionResult EditCstCompraIndustriaMassaModalPost(string strDados, string cstCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPind = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraInd select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompraDeInd = cstCPind; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra de atacado
        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);


            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
           

           
        }

        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstCompraAtacadoMassaModalPost(string strDados, string cstCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPata = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraAta select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompradeAta = cstCPata; //novo cst
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Compra SN
        [HttpGet]
        public ActionResult EditCstCompraSNMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

           
            
        }

        [HttpGet]
        public ActionResult EditCstCompraSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstCompraSNMassaModalPost(string strDados, string cstCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstCPSn = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstCompradeSimpNacional = cstCPSn; //novo cst

                
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Nfe Ind
        [HttpGet]
        public ActionResult EditCstNfeIndMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);

        }

        [HttpGet]
        public ActionResult EditCstNfeIndMassaModalPost(string strDados, string cstCompraNfeInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeInd = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeInd select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstdaNfedaIndFORN = cstNfeInd; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Nfe Ata
        public ActionResult EditCstNfeAtaMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditCstNfeAtaMassaModalPost(string strDados, string cstCompraNfeAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeAta = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeAta select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.cstdaNfedeAtaFORn = cstNfeAta; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //NFe SN
        [HttpGet]
        public ActionResult EditCstNfeSNMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procuraSetor,
            string filtroSetor,
            string procuraCate,
            string filtroCate,
            string procurarPor,
            string procuraCST,
            string filtraCST,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteCST,
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


            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

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
            page = (procurarPor != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;



            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com CST":
                    this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR != null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
                case "Sem CST":
                    this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR == null && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIV(codBarrasL, procurarPor, procuraCEST, procuraNCM, lstCli);

            //procurar por cst

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);


            ViewBag.CstGeral = db.CstIcmsGerais;



            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditCstNfeSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            //ViewBag.Tributos = trib;
            ViewBag.CstGeral = db.CstIcmsGerais;
            return View(trib);
        }
        [HttpGet]
        public ActionResult EditCstNfeSNMassaModalPost(string strDados, string cstCompraNfeSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            //Pega o código da cst pela sua descrição vinda da view
            int cstNfeSN = (int)(from a in db.CstIcmsGerais where a.descricao == cstCompraNfeSN select a.codigo).FirstOrDefault();

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.CsosntdaNfedoSnFOR = cstNfeSN; //novo cst


                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoCstEntrada", "Tributacao");
        }

        //Aliq Entrada Pis
        [HttpGet]
        public ActionResult EditAliEntPisMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;




            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;



            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_PIS != null  && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_PIS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_PIS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_PIS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

           //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_PIS == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_ENTRADA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstPisCofinsEntradas.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist



        }

        [HttpGet]
        public ActionResult EditAliEntPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }
            
            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliEntPisMassaModalPost(string strDados, string aliqEntPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            aliqEntPis = aliqEntPis.Replace(".", ",");
           
            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqEntPis = (aliqEntPis != "") ? trib.aliqEntPis = decimal.Parse(aliqEntPis) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq Saida Pis
        [HttpGet]
        public ActionResult EditAliSaiPisMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;




            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;



            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_PIS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_PIS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_PIS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_PIS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_PIS == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_SAIDA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliSaiPisMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliSaiPisMassaModalPost(string strDados, string aliqSaidaPis)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaidaPis = aliqSaidaPis.Replace(".", ","); 

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqSaidaPis = (aliqSaidaPis != "") ? trib.aliqSaidaPis = decimal.Parse(aliqSaidaPis) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq entrada cofins
        [HttpGet]
        public ActionResult EditAliEntCofinsMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;




            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;



            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_COFINS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_COFINS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_COFINS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_COFINS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ENT_COFINS == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_ENTRADA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstPisCofinsEntradas.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliEntCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliEntCofinsMassaModalPost(string strDados, string aliqEntCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqEntCofins = aliqEntCofins.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqEntCofins = (aliqEntCofins != "") ? trib.aliqEntCofins = decimal.Parse(aliqEntCofins) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Aliq saida cofins
        [HttpGet]
        public ActionResult EditAliSaiCofinsMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;




            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;



            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_COFINS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_COFINS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_COFINS != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_COFINS == null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_SAIDA_COFINS == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_SAIDA_PISCOFINS.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstPisCofinsSaidas.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliSaiCofinsMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliSaiCofinsMassaModalPost(string strDados, string aliqSaiCofins)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqSaiCofins = aliqSaiCofins.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.aliqSaidaCofins = (aliqSaiCofins != "") ? trib.aliqSaidaCofins = decimal.Parse(aliqSaiCofins) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GraficoAliPisCofins", "Tributacao");
        }

        //Red Base Calc Compra de Industria
        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && s.CST_COMPRA_DE_IND == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null  && s.CST_COMPRA_DE_IND == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_IND != null && s.CST_COMPRA_DE_IND == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_IND == null && s.CST_COMPRA_DE_IND == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_IND == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }
        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompIndMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntInd = aliqRedBasCalcIcmsEntInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompraDeInd = (aliqRedBasCalcIcmsEntInd != "") ? trib.redBaseCalcIcmsCompraDeInd = decimal.Parse(aliqRedBasCalcIcmsEntInd) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base Calc ST compra de industria
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && s.CST_COMPRA_DE_IND == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null && s.CST_COMPRA_DE_IND == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND != null && s.CST_COMPRA_DE_IND == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == null && s.CST_COMPRA_DE_IND == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompIndMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntInd = aliqRedBasCalcIcmsSTEntInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompraDeInd = (aliqRedBasCalcIcmsSTEntInd != "") ? trib.redBaseCalcIcmsSTCompraDeInd = decimal.Parse(aliqRedBasCalcIcmsSTEntInd) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base de calc icms compra de atacado
        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_ATA == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }
        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompAtaMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntAta = aliqRedBasCalcIcmsEntAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompraDeAta = (aliqRedBasCalcIcmsEntAta != "") ? trib.redBaseCalcIcmsCompraDeAta = decimal.Parse(aliqRedBasCalcIcmsEntAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base de Calc Icms ST compra de atacado
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompAtaMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntAta = aliqRedBasCalcIcmsSTEntAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompraDeAta = (aliqRedBasCalcIcmsSTEntAta != "") ? trib.redBaseCalcIcmsSTCompraDeAta = decimal.Parse(aliqRedBasCalcIcmsSTEntAta) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base calc icms compra de Simples Nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsCompSNMassaModalPost(string strDados, string aliqRedBasCalcIcmsEntSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsEntSN = aliqRedBasCalcIcmsEntSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsCompradeSimpNacional = (aliqRedBasCalcIcmsEntSN != "") ? trib.redBaseCalcIcmsCompradeSimpNacional = decimal.Parse(aliqRedBasCalcIcmsEntSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }

        //Red Base calc icms ST Compra de Simples Nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTCompSNMassaModalPost(string strDados, string aliqRedBasCalcIcmsSTEntSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcIcmsSTEntSN = aliqRedBasCalcIcmsSTEntSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTCompradeSimpNacional = (aliqRedBasCalcIcmsSTEntSN != "") ? trib.redBaseCalcIcmsSTCompradeSimpNacional = decimal.Parse(aliqRedBasCalcIcmsSTEntSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsEntrada", "Tributacao");
        }




        //Red base calc venda varejo consumidor final
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
                                                                                                                                                                                                              //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarCFMassaModalPost(string strDados, string aliqRedBasCalcVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendVarCF = aliqRedBasCalcVendVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaVarejoConsFinal = (aliqRedBasCalcVendVarCF != "") ? trib.redBaseCalcIcmsVendaVarejoConsFinal = decimal.Parse(aliqRedBasCalcVendVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red bas calc icms ST venda vare consumidor final
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                 
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarCFMassaModalPost(string strDados, string aliqRedBasCalcSTVendVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendVarCF = aliqRedBasCalcSTVendVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTVendaVarejoConsFinal = (aliqRedBasCalcSTVendVarCF != "") ? trib.redBaseCalcIcmsSTVendaVarejoConsFinal = decimal.Parse(aliqRedBasCalcSTVendVarCF) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red bas calc icms venda vare para contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;


            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
                                                                                                                                                                                                              //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;


            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                   
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_VENDA_VAREJO_CONT == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaVarContMassaModalPost(string strDados, string aliqRedBasCalcVendaVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaVarCont = aliqRedBasCalcVendaVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcVendaVarejoCont = (aliqRedBasCalcVendaVarCont != "") ? trib.redBaseCalcVendaVarejoCont = decimal.Parse(aliqRedBasCalcVendaVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }


        //Red Bas Calc ICMS st Venda varejo contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            page = (procurarPor != null) || (procurarPorAliq != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
                                                                                  //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaVarContMassaModalPost(string strDados, string aliqRedBasCalcSTVendaVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendaVarCont = aliqRedBasCalcSTVendaVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.RedBaseCalcSTVendaVarejo_Cont = (aliqRedBasCalcSTVendaVarCont != "") ? trib.RedBaseCalcSTVendaVarejo_Cont = decimal.Parse(aliqRedBasCalcSTVendaVarCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red Base Calc Venda Ata Contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            //procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo
                                                                                                                                                                                                              //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;
            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_CONT == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);


            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaContMassaModalPost(string strDados, string aliqRedBasCalcVendaAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaAtaCont = aliqRedBasCalcVendaAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaAtaCont = (aliqRedBasCalcVendaAtaCont != "") ? trib.redBaseCalcIcmsVendaAtaCont = decimal.Parse(aliqRedBasCalcVendaAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }


        //Red Base Calc ST Venda ata contribuinte
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }



        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaContMassaModalPost(string strDados, string aliqRedBasCalcSTVendaAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendaAtaCont = aliqRedBasCalcSTVendaAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsSTVendaAtaCont = (aliqRedBasCalcSTVendaAtaCont != "") ? trib.redBaseCalcIcmsSTVendaAtaCont = decimal.Parse(aliqRedBasCalcSTVendaAtaCont) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }

        //Red Base Calc  Venda Ata Simples nacional
        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":

                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL == 20 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditRedBCIcmsVendaAtaSNMassaModalPost(string strDados, string aliqRedBasCalcVendaAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcVendaAtaSN = aliqRedBasCalcVendaAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //percorrer o array, atribuir o valor de ncm e salvar o objeto
            for (int i = 0; i < idTrib.Length; i++)
            {
                int idTrb = Int32.Parse(idTrib[i]);
                trib = db.Tributacoes.Find(idTrb);
                trib.dataAlt = DateTime.Now; //data da alteração
                trib.redBaseCalcIcmsVendaAtaSimpNacional = (aliqRedBasCalcVendaAtaSN != "") ? trib.redBaseCalcIcmsVendaAtaSimpNacional = decimal.Parse(aliqRedBasCalcVendaAtaSN) : null;
                db.SaveChanges();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("GfRedBCalcIcmsSaida", "Tributacao");
        }



        //Red Base Calc  Venda ST Ata Simples nacional - ATUALIZAÇÃO VERSÃO FINAL
       
        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL == 60 && s.UF_ORIGEM.Equals(this.ufOrigem) && s.UF_DESTINO.Equals(this.ufDestino));
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }



        [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        
         [HttpGet]
        public ActionResult EditRedBCIcmsSTVendaAtaSNMassaModalPost(string strDados, string aliqRedBasCalcSTVendaAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqRedBasCalcSTVendaAtaSN = aliqRedBasCalcSTVendaAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.redBaseCalcIcmsSTVendaAtaSimpNacional = (aliqRedBasCalcSTVendaAtaSN != "") ? trib.redBaseCalcIcmsSTVendaAtaSimpNacional = decimal.Parse(aliqRedBasCalcSTVendaAtaSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditRedBCIcmsSTVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
          
        }




        //Edição em massa: Aliq Icms Compra de indutria - atualizado em 25112021
        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassa(
            string origem, 
            string destino, 
            string opcao, 
            string param, 
            string ordenacao, 
            string qtdNSalvos, 
            string qtdSalvos, 
            string procurarPor,
            string procurarPorAliq, 
            string procuraNCM, 
            string procuraCEST, 
            string procuraSetor, 
            string filtroSetor, 
            string filtroCorrente, 
            string filtroCorrenteAliq,
            string procuraCate, 
            string filtroCate, 
            string filtroCorrenteNCM, 
            string filtroCorrenteCEST, 
            string filtroNulo, 
            string filtraPor, 
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
            int? page, 
            int? numeroLinhas
           )
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;
            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //var trib1 = from s in db.Tributacoes select s; //variavel carregado de produtos

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nennum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALIQUOTA
                    //switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND != null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2": //sem aliquota
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3": //isenta
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null || s.ALIQ_ICMS_COMP_DE_IND != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nennum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALIQUOTA
                    //switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND != null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2": //sem aliquota
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3": //isenta
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null || s.ALIQ_ICMS_COMP_DE_IND != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nennum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-isenta
                    //switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND != null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2": //sem aliquota
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null && s.CST_COMPRA_DE_IND != 60 && s.CST_COMPRA_DE_IND != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3": //isenta
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == null || s.ALIQ_ICMS_COMP_DE_IND != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;

            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMP_DE_IND == pAlq);

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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }
        [HttpGet]
        public ActionResult EditAliqIcmsCompIndMassaModalPost(string strDados, string aliqIcmsCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraInd = aliqIcmsCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsCompDeInd = (aliqIcmsCompraInd != "") ? trib.aliqIcmsCompDeInd = decimal.Parse(aliqIcmsCompraInd) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });


           
        }



        //Edição em massa: Aliq Icms St Compra de Industria - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao vinda do grafico
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro) 
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;

                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                        switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMP_DE_IND == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_IND.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);

        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompIndMassaModalPost(string strDados, string aliqIcmsSTCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraInd = aliqIcmsSTCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTCompDeInd = (aliqIcmsSTCompraInd != "") ? trib.aliqIcmsSTCompDeInd = decimal.Parse(aliqIcmsSTCompraInd) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTCompIndMassa", new { param = resultado, qtdSalvos = regSalvos });



           
        }



        //Edit Aliq Icms compra de atacado - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;


            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null || s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null || s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {


                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null && s.CST_COMPRA_DE_ATA != 60 && s.CST_COMPRA_DE_ATA != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == null || s.ALIQ_ICMS_COMPRA_DE_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_ATA == pAlq);

            }


            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompAtaMassaModalPost(string strDados, string aliqIcmsCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraAta = aliqIcmsCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsCompradeAta = (aliqIcmsCompraAta != "") ? trib.aliqIcmsCompradeAta = decimal.Parse(aliqIcmsCompraAta) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsCompAtaMassa", new { param = resultado, qtdSalvos = regSalvos });


        }




        //Edit Aliq icms st compra de atacado - ATUALIZADO VERSÃO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");


            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;


            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_ATA == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_ATA != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_ATA == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_ATA == pAlq);

            }

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_ATA.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompAtaMassaModalPost(string strDados, string aliqIcmsSTCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraAta = aliqIcmsSTCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();


            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTCompraDeAta = (aliqIcmsSTCompraAta != "") ? trib.aliqIcmsSTCompraDeAta = decimal.Parse(aliqIcmsSTCompraAta) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTCompAtaMassa", new { param = resultado, qtdSalvos = regSalvos });
           
        }



        //Edit aliq icms compra de simples nacional - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
        string procurarPorAliq,
        string procuraNCM,
        string procuraCEST,
        string procuraSetor,
        string filtroSetor,
        string filtroCorrente,
        string filtroCorrenteAliq,
        string procuraCate,
        string filtroCate,
        string filtroCorrenteNCM,
        string filtroCorrenteCEST,
        string filtroNulo,
        string filtraPor,
        string filtroFiltraPor,
        string procuraCST,
        string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null || s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null || s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null && s.CST_COMPRA_DE_SIMP_NACIONAL != 60 && s.CST_COMPRA_DE_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == null || s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist


        }
        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsCompSNMassaModalPost(string strDados, string aliqIcmsCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsCompraSN = aliqIcmsCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsCompradeSimpNacional = (aliqIcmsCompraSN != "") ? trib.aliqIcmsCompradeSimpNacional = decimal.Parse(aliqIcmsCompraSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsCompSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit aliq icms st compra de simples nacional -  ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsSTCompSNMassa(string origem,
        string destino,
        string opcao,
        string param,
        string ordenacao,
        string qtdNSalvos,
        string qtdSalvos,
        string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;


            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;


            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL == pAlq);

            }

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_COMPRA_DE_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]          
        public ActionResult EditAliqIcmsSTCompSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTCompSNMassaModalPost(string strDados, string aliqIcmsSTCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTCompraSN = aliqIcmsSTCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTCompradeSimpNacional = (aliqIcmsSTCompraSN != "") ? trib.aliqIcmsSTCompradeSimpNacional = decimal.Parse(aliqIcmsSTCompraSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }

            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTCompSNMassa", new { param = resultado, qtdSalvos = regSalvos });

        }




        //Edit aliq Icms Nfe Ind -  ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE != null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                    
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null || s.ALIQ_ICMS_NFE != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE != null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null || s.ALIQ_ICMS_NFE != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE != null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null && s.CST_DA_NFE_DA_IND_FORN != 60 && s.CST_DA_NFE_DA_IND_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == null || s.ALIQ_ICMS_NFE != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DA_IND_FORN.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeIndMassaModalPost(string strDados, string aliqIcmsNfeCompraInd)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraInd = aliqIcmsNfeCompraInd.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsNFE = (aliqIcmsNfeCompraInd != "") ? trib.aliqIcmsNFE = decimal.Parse(aliqIcmsNfeCompraInd) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsNfeIndMassa", new { param = resultado, qtdSalvos = regSalvos });
        }




        //Edit aliq icms nfe Ata - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassa(
            string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;



            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA != null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null || s.ALIQ_ICMS_NFE_FOR_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA != null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null || s.ALIQ_ICMS_NFE_FOR_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA != null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null && s.CST_DA_NFE_DE_ATA_FORN != 60 && s.CST_DA_NFE_DE_ATA_FORN != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_ATA == null || s.ALIQ_ICMS_NFE_FOR_ATA != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.CST_DA_NFE_DE_ATA_FORN == pAlq);

            }

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeAtaMassaModalPost(string strDados, string aliqIcmsNfeCompraAta)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraAta = aliqIcmsNfeCompraAta.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsNfeAta = (aliqIcmsNfeCompraAta != "") ? trib.aliqIcmsNfeAta = decimal.Parse(aliqIcmsNfeCompraAta) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsNfeAtaMassa", new { param = resultado, qtdSalvos = regSalvos });

        }



        //Edit aliq icms nfe SN - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassa(string origem,
            string destino,
            string opcao,
            string param,
            string ordenacao,
            string qtdNSalvos,
            string qtdSalvos,
            string procurarPor,
            string procurarPorAliq,
            string procuraNCM,
            string procuraCEST,
            string procuraSetor,
            string filtroSetor,
            string filtroCorrente,
            string filtroCorrenteAliq,
            string procuraCate,
            string filtroCate,
            string filtroCorrenteNCM,
            string filtroCorrenteCEST,
            string filtroNulo,
            string filtraPor,
            string filtroFiltraPor,
            string procuraCST,
            string filtraCST,
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;


            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN != null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null || s.ALIQ_ICMS_NFE_FOR_SN != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN != null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null || s.ALIQ_ICMS_NFE_FOR_SN != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN != null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);

                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null && s.CSOSNTDANFEDOSNFOR != 60 && s.CSOSNTDANFEDOSNFOR != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == null || s.ALIQ_ICMS_NFE_FOR_SN != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_NFE_FOR_SN == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CSOSNTDANFEDOSNFOR.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsNfeSNMassaModalPost(string strDados, string aliqIcmsNfeCompraSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsNfeCompraSN = aliqIcmsNfeCompraSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsNfeSN = (aliqIcmsNfeCompraSN != "") ? trib.aliqIcmsNfeSN = decimal.Parse(aliqIcmsNfeCompraSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



           

            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsNfeSNMassa", "Tributacao");
        }




        //Edit aliq Icms Venda Varejo consumidor final - ATUALIZADO VERSAO FINAL - ATUALIZADO UFORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST,string filtraCST,  int? page, int? numeroLinhas)

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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;



            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salvar*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) ||(procuraSetor != null) || (procuraCate != null)  ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;
            
            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();

           

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            

            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        /*Aliquota ICMS Venda Varejo Consumidor Final*/


                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "4":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null || s.CST_VENDA_VAREJO_CONS_FINAL != null);
                            this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA == 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "5":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 41 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;

                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "4":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null || s.CST_VENDA_VAREJO_CONS_FINAL != null);
                            this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA == 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "5":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 41 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "4":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null || s.CST_VENDA_VAREJO_CONS_FINAL != null);
                            this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA == 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "5":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 41 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Uso Consumo":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4"; //3-senta
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "4":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null || s.CST_VENDA_VAREJO_CONS_FINAL != null);
                            this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA == 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "5":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 41 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Não Tributada":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5"; //5-NÃO TRIBUTADA
                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && s.CST_VENDA_VAREJO_CONS_FINAL != 60 && s.CST_VENDA_VAREJO_CONS_FINAL != 40 && s.CST_VENDA_VAREJO_CONS_FINAL != 41 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "4":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == null || s.CST_VENDA_VAREJO_CONS_FINAL != null);
                            this.lstCli = this.lstCli.Where(s => s.ID_CATEGORIA == 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "5":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 41 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s=>s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s=>s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarCFMassaModalPost(string strDados, decimal? aliqIcmsVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            //aliqIcmsVenVarCF = aliqIcmsVenVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsVendaVarejoConsFinal = (aliqIcmsVenVarCF != null) ? trib.aliqIcmsVendaVarejoConsFinal = aliqIcmsVenVarCF : null;
                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }

            

           //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsVenVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }





        //Edit Aliq ICMs ST Venda Varejo Consumidor finnal - ATUALIZADA PARA VERSÃO FINAL - ATUALIZADO UF DESTINO E ORIGEM
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

         

            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            //this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 60);
                            //this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            this.lstCli = this.lstCli.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);
                            
                            //ViewBag.AliqICMSSTVendaVarCF = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);
                            //ViewBag.AliqICMSSTVendaVarCFNulla = this.lstCli.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL == 60 && a.ID_CATEGORIA != 21);


                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && s.ID_CATEGORIA != 21);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && s.ID_CATEGORIA != 21);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && s.ID_CATEGORIA != 21);
                            break;
                    }
                    break;
            }
            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONS_FINAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(this.lstCli.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarCFMassaModalPost(string strDados, decimal? aliqIcmsSTVenVarCF)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            //aliqIcmsSTVenVarCF = aliqIcmsSTVenVarCF.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTVendaVarejoConsFinal = (aliqIcmsSTVenVarCF != null) ? trib.aliqIcmsVendaVarejoConsFinal = aliqIcmsSTVenVarCF : null;
                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTVenVarCFMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //daqui alteracao de muitas actions

        //Edit Aliq ICMs Venda Varejo para contribuinte - ATUALIZADO VERSAO FINAL - ATUALIZADO UFORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)

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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }

            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }

            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //criar o temp data da lista ou recupera-lo
            VerificaTempData();
            VerificaOriDest(origem, destino);

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch(opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21  && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null || s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null || s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;

                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null && s.CST_VENDA_VAREJO_CONT != 60 && s.CST_VENDA_VAREJO_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == null || s.ALIQ_ICMS_VENDA_VAREJO_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;

                    }
                    break;
               
            }



            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);
            
            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_VAREJO_CONT == pAlq);

            }

            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

       

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsVenVarContMassaModalPost(string strDados, string aliqIcmsVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVenVarCont = aliqIcmsVenVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsVendaVarejoCont = (aliqIcmsVenVarCont != "") ? trib.aliqIcmsVendaVarejoCont = decimal.Parse(aliqIcmsVenVarCont) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsVenVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }

       
        //Edit Aliq ICMs STVenda Varejo para contribuinte - ATUALIZADO VERSAO FINAL
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_VAREJO_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenVarContMassaModalPost(string strDados, string aliqIcmsSTVenVarCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVenVarCont = aliqIcmsSTVenVarCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTVendaVarejo_Cont = (aliqIcmsSTVenVarCont != "") ? trib.aliqIcmsSTVendaVarejo_Cont = decimal.Parse(aliqIcmsSTVenVarCont) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTVenVarContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit Aliq ICMs venda atacado para contribuinte - ATUALIZAÇÃO VERSÃO FINAL - ATUALIZADO UFORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null || s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null || s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null && s.CST_VENDA_ATA_CONT != 60 && s.CST_VENDA_ATA_CONT != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_CONT == null || s.ALIQ_ICMS_VENDA_ATA_CONT != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }
        
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaContMassaModalPost(string strDados, string aliqIcmsVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVendAtaCont = aliqIcmsVendAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsVendaAtaCont = (aliqIcmsVendAtaCont != "") ? trib.aliqIcmsVendaAtaCont = decimal.Parse(aliqIcmsVendAtaCont) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsVenAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }




        //Edit Aliq ICMs ST venda atacado para contribuinte  - ATUALIZAÇÃO VERSÃO FINAL - ATUALIZADO COM UF DE ORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_CONT != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_CONT == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_CONT != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_CONT == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }


            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_CONT.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }


        
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaContMassaModalPost(string strDados, string aliqIcmsSTVendAtaCont)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVendAtaCont = aliqIcmsSTVendAtaCont.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;


            //objeto tributação
            Tributacao trib = new Tributacao();


            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTVendaAtaCont = (aliqIcmsSTVendAtaCont != "") ? trib.aliqIcmsSTVendaAtaCont = decimal.Parse(aliqIcmsSTVendAtaCont) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }



            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTVenAtaContMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //Edit Aliq ICMs  venda atacado para SN -  ATUALIZAÇÃO VERSÃO FINAL - ATUALIZADO UFORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaSNMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
            string procuraCST, string filtraCST, int? page, int? numeroLinhas)
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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;
            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();


            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;


            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Isenta":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && s.CST_VENDA_ATA_SIMP_NACIONAL != 60 && s.CST_VENDA_ATA_SIMP_NACIONAL != 40 && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "3":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 40 && s.ID_CATEGORIA != 21);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || s.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
            this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                double pAlq = Convert.ToDouble(procurarPorAliq);
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == pAlq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }


        [HttpGet]
        public ActionResult EditAliqIcmsVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }

        
        [HttpGet] 
        public ActionResult EditAliqIcmsVenAtaSNMassaModalPost(string strDados, string aliqIcmsVendAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsVendAtaSN = aliqIcmsVendAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();

            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsVendaAtaSimpNacional = (aliqIcmsVendAtaSN != "") ? trib.aliqIcmsVendaAtaSimpNacional = decimal.Parse(aliqIcmsVendAtaSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }
        




        //Edit Aliq ICMs ST venda atacado para SN - atualizado para VERSÃO FINAL - ATUALZIADO PARA UFORIGEM E DESTINO
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassa(string origem, string destino, string opcao, string param, string ordenacao, string qtdNSalvos, string qtdSalvos, string procurarPor,
            string procurarPorAliq, string procuraNCM, string procuraCEST, string procuraSetor, string filtroSetor, string filtroCorrente, string filtroCorrenteAliq,
            string procuraCate, string filtroCate, string filtroCorrenteNCM, string filtroCorrenteCEST, string filtroNulo, string filtraPor, string filtroFiltraPor,
             string procuraCST, string filtraCST, int? page, int? numeroLinhas)

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
            procurarPorAliq = (procurarPorAliq != null) ? procurarPorAliq.Replace(",", ".") : null;

            //Seleção por setor ou categoria
            filtraPor = (filtraPor != null) ? filtraPor : "Setor"; //padrão é por setor

            if (filtraPor != "Setor")
            {

                ViewBag.FiltrarPor = "Categoria";
                procuraSetor = null;
            }
            else
            {
                ViewBag.FiltrarPor = "Setor";
                procuraCate = null;
            }
            //categoria
            procuraCate = (procuraCate == "") ? null : procuraCate;
            procuraCate = (procuraCate == "null") ? null : procuraCate;
            procuraCate = (procuraCate != null) ? procuraCate : null;

            //setor
            procuraSetor = (procuraSetor == "") ? null : procuraSetor;
            procuraSetor = (procuraSetor == "null") ? null : procuraSetor;
            procuraSetor = (procuraSetor != null) ? procuraSetor : null;

            //cst
            procuraCST = (procuraCST == "") ? null : procuraCST;
            procuraCST = (procuraCST == "null") ? null : procuraCST;
            procuraCST = (procuraCST != null) ? procuraCST : null;

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            //ordenação
            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;


            /*Verifica a opção e atribui a uma tempdata para continuar salva*/
            TempData["opcao"] = opcao ?? TempData["opcao"]; //se opção != null
            opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;


            //persiste tempdata entre as requisições ate que opcao seja mudada na chamada pelo grafico
            TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procurarPor != null) || (procurarPorAliq != null) || (procuraCEST != null) || (procuraNCM != null) || (procuraSetor != null) || (procuraCate != null) || (procuraCST != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

            //atrbui filtro corrente caso alguma procura esteja nulla
            procurarPor = (procurarPor == null) ? filtroCorrente : procurarPor; //atribui o filtro corrente se procuraPor estiver nulo
            procurarPorAliq = (procurarPorAliq == null) ? filtroCorrenteAliq : procurarPorAliq;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCEST : procuraCEST;
            procuraSetor = (procuraSetor == null) ? filtroSetor : procuraSetor;
            procuraCate = (procuraCate == null) ? filtroCate : procuraCate;

            //cst
            procuraCST = (procuraCST == null) ? filtraCST : procuraCST;

            //View pag para filtros
            ViewBag.FiltroCorrente = procurarPor;
            ViewBag.FiltroCorrenteAliq = procurarPorAliq;
            ViewBag.FiltroCorrenteNCM = procuraNCM;
            ViewBag.FiltroCorrenteCEST = procuraCEST;

            ViewBag.FiltroCorrenteSetor = procuraSetor;
            ViewBag.FiltroCorrenteCate = procuraCate;
            ViewBag.FiltroFiltraPor = filtraPor;

            //cst
            ViewBag.FiltroCST = procuraCST;

            //converter o valor da procura por setor ou categoria em inteiro
            if (procuraSetor != null)
            {
                ViewBag.FiltroCorrenteSetorInt = int.Parse(procuraSetor);
            }
            if (procuraCate != null)
            {
                ViewBag.FiltroCorrenteCateInt = int.Parse(procuraCate);
            }
            if (procuraCST != null)
            {
                ViewBag.FiltroCorrenteCSTInt = int.Parse(procuraCST);
            }
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados;
            ViewBag.EstadosDestinos = db.Estados;

            //verifica carregamento da tabela
            VerificaTempData();



            //verifica estados origem e destino
            VerificaOriDest(origem, destino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            //ViewBag com a opcao
            ViewBag.Opcao = opcao;

            switch (opcao)
            {
                case "Com aliquota":
                    //o parametro filtronulo mostra o filtro informado, caso nao informar nenhum ele sera de acordo com a opcao
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1"; //1-COM ALÍQUOTA
                    //Switche do filtro
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
                case "Sem aliquota":
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2"; //2-SEM ALÍQUOTA
                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                        case "2":
                            this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL == 60);
                            this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && s.ID_CATEGORIA != 21 && s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);
                            break;
                    }
                    break;
            }

            //Action para procurar: passando alguns parametros que são comuns em todas as actions
           this.lstCli = ProcurarPorIII(codBarrasL, procurarPor, procuraCEST, procuraNCM, procuraSetor, procuraCate, lstCli);

            //Busca por aliquota
            if (!String.IsNullOrEmpty(procurarPorAliq))
            {
                this.lstCli = this.lstCli.Where(s => s.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL.ToString() == procurarPorAliq);

            }
            //Busca por cst
            if (!String.IsNullOrEmpty(procuraCST))
            {
                this.lstCli = this.lstCli.Where(s => s.CST_VENDA_ATA_SIMP_NACIONAL.ToString() == procuraCST);
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
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.SetorProdutos = db.SetorProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CategoriaProdutos = db.CategoriaProdutos.AsNoTracking().OrderBy(s => s.descricao);
            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking().OrderBy(s => s.codigo);

            //ViewBag.CstGeral = db.CstIcmsGerais; //para montar a descrição da cst na view
            return View(tribMTX.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist
        }

        
        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            string[] dadosDoCadastro = strDados.Split(',');
            dadosDoCadastro = dadosDoCadastro.Where(item => item != "").ToArray(); //retira o 4o. elemento
            trib = new List<Tributacao>();
            for (int i = 0; i < dadosDoCadastro.Length; i++)
            {
                int aux = Int32.Parse(dadosDoCadastro[i]);
                trib.Add(db.Tributacoes.Find(aux));

            }

            return View(trib);
        }


        [HttpGet]
        public ActionResult EditAliqIcmsSTVenAtaSNMassaModalPost(string strDados, string aliqIcmsSTVendAtaSN)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            //trocando o ponto por virgula
            aliqIcmsSTVendAtaSN = aliqIcmsSTVendAtaSN.Replace(".", ",");

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array
            idTrib = idTrib.Where(item => item != "").ToArray();

            //variavel auxiliar para guardar o resultado
            string resultado = "";
            int regSalvos = 0;

            //objeto tributação
            Tributacao trib = new Tributacao();



            try
            {
                //percorrer o array, atribuir o valor de ncm e salvar o objeto
                for (int i = 0; i < idTrib.Length; i++)
                {
                    int idTrb = Int32.Parse(idTrib[i]);
                    trib = db.Tributacoes.Find(idTrb);
                    trib.dataAlt = DateTime.Now; //data da alteração
                    trib.aliqIcmsSTVendaAtaSimpNacional = (aliqIcmsSTVendAtaSN != "") ? trib.aliqIcmsSTVendaAtaSimpNacional = decimal.Parse(aliqIcmsSTVendAtaSN) : null;

                    db.SaveChanges();
                    regSalvos++;

                }

                TempData["tributacaoMTX"] = null; //recarrega a lista
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();
            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EditAliqIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalvos });
        }



        //actions auxiliares // ponto de ajuste: busca por aliquota
        private List<TributacaoGeralView> ProcurarPor(long? codBarrasL, string procurarPor,  string procuraCEST, string procuraNCM, List<TributacaoGeralView> tribMTX)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                tribMTX = (codBarrasL != 0) ? (this.tribMTX.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString())).ToList()) : tribMTX = (this.tribMTX.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())).ToList());
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tribMTX = this.tribMTX.Where(s => s.CEST_PRODUTO == procuraCEST).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO == procuraNCM).ToList();

            }
           
           

            return tribMTX;
        }

        private List<TributacaoGeralView> ProcurarPorII(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, string procuraSetor, string procuraCate, List<TributacaoGeralView> tribMTX)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.tribMTX = (codBarrasL != 0) ? (this.tribMTX.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString())).ToList()) : tribMTX = (this.tribMTX.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())).ToList());
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                this.tribMTX = this.tribMTX.Where(s => s.CEST_PRODUTO == procuraCEST).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                this.tribMTX = this.tribMTX.Where(s => s.NCM_PRODUTO == procuraNCM).ToList();

            }
          
            //Busca por setor
            if (!String.IsNullOrEmpty(procuraSetor))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ID_SETOR.ToString() == procuraSetor).ToList();

            }
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.tribMTX = this.tribMTX.Where(s => s.ID_CATEGORIA.ToString() == procuraCate).ToList();


            }

            return this.tribMTX;
        }

        private IQueryable<TributacaoGeralView> ProcurarPorIII(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, string procuraSetor, string procuraCate, IQueryable<TributacaoGeralView> lstCli)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.lstCli = (codBarrasL != 0) ? (lstCli.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))) : lstCli = (lstCli.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                this.lstCli = lstCli.Where(s => s.CEST_PRODUTO == procuraCEST);
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                this.lstCli = lstCli.Where(s => s.NCM_PRODUTO == procuraNCM);

            }

            //Busca por setor
            if (!String.IsNullOrEmpty(procuraSetor))
            {
                this.lstCli = lstCli.Where(s => s.ID_SETOR.ToString() == procuraSetor);

            }
            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.lstCli = lstCli.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


            }

            return this.lstCli;
        }

        private IQueryable<TributacaoGeralView> ProcurarPorIV(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, IQueryable<TributacaoGeralView> lstCli)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                this.lstCli = (codBarrasL != 0) ? (lstCli.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))) : lstCli = (lstCli.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                this.lstCli = lstCli.Where(s => s.CEST_PRODUTO == procuraCEST);
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                this.lstCli = lstCli.Where(s => s.NCM_PRODUTO == procuraNCM);

            }



            return lstCli;
        }





        //retorno vazio para verificar a tempdata

        public EmptyResult VerificaTempData()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTX"] == null)
            {
                //this.lstCli = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a);
              
                //this.tributacao = db.Tributacoes;

                //this.tributacao = (from a in db.Tributacoes where a.UF_Origem.Equals(this.ufOrigem) && a.UF_Destino.Equals(this.ufDestino) select a);
             
                // this.lstCli = (from a in db.Tributacao_GeralView  select a);
                //this.lstCli = db.Tributacao_GeralView.AsNoTracking();
                this.lstCli = from a in db.Tributacao_GeralView select a;
                              
               
               //TempData["tributacaoMTX"] = this.lstCli; //cria a temp data e popula

                TempData["tributacaoMTX"] = this.lstCli; //cria a temp data e popula

                //TempData["tributacaoMTX"] = this.tributacao; //cria a temp data e popula
                TempData.Keep("tributacaoMTX"); //persiste
            }
            else
            {
                // this.lstCli = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                this.lstCli = (IQueryable<TributacaoGeralView>)TempData["tributacaoMTX"];
                //this.tributacao = (List<Tributacao>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
               //this.tribMTX = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata

                TempData.Keep("tributacaoMTX"); //persiste
            }

            return new EmptyResult();
        }


        /*Bakcup VerificaTempDAtaNCM - 23122022*/
        //public EmptyResult VerificaTempDataNCM()
        //{
        //    /*PAra tipar */
        //    /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
        //     na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
        //    if (TempData["tributacaoMTX_NCM"] == null)
        //    {

        //        this.tribMTX_NCM = from a in db.TributacoesNcm select a;



        //        TempData["tributacaoMTX_NCM"] = this.tribMTX_NCM; //cria a temp data e popula

        //        //TempData["tributacaoMTX"] = this.tributacao; //cria a temp data e popula
        //        TempData.Keep("tributacaoMTX_NCM"); //persiste
        //    }
        //    else
        //    {
        //        // this.tribMTX_NCM = (List<TributacaoNCM>)TempData["tributacaoMTX_NCM"];//atribui a lista os valores de tempdata

        //        this.tribMTX_NCM = (IQueryable<TributacaoNCM>)TempData["tributacaoMTX_NCM"];

        //        TempData.Keep("tributacaoMTX_NCM"); //persiste
        //    }

        //    return new EmptyResult();
        //}

        public EmptyResult VerificaTempDataNCM(string tributacao)
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTX_NCMView"] == null)
            {
               if(tributacao =="SIMPLES")
                {
                    this.tribMTX_NCMView = from a in db.TributacoesNcmView where a.SIMP_NACIONAL==1 select a; //FILTRA O QUE FOR SIMPLES NACIONAL
                }
                else
                {
                    this.tribMTX_NCMView = from a in db.TributacoesNcmView where a.SIMP_NACIONAL == 0 select a;
                }
               


                
                TempData["tributacaoMTX_NCMView"] = this.tribMTX_NCMView; //cria a temp data e popula

                //TempData["tributacaoMTX"] = this.tributacao; //cria a temp data e popula
                TempData.Keep("tributacaoMTX_NCMView"); //persiste
            }
            else
            {
                // this.tribMTX_NCM = (List<TributacaoNCM>)TempData["tributacaoMTX_NCM"];//atribui a lista os valores de tempdata
               
               this.tribMTX_NCMView = (IQueryable<TributacaoNCMView>)TempData["tributacaoMTX_NCMView"];

                TempData.Keep("tributacaoMTX_NCMView"); //persiste
            }

            return new EmptyResult();
        }
        public EmptyResult VerificaTempDataSN()
        {
            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novaente*/
            if (TempData["tributacaoMTXSN"] == null)
            {
                //this.lstCli = (from a in db.Tributacao_GeralView where a.ID.ToString() != null select a);

                //this.tributacao = db.Tributacoes;

                //this.tributacao = (from a in db.Tributacoes where a.UF_Origem.Equals(this.ufOrigem) && a.UF_Destino.Equals(this.ufDestino) select a);

                // this.lstCli = (from a in db.Tributacao_GeralView  select a);
                //this.lstCli = db.Tributacao_GeralView.AsNoTracking();
                this.lstCliSN = from a in db.Tributacao_GeralView_Sn select a;


                //TempData["tributacaoMTX"] = this.lstCli; //cria a temp data e popula

                TempData["tributacaoMTXSN"] = this.lstCliSN; //cria a temp data e popula

                //TempData["tributacaoMTX"] = this.tributacao; //cria a temp data e popula
                TempData.Keep("tributacaoMTXSN"); //persiste
            }
            else
            {
                // this.lstCli = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                this.lstCliSN = (IQueryable<TtributacaoGeralViewSN>)TempData["tributacaoMTXSN"];
                //this.tributacao = (List<Tributacao>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata
                //this.tribMTX = (List<TributacaoGeralView>)TempData["tributacaoMTX"];//atribui a lista os valores de tempdata

                TempData.Keep("tributacaoMTXSN"); //persiste
            }

            return new EmptyResult();
        }


     
        public EmptyResult VerificaTributacoesPorUF(string tributacao)
        {
            if(TempData["tributacaoPorUF"] == null)
            {
                if (tributacao == "SIMPLES")
                {
                    this.tribPorUF = from a in db.TributacoesNcmView where a.SIMP_NACIONAL == 1 select a; //FILTRA O QUE FOR SIMPLES NACIONAL
                }
                else
                {
                    this.tribPorUF = from a in db.TributacoesNcmView where a.SIMP_NACIONAL == 0 select a;
                }

                TempData["tributacaoPorUF"] = this.tribPorUF; //cria a temp data e popula

                //TempData["tributacaoMTX"] = this.tributacao; //cria a temp data e popula
                TempData.Keep("tributacaoPorUF"); //persiste

            }
            else
            {
                this.tribPorUF = (IQueryable<TributacaoNCMView>)TempData["tributacaoPorUF"];

                TempData.Keep("tributacaoPorUF"); //persiste

            }
            return new EmptyResult();
        }


        private IQueryable<TributacaoGeralView> ProcurarPorV(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, IQueryable<TributacaoGeralView> lstCli)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                lstCli = (codBarrasL != 0) ? (lstCli.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))) : lstCli = (lstCli.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                lstCli = lstCli.Where(s => s.CEST_PRODUTO.ToString().StartsWith(procuraCEST.ToString()));
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                string buscaNCM = procuraNCM.Replace(".", "");

                lstCli = lstCli.Where(s => s.NCM_PRODUTO.ToString().StartsWith(buscaNCM.ToString()));

            }

            return lstCli;
        }

        //PROCURA DO SN
        private IQueryable<TtributacaoGeralViewSN> ProcurarPorSN(long? codBarrasL, string procurarPor, string procuraCEST, string procuraNCM, IQueryable<TtributacaoGeralViewSN> lstCliSN)
        {


            if (!String.IsNullOrEmpty(procurarPor))
            {
                lstCliSN = (codBarrasL != 0) ? (lstCliSN.Where(s => s.COD_BARRAS_PRODUTO.ToString().StartsWith(codBarrasL.ToString()))) : lstCliSN = (lstCliSN.Where(s => s.DESCRICAO_PRODUTO.ToString().ToUpper().StartsWith(procurarPor.ToUpper())));
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                lstCliSN = lstCliSN.Where(s => s.CEST_PRODUTO.ToString().StartsWith(procuraCEST.ToString()));
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                lstCliSN = lstCliSN.Where(s => s.NCM_PRODUTO.ToString().StartsWith(procuraNCM.ToString()));

            }

            return lstCliSN;
        }


        private IQueryable<TributacaoNCM> ProcurarPorNCM_CEST(string procuraCEST, string procuraNCM, IQueryable<TributacaoNCM> tribMTX_NCM)
        {
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tribMTX_NCM = tribMTX_NCM.Where(s => s.cest.ToString().StartsWith(procuraCEST.ToString()));
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                tribMTX_NCM = tribMTX_NCM.Where(s => s.ncm.ToString().StartsWith(procuraNCM.ToString()));

            }

            return tribMTX_NCM;

        }




        private IQueryable<TributacaoNCMView> ProcurarPorNCM_CEST_PARA_NCM(string procuraCEST, string procuraNCM, IQueryable<TributacaoNCMView> tribMTX_NCMview)
        {
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tribMTX_NCMview = tribMTX_NCMview.Where(s => s.CEST.ToString().StartsWith(procuraCEST.ToString()));
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                tribMTX_NCMview = tribMTX_NCMview.Where(s => s.NCM.ToString().StartsWith(procuraNCM.ToString()));

            }

            return tribMTX_NCMview;

        }




        //aplica a tributação na tempdada
        private EmptyResult VerificaTributacao(string tributacao)
        {
            if(tributacao == null || tributacao == "")
            {
                TempData["tributacao"] = (TempData["tributacao"] == null) ? "OUTROS" : TempData["tributacao"].ToString();
                TempData.Keep("tributacao");
            }
            else
            {
                if(TempData["tributacao"].ToString() != tributacao)
                {
                    //quer dizer que mudou, logo devemos zerar a tempdata da tributação
                    TempData["tributacaoMTX_NCMView"] = null;
                }
                TempData["tributacao"] = (TempData["tributacao"] == null) ? "OUTROS" :tributacao;
                TempData.Keep("tributacao");
            }
          
            return new EmptyResult();
        }
        private EmptyResult VerificaOriDest()
        {
            TempData["UfOrigem"] = (TempData["UfOrigem"] == null) ? "TO" : TempData["UfOrigem"].ToString();
            TempData.Keep("UfOrigem");
            TempData["UfDestino"] = (TempData["UfDestino"] == null) ? "TO" : TempData["UfDestino"].ToString();
            TempData.Keep("UfDestino");



            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            return new EmptyResult();
        }
        private EmptyResult VerificaOriDest(string origem, string destino)
        {
          
            if(origem == null || origem == "")
            {
                TempData["UfOrigem"] = (TempData["UfOrigem"] == null) ? "TO" : TempData["UfOrigem"].ToString();
                TempData.Keep("UfOrigem");
            }
            else
            {
                TempData["UfOrigem"] = origem;
                TempData.Keep("UfOrigem");

            }

            if(destino == null || destino == "")
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
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

    }//fim controller

}