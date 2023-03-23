using MatrizTributaria.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//atualização pela maquina virtual
namespace MatrizTributaria.Controllers
{
    public class TributacaoMtxController : Controller
    {
        //Objego context
        readonly MatrizDbContext db = new MatrizDbContext();

        //LISTA
        IQueryable<TributacaoNCMView> tributacaoMTX_NCMView;
        List<TributacaoNCM> tribNCM;
        //origem e destino
        string ufOrigem = "";
        string ufDestino = "";

        // GET: TributacaoMtx
        public ActionResult Index()
        {
            return View();
        }

        //Analise usando o NCM: Março/2023
        public ActionResult TributacaoComNCM(string origem, string destino, string opcao, string param, string ordenacao, string qtdSalvos, string qtdNSalvos, string procuraNCM, string procuraCEST,
            string procurarPor, string filtroCorrente, string procuraSetor, string filtroSetor, string procuraCate, string filtroCate, string filtroCorrenteNCM,
            string filtroCorrenteCEST, int? page, int? numeroLinhas, string filtroNulo, string auditadosNCM, string filtraPor, string filtroFiltraPor,
            string filtroCorrenteAudNCM, string crt, string regime)
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
            //if (procuraCate == null || procuraCate == "" || procuraCate == "null")
            //{
            //    if (TempData["procuraCAT"] != null)
            //    {
            //        procuraCate = TempData["procuraCAT"].ToString();
            //    }
            //    else
            //    {
            //        procuraCate = null;
            //        TempData["procuraCAT"] = null;
            //    }

            //}
            //else
            //{
            //    if (TempData["procuraCAT"] != null)
            //    {
            //        if (procuraCate != (TempData["procuraCAT"].ToString()))
            //        {
            //            TempData["procuraCAT"] = procuraCate;
            //        }


            //    }
            //    else
            //    {
            //        TempData["procuraCAT"] = procuraCate;
            //    }



            //}
            //TempData.Keep("procuraCAT");

            //numero de linhas
            ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 30;


            ordenacao = String.IsNullOrEmpty(ordenacao) ? "Produto_asc" : ordenacao; //Se nao vier nula a ordenacao aplicar por produto decrescente
            ViewBag.ParametroProduto = ordenacao;

            //atribui 1 a pagina caso os parametros nao sejam nulos
            page = (procuraCEST != null) || (procuraNCM != null) || (procuraCate != null) ? 1 : page; //atribui 1 à pagina caso procurapor seja diferente de nullo

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

            //Aplica a origem e destino selecionada
            this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.UF_ORIGEM == this.ufOrigem && s.UF_DESTINO == this.ufDestino);


            switch (ViewBag.FiltroCorrenteAuditado)
            {
                case "0": //SOMENTE OS NÃO AUDITADOS
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.AUDITADONCM == 0);
                    break;
                case "1": //SOMENTE OS AUDITADOS
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.AUDITADONCM == 1);
                    break;
                case "2": //TODOS
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.ID != 0);
                    break;
            }

            this.tributacaoMTX_NCMView = ProcurarPorNCM_CEST_PARA_NCM(procuraCEST, procuraNCM, tributacaoMTX_NCMView);

            //Busca por categoria
            if (!String.IsNullOrEmpty(procuraCate))
            {
                this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.ID_CATEGORIA.ToString() == procuraCate);


            }
            switch (ordenacao)
            {
                case "Produto_desc":
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.OrderByDescending(s => s.NCM);
                    break;
                case "Produto_asc":
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.OrderBy(s => s.NCM);
                    break;
                case "Id_desc":
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.OrderBy(s => s.ID);
                    break;
                default:
                    this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.OrderBy(s => s.NCM);
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

            return View(this.tributacaoMTX_NCMView.ToPagedList(numeroPagina, tamanhoPagina));//retorna o pagedlist

           
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
        public ActionResult TributacaoNcmEditMassaNCMModalPost(string ncm, string fecp, string CodReceita, string CstSaidaPisCofins, string aliqSaidaCofins,
            string aliqSaidaPis, string IdFundamentoLegal, string CstVendaVarejoConsFinal, string alVeVarCF, string alVeVarCFSt, string rBcVeVarCF, string rBcSTVeVarCF,
            string CstVendaVarejoCont, string alVeVarCont, string alVeVarContSt, string rBcVeVarCont, string rBcSTVeVarCont, string CstVendaAtaCont,
            string aliqIcmsVendaAtaCont, string aliqIcmsSTVendaAtaCont, string redBaseCalcIcmsVendaAtaCont, string redBaseCalcIcmsSTVendaAtaCont,
            string CstVendaAtaSimpNacional, string alVSN, string alVSNSt, string rBcVSN, string rBcSTVSN, string IdFundLegalSaidaICMS, string cest, string ufOrigem,
            string ufDestino, string titulo)
        {
                     

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

            VerificaTempDataNCM(TempData["crt"].ToString(), TempData["regime"].ToString());

            //buscar os NCM dentro dessa lista
            if (!String.IsNullOrEmpty(buscaNCM))
            {
                this.tributacaoMTX_NCMView = this.tributacaoMTX_NCMView.Where(s => s.NCM == buscaNCM && s.UF_ORIGEM == ufDestino && s.UF_DESTINO == ufDestino);
            }

            //Foreach para buscar os itens na tabela de NCM a serem alterados
            TributacaoNCM trib_NCM = new TributacaoNCM(); //objeto que será alterado

            List<TributacaoNCMView> trb_NMC_List = this.tributacaoMTX_NCMView.ToList(); //lista com os itens selecionados
           


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
            }


            //zera a tempdata caso tenha salvo algum registros
            if (regSalvos > 0)
            {
               
                TempData["tributacao_MTX_NCMView"] = null;
                
                TempData.Keep("tributacao_MTX_NCMView"); //persiste
            }


            return RedirectToAction("TributacaoComNCM", new { param = retorno, qtdSalvos = regSalvos, qtdNSalvos = regNSalvos });



        }


        //temp data para nova action

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


        //SALVAR SELECIONADO - SO NCM E CEST
        public ActionResult TributacaoNcmEditMassaModalEditMassaModalComNCMPost(string strDados, string ncm, string cest)
        {

            //variaveis de auxilio
            int regSalvos = 0;
            int regNSalvos = 0;

            int regProdSalvos = 0;
            int regProdNSalvos = 0;

            string retorno = "";
            //PEGAR ESTADO ORIGEM E DESTINO E SE A EMPRESA É SIMPLES NACIONAL
            string uf_origem = TempData["UfOrigem"].ToString();
            string uf_Destino = TempData["UfDestino"].ToString();

            int crtTrib = int.Parse(TempData["crt"].ToString());
            int regimetrib =int.Parse(TempData["regime"].ToString());
          
            //ALTERA A TABELA DE TRIBUTAÇÃO POR NCM
            List<TributacaoNCM> tribNCM;

            //ALTERA O NCM NA TABELA DE PRODUTO PARA QUE ELE POSSA COMPARAR E AJUSTAR NO CLIENTE
            List<Produto> prod; //lista de produtos

            if (ncm != "")
            {
                string ncmReplace = ncm.Replace(".", "");
                ncmReplace = ncmReplace.Trim();

                //verifica se ja existe um ncm desse na tabela
                tribNCM = db.TributacoesNcm.Where(a => a.UF_Origem == uf_origem && a.UF_Destino == uf_Destino && a.crt == crtTrib && a.regime_trib == regimetrib && a.ncm.Equals(ncmReplace)).ToList();
                              



                //FAZER COM QUE O SISTEMA PROCURE NCM NO CLIENTE E ALTERE TAMBEM, BEM COMO NOS PRODUTOS
                if (tribNCM.Count > 1)
                {
                    int qtd = 0;
                    for (int a = 0; a > tribNCM.Count - 1; a++) //dessa forma ele so deixa um registro, nao impota a quantidade, e esse registro que sera excluido
                    {
                        qtd++;
                    }

                    for (int i = 0; i < qtd; i++)
                    {
                        TributacaoNCM tribDel = db.TributacoesNcm.Find(tribNCM[i].id);
                        db.TributacoesNcm.Remove(tribDel);
                        db.SaveChanges();
                    }


                }
                //buscando novamente apos a exclusão
                tribNCM = db.TributacoesNcm.Where(a => a.UF_Origem == uf_origem && a.UF_Destino == uf_Destino && a.crt == crtTrib && a.regime_trib == regimetrib && a.ncm.Equals(ncmReplace)).ToList();

                strDados = tribNCM[0].id.ToString();

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


            //objeto tributacao ncm
            TributacaoNCM tNCM = new TributacaoNCM();

            //busca o objeto da tributacao em ncm para pegar o ncm antigo
            int idTNCM_Antigo = Int32.Parse(idTribNCM[0]);
            tNCM = db.TributacoesNcm.Find(idTNCM_Antigo);

           

            //para tabela de trib ncm
            if (tNCM != null)
            {


                if (cestMudar != null)
                {
                    
                    if (cestMudar != "")
                    {

                        //tabela de produto
                        //busca o produto com o ncm antigo, que ainda nao foi alterado
                        prod = db.Produtos.Where(a => a.ncm.Equals(tNCM.ncm)).ToList();

                        //agora ele varre essa lista buscando o ncm que dever ser mudado
                        //TO-DO 
                        //verificar na tabela de produto
                        if (prod.Count > 0)
                        {
                            int qtd = prod.Count();

                            for (int i = 0; i < qtd; i++)
                            {
                                //montar o objeto que sera alterado para o novo ncm
                                Produto altProd = db.Produtos.Find(prod[i].Id);
                                altProd.cest = cestMudar; //recebeu o novo ncm
                                altProd.dataAlt = DateTime.Now; //alterar
                                                                //try para salvar
                                try
                                {
                                    db.SaveChanges();
                                    regProdSalvos++;
                                }
                                catch (Exception e)
                                {

                                    regProdNSalvos++;
                                }

                            }
                        }

                        tNCM.cest = cestMudar;
                        tNCM.auditadoPorNCM = 1; //flag que foi auditado pelo ncm
                        tNCM.dataAlt = DateTime.Now; //data da alteração
                    }

                }
                else
                {
                    //verficar se é para mudar o cest, se ele estiver true, quer dizer que deve ser mudado o cest de qq jeito
                    if (mudarCest)
                    {
                        //tabela de produto
                        //busca o produto com o ncm antigo, que ainda nao foi alterado
                        prod = db.Produtos.Where(a => a.ncm.Equals(tNCM.ncm)).ToList();

                        //agora ele varre essa lista buscando o ncm que dever ser mudado
                        //TO-DO 
                        //verificar na tabela de produto
                        if (prod.Count > 0)
                        {
                            int qtd = prod.Count();

                            for (int i = 0; i < qtd; i++)
                            {
                                //montar o objeto que sera alterado para o novo ncm
                                Produto altProd = db.Produtos.Find(prod[i].Id);
                                altProd.cest = cestMudar; //recebeu o novo ncm
                                altProd.dataAlt = DateTime.Now; //alterar
                                                                //try para salvar
                                try
                                {
                                    db.SaveChanges();
                                    regProdSalvos++;
                                }
                                catch (Exception e)
                                {

                                    regProdNSalvos++;
                                }

                            }
                        }


                        tNCM.cest = cestMudar;
                        tNCM.auditadoPorNCM = 1; //flag que foi auditado pelo ncm
                        tNCM.dataAlt = DateTime.Now; //data da alteração
                    }
                }
                //verificar se veio nulo
                if (ncmMudar != null)
                {
                    if (ncmMudar != "") //VERIFICAR SE VEIO VAZIO
                    {
                        
                        if (tNCM.ncm != ncmMudar)
                        {
                            //tabela de produto
                            //busca o produto com o ncm antigo, que ainda nao foi alterado
                            prod = db.Produtos.Where(a => a.ncm.Equals(tNCM.ncm)).ToList();

                            //agora ele varre essa lista buscando o ncm que dever ser mudado
                            //TO-DO 
                            //verificar na tabela de produto
                            if (prod.Count > 0)
                            {
                                int qtd = prod.Count();

                                for (int i = 0; i < qtd; i++)
                                {
                                    //montar o objeto que sera alterado para o novo ncm
                                    Produto altProd = db.Produtos.Find(prod[i].Id);
                                    altProd.ncm = ncmMudar; //recebeu o novo ncm
                                    altProd.dataAlt = DateTime.Now; //alterar
                                                                    //try para salvar
                                    try
                                    {
                                        db.SaveChanges();
                                        regProdSalvos++;
                                    }
                                    catch (Exception e)
                                    {

                                        regProdNSalvos++;
                                    }

                                }
                            }

                            tNCM.ncm = ncmMudar;
                            tNCM.auditadoPorNCM = 1; //flag que foi auditado pelo ncm
                            tNCM.dataAlt = DateTime.Now; //data da alteração
                        }
                    }

                }

                //tNCM.auditadoPorNCM = 1;
               
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



                      
            if (regSalvos > 0)
            {
                retorno = "Registro Salvo com Sucesso!!";
                
                TempData["tributacao_MTX_NCMView"] = null;
                TempData.Keep("tributacao_MTX_NCMView"); //persiste
              
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

            ViewBag.Cest = tributacaoNcm.cest;
            ViewBag.Ncm = tributacaoNcm.ncm;


            //Condição para montar a viewbag: se existir natureza da receita cadastrada; caso contrario null
            ViewBag.NaturezaReceita = (tributacaoNcm.codNatReceita == null) ? null : db.NaturezaReceitas.Find(tributacaoNcm.codNatReceita).descricao;
            ViewBag.FundLegalPC = (tributacaoNcm.idFundamentoLegal == null) ? null : db.Legislacoes.Find(tributacaoNcm.idFundamentoLegal).fundLegal;
            ViewBag.FundLegalSaida = (tributacaoNcm.idFundLegalSaidaICMS == null) ? null : db.Legislacoes.Find(tributacaoNcm.idFundLegalSaidaICMS).fundLegal;
            ViewBag.FundLegalEndrada = (tributacaoNcm.idFundLelgalEntradaICMS == null) ? null : db.Legislacoes.Find(tributacaoNcm.idFundLelgalEntradaICMS).fundLegal;
            ViewBag.Categoria = (tributacaoNcm.categoria == null) ? null : db.CategoriaProdutos.Find(tributacaoNcm.categoria).descricao;


            //ViewBag.Legislacao = db.Legislacoes.Find(tributacao.idFundLelgalEntradaICMS).id;
            ViewBag.CreditoOutorgado = tributacaoNcm.creditoOutorgado;
            ViewBag.Regime2560 = tributacaoNcm.regime2560;
            ViewBag.DataInicio = tributacaoNcm.inicioVigenciaMVA;
            ViewBag.DataFim = tributacaoNcm.fimVigenciaMVA;

            ViewBag.UfOrigem = tributacaoNcm.UF_Origem;
            ViewBag.UfDestino = tributacaoNcm.UF_Destino;
            ViewBag.DtaAlt = tributacaoNcm.DataFormatada;
            ViewBag.DataAlta = tributacaoNcm.dataAlt;


            return View(tributacaoNcm);
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


        //Verifica a tributação e regime
        public EmptyResult VerificaTempDataNCM(string regime, string crt)
        {
            //vai filtrar pelo regime e pelo crt

            int regimeTrib = int.Parse(regime);
            int crtTrib = int.Parse(crt);

            /*PAra tipar */
            /*A lista é salva em uma tempdata para ficar persistida enquanto o usuario está nessa action
             na action de salvar devemos anular essa tempdata para que a lista seja carregada novamente*/
            if (TempData["tributacao_MTX_NCMView"] == null)
            {
                //this.tributacaoMTX_NCMView = (IQueryable<TributacaoNCMView>)db.TributacoesNcmView.Where(a => a.CRT == crtTrib && a.REGIME_TRIBUTARIO == regimeTrib).ToList();
              this.tributacaoMTX_NCMView = from a in db.TributacoesNcmView where a.CRT == crtTrib && a.REGIME_TRIBUTARIO == regimeTrib select a; //FILTRA O QUE FOR SIMPLES NACIONAL

               
                TempData["tributacao_MTX_NCMView"] = this.tributacaoMTX_NCMView; //cria a temp data e popula
                TempData.Keep("tributacao_MTX_NCMView"); //persiste

            }
            else
            {
                
                this.tributacaoMTX_NCMView = (IQueryable<TributacaoNCMView>)TempData["tributacao_MTX_NCMView"];
                TempData.Keep("tributacao_MTX_NCMView"); //persiste
            }


           
            return new EmptyResult();
        }


        //aplica a tributação na tempdada
        private EmptyResult VerificaTributacao(string crt, string regime)
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
                    TempData["tributacao_MTX_NCMView"] = null;
                    TempData.Keep("tributacao_MTX_NCMView");
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
                    TempData["tributacao_MTX_NCMView"] = null;
                    TempData.Keep("tributacao_MTX_NCMView");
                }
                TempData["regime"] = (TempData["regime"] == null) ? "2" : regime;
                TempData.Keep("regime");
            }

            return new EmptyResult();
        }


        //ORIGEM E DESTINO
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

        private IQueryable<TributacaoNCMView> ProcurarPorNCM_CEST_PARA_NCM(string procuraCEST, string procuraNCM, IQueryable<TributacaoNCMView> tributacaoMTX_NCMView)
        {
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                tributacaoMTX_NCMView = tributacaoMTX_NCMView.Where(s => s.CEST.ToString().StartsWith(procuraCEST.ToString()));
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
                tributacaoMTX_NCMView = tributacaoMTX_NCMView.Where(s => s.NCM.ToString().StartsWith(procuraNCM.ToString()));

            }

            return tributacaoMTX_NCMView;

        }
    }
}