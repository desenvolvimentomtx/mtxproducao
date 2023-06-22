using MatrizTributaria.Areas.Cliente.Models;
using MatrizTributaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class SimpleHomeController : Controller
    {
        readonly private MatrizDbContext db = new MatrizDbContext();
        readonly private MatrizDbContextCliente dbCliente = new MatrizDbContextCliente(); //contexto do banco

        List<TributacaoEmpresa> dadosClienteBkp = new List<TributacaoEmpresa>(); //Dados do cliente em outro banco

        //LITA COM A ANALISE POR NCM
        List<AnaliseTributariaNCM> analise_NCM = new List<AnaliseTributariaNCM>(); //por ncm

        Empresa empresa;
        // GET: Cliente/SimpleHome
        public ActionResult Index(
            string cnpj, 
            int? numeroLinhas, 
            string ufOrigem,
            string ufDestino,
            string opcao,
            string param,
            string qtdNSalvos,
            string qtdSalvos,
            string ordenacao,
            string procuraPor,
            string procuraNCM,
            string procuraCEST,
            string filtroCorrente,
            string filtroCorrenteNCM,
            string filtroCorrenteCest,
            string filtroNulo,
            int? page )
        {
            //usar esse link
            //https://localhost:44324/Cliente/SimpleHome/Index?cnpj=15202475000219

            //formatando o cnpj
            string cnpjRecebido = FormatCNPJ(cnpj); 
    
            //pegando a empresa
            this.empresa = (from a in db.Empresas where a.cnpj == cnpjRecebido select a).FirstOrDefault(); 


            //passando o nome fantasia para a view
            ViewBag.Empresa = this.empresa.fantasia;

            //pegando o estado da empresa
            ViewBag.UfOrigem = this.empresa.estado;
            ViewBag.UfDestino = this.empresa.estado;

            VerificaTempDataEmpresa(this.empresa.cnpj);

            ViewBag.DadosClientes = this.dadosClienteBkp;

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            //Colocar o CRT em viewBags
            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;


            //converte em long caso seja possivel e atribui à variavel tipada:
            //isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);


            //verifica se veio parametros na busca
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 50
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            //NESTE CASO, NAO HAVERA OPÇAO, POIS TODOS OS PRODUTOS DEVERÃO SER MOSTRADOS NA TELA
            //VerificarOpcaoRed(filtroNulo, opcao);
            //opcao = TempData["opcao"].ToString();



            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor  = (procuraPor  == null) ? filtroCorrente     : procuraPor;
            procuraNCM  = (procuraNCM  == null) ? filtroCorrenteNCM  : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM  = procuraNCM; 
            ViewBag.FiltroCorrente     = procuraPor;

            //aplica estado origem e destino
            ViewBag.UfOrigem  = this.empresa.estado;
            ViewBag.UfDestino = this.empresa.estado;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            //colocar as pesquisas a partir desse ponto
            // veriricar estrategia de como serao colocadas

            return View();


        }


        /*ACTIONS AUXILIARES*/
        public EmptyResult VerificaTribNMCEmpresa(string crt, string regime)
        {
            if (TempData["analise_trib_Cliente_NCm"] == null)
            {
                //this.analise_NCM = db.Analise_TributariaNCM.Where(a => a.CNPJ_EMPRESA.Equals(this.empresa.cnpj) && a.CRT_BASE == (int)this.empresa.crt && a.REGIME_TRIB_BASE == (int)this.empresa.regime_trib).ToList();
                this.analise_NCM = (from a in db.Analise_TributariaNCM where a.CNPJ_EMPRESA.Equals(this.empresa.cnpj) && a.CRT_BASE == (int)this.empresa.crt && a.REGIME_TRIB_BASE == (int)this.empresa.regime_trib select a).ToList();

                TempData["analise_trib_Cliente_NCm"] = this.analise_NCM;
                TempData.Keep("analise_trib_Cliente_NCm");

            }
            else
            {
                this.analise_NCM = (List<AnaliseTributariaNCM>)TempData["analise_trib_Cliente_NCm"];

                TempData.Keep("analise_trib_Cliente_NCm");
            }



            return new EmptyResult();

        }


        public static string FormatCNPJ(string CNPJ)
        {
            return Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");
        }

        //VERIFICAR A TEMPDATA DA EMPRESA NO OUTRO BANCO
        public EmptyResult VerificaTempDataEmpresa(string cnpj)
        {
            if (TempData["dadosOutroBanco"] == null)
            {
                TempData["dadosOutroBanco"] = dbCliente.TributacaoEmpresas.Where(a => a.CNPJ_EMPRESA == this.empresa.cnpj).ToList();
                this.dadosClienteBkp = (List<TributacaoEmpresa>)TempData["dadosOutroBanco"];
                TempData.Keep("dadosOutroBanco");

            }
            else
            {
                this.dadosClienteBkp = (List<TributacaoEmpresa>)TempData["dadosOutroBanco"];
                TempData.Keep("dadosOutroBanco");
            }

            return new EmptyResult();
        }

        public EmptyResult VerificarLinhas(int? numeroLinhas)
        {
            //vefifica o numero de linhas se está nulo
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
                    }

                }
                else
                {
                    //se o numero de linhas não estiver salvo na tempdata ele vai pegar o numero de linhas
                    TempData["linhas"] = numeroLinhas;
                    TempData.Keep("linhas");
                    ViewBag.NumeroLinhas = numeroLinhas;

                }


            }
            else
            {
                if (TempData["linhas"] == null)
                {
                    TempData["linhas"] = 50;
                    TempData.Keep("linhas");
                    ViewBag.NumeroLinhas = 50;
                }
                else
                {
                    ViewBag.NumeroLinhas = TempData["linhas"];
                }
            }
            return new EmptyResult();
        }
    }
}