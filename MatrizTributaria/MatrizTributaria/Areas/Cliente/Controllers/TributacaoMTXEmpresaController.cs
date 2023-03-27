using MatrizTributaria.Models;
using MatrizTributaria.Areas.Cliente.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using MatrizTributaria.Models.ViewModels;
using System.Security.Cryptography;
//ALTERAÇÃO SO NA BRANCH DEV PAULO
namespace MatrizTributaria.Areas.Cliente.Controllers
{
    public class TributacaoMTXEmpresaController : Controller
    {
        //OBEJTOS DE APOIO

        readonly private MatrizDbContext db = new MatrizDbContext(); //contexto do banco
        readonly private MatrizDbContextCliente dbCliente = new MatrizDbContextCliente(); //contexto do banco

        //Empresa da sessão
        Empresa empresa;

        //origem e destino
        string ufOrigem = "";
        string ufDestino = "";

        //LITA COM A ANALISE POR NCM
        List<AnaliseTributariaNCM> analise_NCM = new List<AnaliseTributariaNCM>(); //por ncm

        List<TributacaoEmpresa> dadosClienteBkp = new List<TributacaoEmpresa>(); //por ncm

        // GET: Cliente/TributacaoMTXEmpresa
        public ActionResult Index()
        {
            return View();
        }


        //METODO PARA DESCREVER AS ANALISES EM FORMA DE GRÁFICO NA TELA
        [HttpGet]
        public ActionResult AnaliseTributaria(string ufOrigem, string ufDestino, string crt, string regime)
        {
            //VERIFICAR SESSÃO DO USUÁRIO
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
            }

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //pega a verificação pelo ncm AGORA TEM QUE PASSAR O CRT E O REGIME
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            //TOTAL DE REGISTROG
            ViewBag.TotalRegistros = this.analise_NCM.Count(a => a.TE_ID > 0 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            /*     ANALISE USANDO O NCM    */

            /*FIGURA: VENDA NO VAREJO PARA CONSUMIDOR FINAL*/
            ViewBag.AlqICMSVarejoCFMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null); //NÃO PODE SER NULA
            ViewBag.AlqICMSVarejoCFNullaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)); //onde nao for nulo no cliente mas no mtx sim
            ViewBag.AlqICMSVarejoCFNullaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFNullaAmbos = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFUsoConsumo = this.analise_NCM.Count(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            ViewBag.AlqICMSVarejoCFNullaExternoIsenta = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFNullaExternoNaoTrib = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVarejoCFSubsTributaria = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*FIGURA: VENDA NO VAREJO PARA CONSUMIDOR FINAL EM ST: CST 60 TEM QUE SER IGUAL - ok*/
            ViewBag.AlqICMSSTVarejoCFMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVarejoCFMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVarejoCFIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVarejoCFNulaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVarejoCFNulaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVarejoCFNulaAmbos = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
         

            /*FIGURA: VENDA NO VAREJO PARA CONTRIBUINTE*/
            ViewBag.AlqICMSVendaVContMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContIguais = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContNulasInternos = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContNulasExternos = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContNulasNulaAmbos = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            ViewBag.AlqICMSVendaVContUsoConsumo = this.analise_NCM.Count(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContIsenta = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContNaoTrib = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVendaVContSubsTributaria = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*FIGURA: VENDA NO VAREJO EMM ST PARA CONTRIBUINTE - ok*/
            ViewBag.AlqICMSSTVendaVContMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVendaVContMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVendaVContIguais = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVendaVContNulasInternos = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVendaVContNulasExternos = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVendaVContNulasNulaAmbos = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
         

            /*FIGURA: VENDA NO ATACADO PARA CONTRIBUINTE*/
            ViewBag.AlqICMSVataMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.ALIQ_ICMS_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataNulaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataNulaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataNulaAmbos = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataUsoConsumo = this.analise_NCM.Count(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataIsenta = this.analise_NCM.Count(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataNaoTrib = this.analise_NCM.Count(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSVataSubsTributaria = this.analise_NCM.Count(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));




            /*Aliq ICMS ST venda ATA - ok*/
            ViewBag.AlqICMSSTVataMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVataMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVataIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVataNulaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVataNulaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqICMSSTVataNulaAMBOS = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
         
            /*Aliq ICMS Vendo no atacado para Simples Nacional - ok*/
            ViewBag.AliqICMSVendaAtaSimpNacionalMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalNulaAMBOS = this.analise_NCM.Count(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalUsoConsumo = this.analise_NCM.Count(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalIsenta = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalNTrib = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSVendaAtaSimpNacionalSubsTributaria = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Aliq ICMS ST Venda no atacado para Simples Nacional - ok*/
            ViewBag.AliqICMSSTVendaAtaSimpNacionalMaior = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSSTVendaAtaSimpNacionalMenor = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSSTVendaAtaSimpNacionalIgual = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSSTVendaAtaSimpNacionalNulaInterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSSTVendaAtaSimpNacionalNulaExterno = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqICMSSTVendaAtaSimpNacionalNulaAMBOS = this.analise_NCM.Count(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            return View();

           

        } //fim action inicial
                    

        [HttpGet]
        public ActionResult AnaliseRedBaseCalSai(string ufOrigem, string ufDestino, string crt, string regime)
        {
            //VERIFICAR SESSÃO DO USUÁRIO
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
            }

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //pega a verificação pelo ncm AGORA TEM QUE PASSAR O CRT E O REGIME
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            //TOTAL DE REGISTROG
            ViewBag.TotalRegistros = this.analise_NCM.Count(a => a.TE_ID > 0 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));




            /*Aliq Redução da Base Calc ICMS venda CF OK */
            ViewBag.AlqRBCIcmsCFMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFNullaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFNullaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFNullaAMBOS = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsCFSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70);




            /*Aliq Redução Base Calc ICMS ST venda CF OK*/
            ViewBag.AlqRBCIcmsSTCFMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFIguais = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFNullaInternos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFNullaExternos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFNullaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRBCIcmsSTCFSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70);



            /*Reedução Base de Calculo venda varejo contribuinte OK*/
            ViewBag.AlqRDBCICMSVendaVarContMarior = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContIguais = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContNulaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContNulaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSVendaVarContSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Reedução Base de Calculo ST venda varejo contribuinte ok */
            ViewBag.AlqRDBCICMSSTVendaVarContMarior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContNulaInterna = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContNulaExterna = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqRDBCICMSSTVendaVarContSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Red Base Calc  ICMS  venda ATA PARA CONTRIBUINTE*/
            ViewBag.RedBaseCalcICMSVataMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataNulaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataNulaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVataSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Red Base Calc  ICMS ST  venda ATA PARA CONTRIBUINTE*/
            ViewBag.RedBaseCalcICMSSTVataMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVataMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVataIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVataNulaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVataNulaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVataNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            ViewBag.RedBaseCalcICMSSTVataSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Redução base de calc ICMS venda no atacado para Simples Nacional*/
            ViewBag.RedBaseCalcICMSVATASimpNacionalMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVATASimpNacionalMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVATASimpNacionalIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVATASimpNacionalNulaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVATASimpNacionalNulaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSVATASimpNacionalNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            ViewBag.RedBaseCalcICMSVATASimpNacionalSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));




            /*Redução base de calc ICMS ST venda no atacado para Simples Nacional*/
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalMaior = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalMenor = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalIgual = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalNulaInterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalNulaExterno = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.RedBaseCalcICMSSTVATASimpNacionalNulaAmbos = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            ViewBag.RedBaseCalcICMSSTVATASimpNacionalSemReducao = this.analise_NCM.Count(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            return View();
        }

        //REDULÇAO BASE DE CALC VENDA VAR CONS. FINAL

        //EdtCliAliqRedBasCalcIcmsVenVarCFMassa
        //EdtCliAliqRedBasCalcIcmsVenVarCFMassaTODOS
        //EdtCliAliqRedBasCalcIcmsVenVarCFMassaMODAL


        /*Edição Red Base de Calc ICMS Venda Consumidor Final*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS Venda para CONSUMIDOR FINAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");


            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();
                  
                    if (trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = analiseNCM.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }




                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }



                return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }


        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarCFMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {
                //TO-DO
                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = this.analise_NCM[i].RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";



            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");

            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });
        }





        //ANALISE PIS COFINS - ALIQUOTAS - 03/2023
        // ACTIONS:
        // EdtCliAliqSaidaPisMassa - INDEX
        // EdtCliAliqSaidaPisMassaTODOS - ALTERAÇÃO DE TODOS
        // EdtCliAliqSaidaPisMassaMODAL - ALTERAÇÃO INDIVIDUAL

        [HttpGet]
        public ActionResult AnalisePisCofins(string ufOrigem, string ufDestino, string crt, string regime)
        {
            //VERIFICAR SESSÃO DO USUÁRIO
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
            }

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //pega a verificação pelo ncm AGORA TEM QUE PASSAR O CRT E O REGIME
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            //TOTAL DE REGISTROG
            ViewBag.TotalRegistros = this.analise_NCM.Count(a => a.TE_ID > 0 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


           
            /*Aiquota Saida PIS*/
            ViewBag.AlqSPMaior = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSPMenor = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSPIguais = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSPNulaInterno = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSPNulaCliente = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AliqSPNulaAmbos = this.analise_NCM.Count(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

          
            /*Aliquota saida cofins*/
            ViewBag.AlqSaidaCofinsMaior = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSaidaCofinsMenor = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSaidaCofinsIguais = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSCNullaInterna = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSCNullaCliente = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.AlqSCNullaAmbos = this.analise_NCM.Count(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));

            return View();
        }



        //ANALISE   red bas cal icms st ven var cf
        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaMODAL - ALTERAÇÃO INDIVIDUAL


        /*Edição Red Base de Calc ICMS ST Venda Consumidor Final*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda para CONSUMIDOR FINAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

          

            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao





            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //Sem Redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }




        [HttpGet]           
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = this.analise_NCM[i].RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");



            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });
        }


        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarCFMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);


                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL != analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }

                  



                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }





        
                TempData["analise"] = null;
                return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            

        }



        //ANALISE   red bas cal icms st ven var cf
        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsVenVarContMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsVenVarContMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsVenVarContMassaMODAL - ALTERAÇÃO INDIVIDUAL



        /*Edição Red Base de Calc ICMS Venda varejo para contrbuinte*/
        [HttpGet]
       public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassa(
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
            int? page, 
            int? numeroLinhas)
         {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }


            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS Venda para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa


            //Monta as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


            //origem e destino
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 20 && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL > a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL < a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL != 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL == 0.00 && a.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == 0.00 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ambos
                            this.analise_NCM = this.analise_NCM.Where(a => a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 20 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 70).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }



            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);


            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }



        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT < a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_VENDA_VAREJO_CONT = this.analise_NCM[i].RED_BASE_CALC_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });
        }


        
       [HttpGet]
       public ActionResult EdtCliAliqRedBasCalcIcmsVenVarContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();
                   
                    
                    if (trib.RED_BASE_CALC_VENDA_VAREJO_CONT == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_VENDA_VAREJO_CONT != analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_VENDA_VAREJO_CONT = analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT == null) ? analiseTrib : (trib.RED_BASE_CALC_VENDA_VAREJO_CONT);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_VENDA_VAREJO_CONT = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_VENDA_VAREJO_CONT = analiseNCM.RED_BASE_CALC_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }

           
                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";


            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }





                TempData["analise"] = null;
                return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }





        //ANALISE   red bas cal icms st ven var cf
        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsSTVenVarContMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsSTVenVarContMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsSTVenVarContMassaMODAL - ALTERAÇÃO INDIVIDUAL


        /*Edição Red Base de Calc ICMS ST Venda varejo para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassa(
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
            int? page,
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Varejo para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");


            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();


            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //origem e destino
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }



            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }



        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            //VerificaTempData();
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT < a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null && a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = this.analise_NCM[i].RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";



            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");

            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }



        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenVarContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT != analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {

                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT == null) ? analiseTrib : (trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = analiseNCM.RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }




                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }






            TempData["analise"] = null;
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });




        }





        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsVenAtaContMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsVenAtaContMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsVenAtaContMassaMODAL - ALTERAÇÃO INDIVIDUAL

   
        /*Edição Red Base de Calc ICMS  Venda Atacado para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassa(string ufOrigem, string ufDestino, string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS  Venda Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);



            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;



            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //sem redução
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 20 && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            //this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }




            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

       [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA > a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA < a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_VENDA_ATA = this.analise_NCM[i].RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";



            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");

            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }

        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaContMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ICMS_VENDA_ATA == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_VENDA_ATA != analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA = analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_VENDA_ATA);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA = analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }

                    }





                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }



                TempData["analise"] = null;
                return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            
        }



        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaMODAL - ALTERAÇÃO INDIVIDUAL

        /*Edição Red Base de Calc ICMS ST  Venda Atacado para contrbuinte*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa(string ufOrigem, string ufDestino, string opcao, string param, string qtdNSalvos, string qtdSalvos, string ordenacao, string procuraPor, string procuraNCM, string procuraCEST, string filtroCorrente, string filtroCorrenteNCM, string filtroCorrenteCest, string filtroNulo, int? page, int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();



            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;

                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.CST_VENDA_ATA_CONT_BASE == 70 && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = this.analise_NCM[i].RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }


        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL


            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA != analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }
       


                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";


            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }






                TempData["analise"] = null;
                return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            

        }


        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsVenAtaSNMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsVenAtaSNMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsVenAtaSNMassaMODAL - ALTERAÇÃO INDIVIDUAL


        
        /*Edição Red Base de Calc ICMS  Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassa
            (
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS  Venda Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //variavel auxiliar
            string resultado = param;

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();



            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();


            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 20 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 20 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = this.analise_NCM[i].RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";

            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }

        

        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsVenAtaSNMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL


            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL != analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = analiseNCM.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }

                

                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }



            
                TempData["analise"] = null;
                return RedirectToAction("EdtCliAliqRedBasCalcIcmsVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }


        // ACTIONS:
        //EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa - INDEX
        //EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaTODOS - ALTERAÇÃO DE TODOS
        //EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaMODAL - ALTERAÇÃO INDIVIDUAL
 
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Redução Base de Calc. ICMS ST Venda Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();



            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            VerificarOpcaoRed(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Sem Redução":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7":
                            this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;


            }//fim do switche

            //Action para procurar
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        
        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 70 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = this.analise_NCM[i].RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }




        [HttpGet]
        public ActionResult EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassaModal(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null)
                    {
                        if (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null)
                        {
                            if (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString())
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null) ? analiseRetorno : (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : (trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL).Replace(",", ".");
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = "0.000";
                            }
                            else
                            {
                                trib.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }

     



                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }




            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqRedBasCalcIcmsSTVenAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }



        /*Edição Aliquota de Pis de saída*/
        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Saída para PIS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão



            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            /*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();
        

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //VerificaTempData();

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":

                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":

                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == a.ALIQ_SAIDA_PIS_BASE && a.ALIQ_SAIDA_PIS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS != null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;



            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            ViewBag.CstPisCofinsS = db.CstPisCofinsSaidas.ToList();

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        
        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa


            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.ALIQ_SAIDA_PIS == null)
                    {
                        if (analiseNCM.ALIQ_SAIDA_PIS_BASE != null)
                        {
                            if (trib.ALIQ_SAIDA_PIS != analiseNCM.ALIQ_SAIDA_PIS_BASE.ToString())
                            {
                                trib.ALIQ_SAIDA_PIS = analiseNCM.ALIQ_SAIDA_PIS_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        //pegar valores

                        analiseRetorno = (analiseNCM.ALIQ_SAIDA_PIS_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_SAIDA_PIS_BASE).ToString();
                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem
                        analiseTrib = (analiseNCM.ALIQ_SAIDA_PIS_BASE == null) ? analiseTrib : (trib.ALIQ_SAIDA_PIS).ToString();


                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++; //se são iguais não salva
                        }
                        else
                        { //se são diferentes
                            if (analiseRetorno == null)
                            {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                                trib.ALIQ_SAIDA_PIS = "0.000";
                            }
                            else
                            {
                                //caso contrario atribui o valor procurado na analise ao objeto instanciado
                                trib.ALIQ_SAIDA_PIS = analiseNCM.ALIQ_SAIDA_PIS_BASE.ToString().Replace(",", ".");
                            }

                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }
                    }

                   


                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";


            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaidaPisMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }


        
        [HttpGet]
        public ActionResult EdtCliAliqSaidaPisMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS > a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS < a.ALIQ_SAIDA_PIS_BASE && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_PIS == null && a.ALIQ_SAIDA_PIS_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;

            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_SAIDA_PIS = this.analise_NCM[i].ALIQ_SAIDA_PIS_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliAliqSaidaPisMassa", new { param = resultado, qtdSalvos = regSalv });


        }





        //ANALISE PIS COFINS - ALIQUOTAS - 03/2023
        // ACTIONS:
        // EdtCliAliqSaiCofinsMassa - INDEX
        // EdtCliAliqSaiCofinsMassaTODOS - ALTERAÇÃO DE TODOS
        // EdtCliAliqSaiCofinsMassaMODAL - ALTERAÇÃO INDIVIDUAL
        /*Edição Aliquota de  COFINS de saída*/


        /*Edição Aliquota cofins de saída*/
        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota de Saída para COFINS no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão




            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;


            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();


            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //VerificaTempData();

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":

                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS > a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS < a.ALIQ_SAIDA_COFINS_BASE && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == a.ALIQ_SAIDA_COFINS_BASE && a.ALIQ_SAIDA_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS != null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6":
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_SAIDA_COFINS == null && a.ALIQ_SAIDA_COFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            ViewBag.CstPisCofinsS = db.CstPisCofinsSaidas.ToList();
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        
        [HttpGet]
        public ActionResult EdtCliAliqSaiCofinsMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }

            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();



                    if (trib.ALIQ_SAIDA_COFINS == null)
                    {
                        if (analiseNCM.ALIQ_SAIDA_COFINS_BASE != null)
                        {
                            if (trib.ALIQ_SAIDA_COFINS != analiseNCM.ALIQ_SAIDA_COFINS_BASE.ToString())
                            {
                                trib.ALIQ_SAIDA_COFINS = analiseNCM.ALIQ_SAIDA_COFINS_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        /*Caso esteja nulo o retorno do valor a variavel continuar com 0 evitando erro de valores nulos*/
                        analiseRetorno = (analiseNCM.ALIQ_SAIDA_COFINS_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_SAIDA_COFINS_BASE).ToString();

                        analiseTrib = (analiseNCM.ALIQ_SAIDA_COFINS_BASE == null) ? analiseTrib : (trib.ALIQ_SAIDA_COFINS).ToString();


                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++; //se são iguais não salva
                        }
                        else
                        { //se são diferentes
                            if (analiseRetorno == null)
                            {  //se o valor continnuar 0 atribui-se ao valor na base de dados nulo
                                trib.ALIQ_SAIDA_COFINS = "0.000";
                            }
                            else
                            {
                                //caso contrario atribui o valor procurado na analise ao objeto instanciado
                                trib.ALIQ_SAIDA_COFINS = analiseNCM.ALIQ_SAIDA_COFINS_BASE.ToString().Replace(",", ".");
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }

                    }
                    //pegar valores


                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            //Redirecionar para a tela de graficos
            return RedirectToAction("EdtCliAliqSaiCofinsMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
        }
       // EdtCliAliqSaiCofinsMassaTODOS

        /*CST: Código de Situação Tributária*/
        [HttpGet]
        public ActionResult AnaliseCST(string ufOrigem, string ufDestino, string crt, string regime)
        { 
         //VERIFICAR SESSÃO DO USUÁRIO
            string usuarioSessao = ""; //variavel auxiliar
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            else
            {
                usuarioSessao = Session["usuario"].ToString(); //pega o usuário da sessão
            }

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            //pega a verificação pelo ncm AGORA TEM QUE PASSAR O CRT E O REGIME
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            //TOTAL DE REGISTROG
            ViewBag.TotalRegistros = this.analise_NCM.Count(a => a.TE_ID > 0 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Saída PIS Cofins OK 03/2023*/
            ViewBag.CstSaidaPisCofinsNulaCliente = this.analise_NCM.Count(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstSaidaPisCofinsNulaAmbos = this.analise_NCM.Count(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstSaidaPisCofinsNulaMtx = this.analise_NCM.Count(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstSaidaPisCofinsIgual = this.analise_NCM.Count(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstSaidaPisCofinsDife = this.analise_NCM.Count(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Saída VENDA VARENO CONSUMIDOR FINAL OK 03/2023*/
            ViewBag.CstVendaVarejoCFNulaCliente = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoCFNulaAmbos = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoCFNulaMtx = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoCFIgual = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoCFDif = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));


            /*Saída VENDA VAREJO CONTRIBUINTE OK 03/2023*/
            ViewBag.CstVendaVarejoContNulaCliente = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CSTVendaVarejoContNulaAmbos = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoContNulaMtx = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoContIgual = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaVarejoContDif = this.analise_NCM.Count(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Saída VENDA ATA SN OK 03/2023*/
            ViewBag.CstVendaAtaSNNulaCliente = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaSNNulaAmbos = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaSNNulaMtx = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaSNIgual = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaSNDif = this.analise_NCM.Count(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            /*Saída VENDA ATA CONTRIBUINTE OK 03/2023*/
            ViewBag.CstVendaAtaContNulaCliente = this.analise_NCM.Count(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaContNulaAmbos = this.analise_NCM.Count(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaContNulaMtx = this.analise_NCM.Count(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaContIgual = this.analise_NCM.Count(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));
            ViewBag.CstVendaAtaContDif = this.analise_NCM.Count(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino));



            return View();

        }


        //EDICOES CST A PARTIR DE 03/2023

        /*Edição de CST de Pis Cofins de Saída*/
        [HttpGet]
        public ActionResult EdtCliCstSaidaPisCofinsMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "CST de Saída para Pis Cofins no Cliente vs no MTX";


           
            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão


            //FAZER A VIEWBAG PARA COMPARAR O QUE JA TINHA NO CLIENTE O QUE VAI TER AGORA

            //Criar uma tempdata para esse recurso
            VerificaTempDataEmpresa(this.empresa.cnpj);
    
             ViewBag.DadosClientes = this.dadosClienteBkp;
            

           

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            VerificarLinhas(numeroLinhas);




            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            VerificarOpcao(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();
            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;



            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            ViewBag.CstPisCofinsS = db.CstPisCofinsSaidas.ToList();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Iguais":
                case "Cst Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Diferentes":
                case "Cst Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Nulos Cliente":
                case "Cst Nulos no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulos MTX":
                case "Cst Nulos no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulos Ambos":
                case "Cst Nulos em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PISCOFINS_BASE == null && a.CST_SAIDA_PIS_COFINS != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5":
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


        [HttpGet]
        public ActionResult EdtCliCstSaidaPisCofinsMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempDataSN();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Cst Diferentes")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS != a.CST_SAIDA_PISCOFINS_BASE && a.CST_SAIDA_PIS_COFINS != null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Cst Nulos no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_SAIDA_PIS_COFINS == null && a.CST_SAIDA_PISCOFINS_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.CST_SAIDA_PIS_COFINS = this.analise_NCM[i].CST_SAIDA_PISCOFINS_BASE.ToString();
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";

            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliCstSaidaPisCofinsMassa", new { param = resultado, qtdSalvos = regSalv });

        }


        [HttpGet]
        public ActionResult EdtCliCstSaidaPisCofinsMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa


            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui zero ao valor
            string analiseTrib = null; //atribui zero ao valor


            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.CST_SAIDA_PIS_COFINS == null)
                    {
                        if (analiseNCM.CST_SAIDA_PISCOFINS_BASE != null)
                        {
                            if (trib.CST_SAIDA_PIS_COFINS != analiseNCM.CST_SAIDA_PISCOFINS_BASE.ToString())
                            {
                                trib.CST_SAIDA_PIS_COFINS = analiseNCM.CST_SAIDA_PISCOFINS_BASE.ToString();

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {

                        analiseRetorno = (analiseNCM.CST_SAIDA_PISCOFINS_BASE == null) ? analiseRetorno : (analiseNCM.CST_SAIDA_PISCOFINS_BASE).ToString();
                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem
                        analiseTrib = (analiseNCM.CST_SAIDA_PIS_COFINS == null) ? analiseTrib : (trib.CST_SAIDA_PIS_COFINS).ToString();

                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.CST_SAIDA_PIS_COFINS = null;
                            }
                            else
                            {

                                trib.CST_SAIDA_PIS_COFINS = analiseNCM.CST_SAIDA_PISCOFINS_BASE.ToString();
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }

                    }



                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";



            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }

            
                //Redirecionar para a tela de graficos
                return RedirectToAction("EdtCliCstSaidaPisCofinsMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
            
        }


        [HttpGet]
        public ActionResult EdtCliCstVendaVarCFMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão


            //Mensagem do card
            ViewBag.Mensagem = "CST de Venda Varejo para Consumidor Final no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;
            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;



            VerificarLinhas(numeroLinhas);

            //Criar uma tempdata para esse recurso
            VerificaTempDataEmpresa(this.empresa.cnpj);

            ViewBag.DadosClientes = this.dadosClienteBkp;


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            VerificarOpcao(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();
            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


         
            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();


            //VerificaTempData();
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            /*Switch da opção*/
            switch (opcao)
            {
                case "Iguais":
                case "Cst Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Diferentes":
                case "Cst Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Nulos Cliente":
                case "Cst Nulos no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;





                    }
                    break;
                case "Nulos MTX":
                case "Cst Nulos no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulos Ambos":
                case "Cst Nulos em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";
            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

      
        //Editar todos os Registros
        [HttpGet]
        public ActionResult EdtCliCstVendaVarCFMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempDataSN();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Cst Diferentes")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL != a.CST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Cst Nulos no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.CST_VENDA_VAREJO_CONS_FINAL = this.analise_NCM[i].CST_VENDA_VAREJO_CONS_FINAL_BASE.ToString();
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action

                    //EdtCliCstVendaVarCFMassaSNTODOSCONTAGEM(opcao);


                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliCstVendaVarCFMassa", new { param = resultado, qtdSalvos = regSalv });

        }
       
        [HttpGet]
        public ActionResult EdtCliCstVendaVarCFMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;


            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();




            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.CST_VENDA_VAREJO_CONS_FINAL == null)
                    {
                        if (analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.CST_VENDA_VAREJO_CONS_FINAL != analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.CST_VENDA_VAREJO_CONS_FINAL = analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE.ToString();

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE).ToString();
                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem
                        analiseTrib = (analiseNCM.CST_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : (trib.CST_VENDA_VAREJO_CONS_FINAL).ToString();
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.CST_VENDA_VAREJO_CONS_FINAL = null;
                            }
                            else
                            {

                                trib.CST_VENDA_VAREJO_CONS_FINAL = analiseNCM.CST_VENDA_VAREJO_CONS_FINAL_BASE.ToString();
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }
                    }

                  

                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


            
                //Redirecionar para a tela de graficos
                return RedirectToAction("EdtCliCstVendaVarCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
            


        }






        /*Edição de CST de Venda varejo para Contribuinte*/
        [HttpGet]
        public ActionResult EdtCliCstVendaVarContMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "CST de Venda Varejo para Contribuinte no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão



            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();


            //Criar uma tempdata para esse recurso
            VerificaTempDataEmpresa(this.empresa.cnpj);

            ViewBag.DadosClientes = this.dadosClienteBkp;


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            VerificarOpcao(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

           
            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Iguais":
                case "Cst Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Diferentes":
                case "Cst Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Nulos Cliente":
                case "Cst Nulos no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulos MTX":
                case "Cst Nulos no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulos Ambos":
                case "Cst Nulos em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        


         [HttpGet]
        public ActionResult EdtCliCstVendaVarContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempDataSN();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Cst Diferentes")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT != a.CST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Cst Nulos no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT == null && a.CST_VENDA_VAREJO_CONT_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.CST_VENDA_VAREJO_CONT = this.analise_NCM[i].CST_VENDA_VAREJO_CONT_BASE.ToString();
                trib.DT_ALTERACAO = DateTime.Now;

                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliCstVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv });

        }

        [HttpGet]
        public ActionResult EdtCliCstVendaVarContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            //pega a empresa da seção
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa


            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.CST_VENDA_VAREJO_CONT == null)
                    {
                        if(analiseNCM.CST_VENDA_VAREJO_CONT_BASE != null)
                        {
                            if(trib.CST_VENDA_VAREJO_CONT != analiseNCM.CST_VENDA_VAREJO_CONT_BASE.ToString())
                            {
                                trib.CST_VENDA_VAREJO_CONT = analiseNCM.CST_VENDA_VAREJO_CONT_BASE.ToString();

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.CST_VENDA_VAREJO_CONT_BASE == null) ? analiseRetorno : (analiseNCM.CST_VENDA_VAREJO_CONT_BASE).ToString();

                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem
                        analiseTrib = (analiseNCM.CST_VENDA_VAREJO_CONT == null) ? analiseTrib : (trib.CST_VENDA_VAREJO_CONT).ToString();


                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.CST_VENDA_VAREJO_CONT = null;
                            }
                            else
                            {

                                trib.CST_VENDA_VAREJO_CONT = analiseNCM.CST_VENDA_VAREJO_CONT_BASE.ToString();
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }
                    }

                  
                   
                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");
              
                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }

    
                //Redirecionar para a tela de graficos
                return RedirectToAction("EdtCliCstVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
            
        }


        [HttpGet]
        public ActionResult EdtCliCstVendaAtaSNMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "CST de Venda Atacado para Simples Nacional no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Criar uma tempdata para esse recurso
            VerificaTempDataEmpresa(this.empresa.cnpj);

            ViewBag.DadosClientes = this.dadosClienteBkp;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            VerificarOpcao(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;



            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //origem e destino


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Iguais":
                case "Cst Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Diferentes":
                case "Cst Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulos Cliente":
                case "Cst Nulos no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulos MTX":
                case "Cst Nulos no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulos Ambos":
                case "Cst Nulos em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;



            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        //Alteração de todos os itens
        [HttpGet]
        public ActionResult EdtCliCstVendaAtaSNMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();
            //VerificaTempDataSN();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Cst Diferentes")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL != a.CST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Cst Nulos no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.CST_VENDA_ATA_SIMP_NACIONAL = this.analise_NCM[i].CST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString();
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliCstVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv });

        }

        [HttpGet]
        public ActionResult EdtCliCstVendaAtaSNMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //seleciona a empresa
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa


            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui zero ao valor
            string analiseTrib = null; //atribui zero ao valor

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

           


            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();
                  
                    if (trib.CST_VENDA_ATA_SIMP_NACIONAL == null)
                    {
                        if (analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE != null)
                        {
                            if (trib.CST_VENDA_ATA_SIMP_NACIONAL != analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString())
                            {
                                trib.CST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString();

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE == null) ? analiseRetorno : (analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE).ToString();
                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem

                        analiseTrib = (analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : (trib.CST_VENDA_ATA_SIMP_NACIONAL).ToString();


                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.CST_VENDA_ATA_SIMP_NACIONAL = null;
                            }
                            else
                            {

                                trib.CST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.CST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString();
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }
                    }
                  
              
                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }


     

                //Redirecionar para a tela de graficos
                return RedirectToAction("EdtCliCstVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
            

        }



        /*Edição de CST de Venda Atacado para Contribuinte*/
        [HttpGet]
        public ActionResult EdtCliCstVendaAtaContMassa(
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
            int? page,
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "CST de Venda Atacado para Contribuinte no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            /*Pegando o usuário e a empresa do usuário*/
            string user = Session["usuario"].ToString();

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            ViewBag.CstGeral = db.CstIcmsGerais.ToList();
            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");



            VerificarOpcao(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            //Criar uma tempdata para esse recurso
            VerificaTempDataEmpresa(this.empresa.cnpj);

            ViewBag.DadosClientes = this.dadosClienteBkp;

            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Iguais":
                case "Cst Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Diferentes":
                case "Cst Diferentes":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;




                    }
                    break;
                case "Nulos Cliente":
                case "Cst Nulos no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;




                    }
                    break;
                case "Nulos MTX":
                case "Cst Nulos no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;




                    }
                    break;
                case "Nulos Ambos":
                case "Cst Nulos em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //DIFERENTES
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://NULOS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULOS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULOS EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA == null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }
            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

      
        //alterar todos
        [HttpGet]
        public ActionResult EdtCliCstVendaAtaContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Cst Diferentes")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA != a.CST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Cst Nulos no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA == null && a.CST_VENDA_ATA_CONT_BASE != null && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {
                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.CST_VENDA_ATA = this.analise_NCM[i].CST_VENDA_ATA_CONT_BASE.ToString();
                trib.DT_ALTERACAO = DateTime.Now;

                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EdtCliCstVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv });
        }

        [HttpGet]
        public ActionResult EdtCliCstVendaAtaContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //Variaveis auxiliares
            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.CST_VENDA_ATA == null)
                    {
                        if (analiseNCM.CST_VENDA_ATA_CONT_BASE != null)
                        {
                            if (trib.CST_VENDA_ATA != analiseNCM.CST_VENDA_ATA_CONT_BASE.ToString())
                            {
                                trib.CST_VENDA_ATA = analiseNCM.CST_VENDA_ATA_CONT_BASE.ToString();

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.CST_VENDA_ATA_CONT_BASE == null) ? analiseRetorno : (analiseNCM.CST_VENDA_ATA_CONT_BASE).ToString();
                        //verifica na analise qual o valor que esta no cliente, se estiver nulo,continua 0 se nao pega o valor que tem
                        analiseTrib = (analiseNCM.CST_VENDA_ATA == null) ? analiseTrib : (trib.CST_VENDA_ATA).ToString();
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.CST_VENDA_ATA = null;
                            }
                            else
                            {

                                trib.CST_VENDA_ATA = analiseNCM.CST_VENDA_ATA_CONT_BASE.ToString();
                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos
                        }
                    }
          
                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                resultado = "Problemas ao salvar o registro: " + e.ToString();

            }



                //Redirecionar para a tela de graficos
                return RedirectToAction("EdtCliCstVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });
            
        }


        //EDIÇÕES a partir de 03/2023

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarfCFMassa
            (
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
            int? page, 
            int? numeroLinhas
            )
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            //variavel auxiliar
            string resultado = param;


            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS Venda no Varejo para consumidor final no Cliente X  no MTX";

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            VerificarLinhas(numeroLinhas);



            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();
            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;

            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

                        
            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":

                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";


                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;

                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Isentas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Não Tributadas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "8";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Uso Consumo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "9";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Subst. Tributária":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "10";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList(); //onde nao for nulo no cliente mas no mtx sim
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //Subt Tributária
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;





            }//fim do switche


            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }



      

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarfCFMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            //ViewBag.Tributacao = TempData["tributacao"].ToString();

            
            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempData();
            //VerificaTempData_por_NCM(TempData["tributacao"].ToString()); //manda verificar passando a tributacao

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 60 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 40 && a.CST_VENDA_VAREJO_CONS_FINAL_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Subst. Tributária")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Não Tributadas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }
            if (opcao == "Isentas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null || a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = this.analise_NCM[i].ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action


                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            //TempData["analise"] = null;

            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");
            //string[] idTrib = this.alanliseSN.
            //a analise vai me dar todos os ids


            return RedirectToAction("EditClienteAliqIcmsVendaVarfCFMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });


        }

    
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarfCFMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();

            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL != analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : (trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL);

                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = "0.000";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }
                  
                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }

           
                return RedirectToAction("EditClienteAliqIcmsVendaVarfCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


            //Redirecionar para a tela de graficos


        }




        //EditClienteAliqIcmsVendaVarSTCFMassa
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarSTCFMassa(
            string ufOrigem, 
            string ufDestino, 
            string opcao, string param, 
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Varejo para consumidor final no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão


            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            // ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            VerificarLinhas(numeroLinhas);




            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");


            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();


            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/

            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


         
            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino) && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULAS MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;


            ViewBag.CstGeral = db.CstIcmsGerais.AsNoTracking(); //para montar a descrição da cst na view
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


       

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarSTCFMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();


            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempData();
            //VerificaTempData_por_NCM(TempData["tributacao"].ToString()); //manda verificar passando a tributacao

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();


            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null && a.CST_VENDA_VAREJO_CONS_FINAL_BASE == 60 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = this.analise_NCM[i].ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            //TempData["analise"] = null;
            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");
            //string[] idTrib = this.alanliseSN.
            //a analise vai me dar todos os ids


            return RedirectToAction("EditClienteAliqIcmsVendaVarSTCFMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });
        }

        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarSTCFMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();
           
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();


                    if (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL != analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {

                        analiseRetorno = (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL == null) ? analiseTrib : (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL);

                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = "0.00";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }

                    }



                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }

            

                TempData["analise"] = null;
                return RedirectToAction("EditClienteAliqIcmsVendaVarSTCFMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }




        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarContMassa
        (
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
          int? page, 
          int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS  Venda no Varejo para CONTRIBUINTE no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;


            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão



            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();



            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");

            VerificarOpcaoAliq(filtroNulo, opcao);

            opcao = TempData["opcao"].ToString();




            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 

            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao






            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Isentas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Não Tributadas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "8";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Uso Consumo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "9";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Subst. Tributária":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "10";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULLA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLO EM AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST TRIBUTÁRIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;



            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }




        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();
            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //VerificaTempData();
            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT < a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE != 60 && a.CST_VENDA_VAREJO_CONT_BASE != 40 && a.CST_VENDA_VAREJO_CONT_BASE != 41 && a.PRODUTO_CATEGORIA != 21 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Subst. Tributária")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Não Tributadas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }
            if (opcao == "Isentas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_VAREJO_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_VAREJO_CONT == null || a.ALIQ_ICMS_VENDA_VAREJO_CONT != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_VENDA_VAREJO_CONT = this.analise_NCM[i].ALIQ_ICMS_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action

                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            //TempData["analise"] = null;
            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");
            //string[] idTrib = this.alanliseSN.
            //a analise vai me dar todos os ids


            return RedirectToAction("EditClienteAliqIcmsVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }


        
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaVarContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;

            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {
                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);


                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();


                    if (trib.ALIQ_ICMS_VENDA_VAREJO_CONT == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_VENDA_VAREJO_CONT != analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONT = analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE == null) ? analiseTrib : (trib.ALIQ_ICMS_VENDA_VAREJO_CONT);
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONT = "0.00";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_VENDA_VAREJO_CONT = analiseNCM.ALIQ_ICMS_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }
                   
                   
            
                }

                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";
            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }


          
                return RedirectToAction("EditClienteAliqIcmsVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }





        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaVarContMassa
            (string ufOrigem, 
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Varejo para CONTRIBUINTE no Cliente X  no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão




            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;

            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;


            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 
                                                  //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAL
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULLA NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLA AMBOS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE == null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;




            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }



        

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaVarContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao




            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT < a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null && a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE != null && a.CST_VENDA_VAREJO_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = this.analise_NCM[i].ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EditClienteAliqIcmsSTVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaVarContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL


            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT != analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT  = analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONT == null) ? analiseTrib : (trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT);

                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = "0.00";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = analiseNCM.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }

                   

                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }




            return RedirectToAction("EditClienteAliqIcmsSTVendaVarContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });




        }


        /*Edição ICMS Venda Atacado para Contribuinte*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaContMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS  Venda no Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;
            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão


            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();



            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);

            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            
            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;

            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();


            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Isentas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Não Tributadas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "8";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Uso Consumo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "9";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Subst. Tributária":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "10";

                    switch (ViewBag.Filtro)
                    {
                        case "1": //MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENORES
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.ALIQ_ICMS_VENDA_ATA != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4": //NULAS NO CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5": //NULLAS NO MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA != null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULLAS EM AMBOX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //ISENTAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //NÃO TRIBUTADA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //USO CONSUMO
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBS TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;
            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


        

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA > a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA < a.ALIQ_ICMS_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA == null && a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE != 60 && a.CST_VENDA_ATA_CONT_BASE != 40 && a.CST_VENDA_ATA_CONT_BASE != 41 && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Subst. Tributária")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null) && a.UF_ORIGEM.Equals(ufOrigem) && a.UF_DESTINO.Equals(ufDestino)).ToList();

            }
            if (opcao == "Não Tributadas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }
            if (opcao == "Isentas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_CONT_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA == null || a.ALIQ_ICMS_VENDA_ATA != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }


            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_VENDA_ATA = this.analise_NCM[i].ALIQ_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EditClienteAliqIcmsVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }

     
                    [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL


            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);


                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();
                    
                    if (trib.ALIQ_ICMS_VENDA_ATA == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_VENDA_ATA != analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_VENDA_ATA = analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_VENDA_ATA == null) ? analiseTrib : (trib.ALIQ_ICMS_VENDA_ATA);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_VENDA_ATA = "0.00";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_VENDA_ATA = analiseNCM.ALIQ_ICMS_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }

                    }




                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";


            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }




          
                return RedirectToAction("EditClienteAliqIcmsVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }



        /*Edição ICMS ST Venda Atacado para Contribuinte*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaContMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST  Venda no Atacado para CONTRIBUINTE no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);



            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

          
            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();

            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Isentas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;
                case "Não Tributadas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "8";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Uso Consumo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "9";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3"://IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.ALIQ_ICMS_ST_VENDA_ATA != null && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA != null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;



                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }


            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaContMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();


            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao

            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA > a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA < a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA == null && a.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null && a.CST_VENDA_ATA_CONT_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_ST_VENDA_ATA = this.analise_NCM[i].ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }

              

        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaContMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();
            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);


                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();


                    if (trib.ALIQ_ICMS_ST_VENDA_ATA == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_ST_VENDA_ATA != analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_ST_VENDA_ATA = analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA == null) ? analiseTrib : (trib.ALIQ_ICMS_ST_VENDA_ATA);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_ST_VENDA_ATA = "0.000";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_ST_VENDA_ATA = analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }


                    }


                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }



                return RedirectToAction("EditClienteAliqIcmsSTVendaAtaContMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            


        }




    

        /*Edição ICMS ST Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaSNMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS ST Venda no Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;

            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão


            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

            ///*Variavel temporaria para guardar a opção: tempData para que o ciclo de vida seja maior*/
            //TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
            //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata

            ////persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
            //TempData.Keep("opcao");
            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();
            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 
            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao



            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;




                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2": //MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6": //NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;


                    }
                    break;




            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);


            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }
            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";


            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }



        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaSNMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = this.analise_NCM[i].ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";

            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EditClienteAliqIcmsSTVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });
        }


        [HttpGet]
        public ActionResult EditClienteAliqIcmsSTVendaAtaSNMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];
            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);


                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL != analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : (trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = "0.000";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = analiseNCM.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }


                    }





                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }


             return RedirectToAction("EditClienteAliqIcmsSTVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });

            

        }





        /*Edição ICMS Venda Atacado para Simples Nacional*/
        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaSNMassa(
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
            int? page, 
            int? numeroLinhas)
        {
            /*Verificando a sessão*/
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }

            //Mensagem do card
            ViewBag.Mensagem = "Alíquota ICMS  Venda no Atacado para SIMPLES NACIONAL no Cliente X no MTX";

            //variavel auxiliar
            string resultado = param;


            //será usada para carregar a lista pelo cnpj
            this.empresa = (Empresa)Session["empresas"]; //se nao for nula basta carregar a empresa em outra variavel de sessão

            //Mota as view bag de origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();

            //Monta as viewbags do CRT e situação tributaria
            ViewBag.CRT = db.Crts.ToList();
            ViewBag.RegTrib = db.RegimesTribarios.ToList();

            //Pegar o CRT e o Regime tributario e gravar numa temp data
            TempData["crtEmpresa"] = this.empresa.crt.ToString();
            TempData.Keep("crtEmpresa");
            TempData["regimeTribEmpresa"] = this.empresa.regime_trib.ToString();
            TempData.Keep("regimeTribEmpresa");

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            //se o filtro corrente estiver nulo ele busca pelo parametro procurarpor
            string codBarras = (filtroCorrente != null) ? filtroCorrente : procuraPor;

            //converte em long caso seja possivel e atribui à variavel tipada: isso é necessário caso o usuário digitou codigo de barras ao inves de descrição do produto
            long codBarrasL = 0; //variavel tipada
            bool canConvert = long.TryParse(codBarras, out codBarrasL);

            //verifica se veio parametros
            procuraCEST = (procuraCEST != null) ? procuraCEST : null;
            procuraNCM = (procuraNCM != null) ? procuraNCM : null;

            //numero de linhas: Se o parametro numerolinhas vier preenchido ele atribui, caso contrario ele atribui o valor padrao: 10
            //ViewBag.NumeroLinhas = (numeroLinhas != null) ? numeroLinhas : 10;
            VerificarLinhas(numeroLinhas);


            //parametro de ordenacao da tabela
            ViewBag.Ordenacao = ordenacao;

            //Se a ordenação nao estiver nula ele aplica a ordenação produto decresente
            ViewBag.ParametroProduto = (String.IsNullOrEmpty(ordenacao) ? "Produto_desc" : "");

          

            VerificarOpcaoAliq(filtroNulo, opcao);
            opcao = TempData["opcao"].ToString();


            //atribui 1 a pagina caso os parametreos nao sejam nulos
            page = (procuraPor != null) || (procuraCEST != null) || (procuraNCM != null) ? 1 : page;


            //atribui fitro corrente caso alguma procura esteja nulla(seja nullo)
            procuraPor = (procuraPor == null) ? filtroCorrente : procuraPor;
            procuraNCM = (procuraNCM == null) ? filtroCorrenteNCM : procuraNCM;
            procuraCEST = (procuraCEST == null) ? filtroCorrenteCest : procuraCEST;


            /*Ponto de ajuste: fazer com que as buscas persistam entre as requisições usando temp data*/
            //ViewBag.FiltroCorrente = procuraPor;
            ViewBag.FiltroCorrenteCest = procuraCEST;
            ViewBag.FiltroCorrenteNCM = procuraNCM; //nao procura por ncm mas ficara aqui para futuras solicitações
            ViewBag.FiltroCorrente = procuraPor;
            //origem e destino

            //montar select estado origem e destino
            ViewBag.EstadosOrigem = db.Estados.ToList();
            ViewBag.EstadosDestinos = db.Estados.ToList();



            //verifica estados origem e destino
            VerificaOriDest(ufOrigem, ufDestino); //verifica a UF de origem e o destino 


            //aplica estado origem e destino
            ViewBag.UfOrigem = this.ufOrigem;
            ViewBag.UfDestino = this.ufDestino;


            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            /*Switch da opção*/
            switch (opcao)
            {
                case "Maiores":
                case "Alíquotas Maiores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "1";

                    switch (ViewBag.Filtro)
                    {

                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Menores":
                case "Alíquotas Menores":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "2";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Iguais":
                case "Alíquotas Iguais":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "3";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas Cliente":
                case "Alíquotas Nulas no Cliente":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "4";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                    }
                    break;
                case "Nulas MTX":
                case "Alíquotas Nulas no MTX":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "5";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Nulas Ambos":
                case "Alíquotas Nulas em Ambos":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "6";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Isentas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "7";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Não Tributadas":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "8";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Uso Consumo":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "9";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;
                case "Subst. Tributária":
                    //O parametro filtro nulo mostra o filtro que foi informado, caso não informa nenhum ele será de acordo com a opção
                    ViewBag.Filtro = (filtroNulo != null) ? filtroNulo : "10";

                    switch (ViewBag.Filtro)
                    {
                        case "1"://MAIOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "2"://MENOR
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "3": //IGUAIS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "4"://NULA CLIENTE
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "5"://NULA MTX
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "6"://NULAS AMBAS
                            this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "7": //isenta
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "8": //nao tributada
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "9": //uso consumo
                            this.analise_NCM = this.analise_NCM.Where(a => a.PRODUTO_CATEGORIA == 21 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;
                        case "10": //SUBST-TRIBUTARIA
                            this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();
                            break;

                    }
                    break;


            }//fim do switche

            //Action para procurar
            //analise = ProcuraPor(codBarrasL, procuraPor, procuraCEST, procuraNCM, analise);
            this.analise_NCM = ProcuraPorSnPorNCM(codBarrasL, procuraPor, procuraCEST, procuraNCM, this.analise_NCM);

            switch (ordenacao)
            {
                case "Produto_desc":
                    this.analise_NCM = this.analise_NCM.OrderByDescending(s => s.PRODUTO_DESCRICAO).ToList();
                    break;
                default:
                    this.analise_NCM = this.analise_NCM.OrderBy(s => s.PRODUTO_NCM).ToList();
                    break;
            }

            //montar a pagina
            int tamaanhoPagina = 0;

            //ternario para tamanho da pagina
            tamaanhoPagina = (ViewBag.NumeroLinha != null) ? ViewBag.NumeroLinhas : (tamaanhoPagina = (numeroLinhas != 10) ? ViewBag.numeroLinhas : (int)numeroLinhas);

            //Mensagens de retorno
            ViewBag.MensagemGravar = (resultado != null) ? resultado : "";
            ViewBag.RegSalvos = (qtdSalvos != null) ? qtdSalvos : "";
            ViewBag.RegNsalvos = (qtdNSalvos != null) ? qtdNSalvos : "0";

            //mandar a opção para que o javascript veja
            ViewBag.Opcao = opcao;

            int numeroPagina = (page ?? 1);

            return View(this.analise_NCM.ToPagedList(numeroPagina, tamaanhoPagina));//retorna a view tipada
        }

        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaSNMassaTODOS(string opcao)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "../Home");
            }
            this.ufOrigem = TempData["UfOrigem"].ToString();
            this.ufDestino = TempData["UfDestino"].ToString();

            ViewBag.CrtEmpresa = TempData["crtEmpresa"].ToString();
            ViewBag.RegiTribEmpresa = TempData["regimeTribEmpresa"].ToString();

            VerificaTribNMCEmpresa(TempData["crtEmpresa"].ToString(), TempData["regimeTribEmpresa"].ToString()); ; //manda verificar passando a tributacao


            TributacaoEmpresa trib = new TributacaoEmpresa();

            if (opcao == "Alíquotas Maiores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Menores")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL < a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Alíquotas Nulas no Cliente")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null && a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 60 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 40 && a.CST_VENDA_ATA_SIMP_NACIONAL_BASE != 41 && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }

            if (opcao == "Subst.Tributária")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 60 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }
            if (opcao == "Não Tributadas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 41 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }
            if (opcao == "Isentas")
            {
                this.analise_NCM = this.analise_NCM.Where(a => a.CST_VENDA_ATA_SIMP_NACIONAL_BASE == 40 && (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null || a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != null) && a.UF_ORIGEM.Equals(this.ufOrigem) && a.UF_DESTINO.Equals(this.ufDestino)).ToList();

            }


            int regSalv = 0; //reg salvos
            int regNsalv = 0; //reg não salvos
            string resultado = ""; //variavel auxiliar;
            //pega todos os ID para serem alterados
            //this.analiseSn.Count()
            for (int i = 0; i < this.analise_NCM.Count(); i++)
            {

                //converter em inteiro
                int? idTrb = (this.analise_NCM[i].TE_ID);
                trib = db.TributacaoEmpresas.Find(idTrb);//busca o registro
                trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = this.analise_NCM[i].ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");
                trib.DT_ALTERACAO = DateTime.Now;
                try
                {

                    db.SaveChanges();
                    regSalv++; //contagem de registros salvos
                               //toda vez que salvar, gravar uma nova lista e mandar para action



                }
                catch (Exception e)
                {
                    resultado = "Problemas ao salvar o registro: " + e.ToString();
                    regNsalv++;
                }

            }
            resultado = "Registro Salvo com Sucesso!!";


            TempData["analise_trib_Cliente_NCm"] = null;
            TempData.Keep("analise_trib_Cliente_NCm");


            return RedirectToAction("EditClienteAliqIcmsVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, opcao = opcao });

        }




        [HttpGet]
        public ActionResult EditClienteAliqIcmsVendaAtaSNMassaMODAL(string strDados)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("../Home/Login");
            }
            this.empresa = (Empresa)Session["empresas"];

            //Objeto do tipo tributação empresa
            TributacaoEmpresa trib = new TributacaoEmpresa();
            string resultado = ""; //variavel auxiliar;

            //separar a String em um array
            string[] idTrib = strDados.Split(',');

            //retira o elemento vazio do array deixando somente os id dos registros
            idTrib = idTrib.Where(item => item != "").ToArray();


            //registros salvos
            int regSalv = 0;
            int regNsalv = 0;
            string analiseRetorno = null; //atribui NULL AO VALOR INICIAL
            string analiseTrib = null; //atribui  NULL AO VALOR INICIAL

            string ufOrigem = TempData["UfOrigem"].ToString();
            string ufDestino = TempData["UfDestino"].ToString();

            try
            {

                //laço de repetição para percorrer o array com os registros
                for (int i = 0; i < idTrib.Length; i++)
                {
                    //converter em inteiro
                    int idTrb = int.Parse(idTrib[i]);

                    //faz a busca no objeto criado instanciando um so objeto
                    trib = db.TributacaoEmpresas.Find(idTrb);

                    //NA HORA DE COMPARAR DEVE SE PROCURAR PELO ID DO REGISTRO DA EMPRESA, CASO CONTRARIO ELE COMPARA COM O PRIMEIRO REGISTRO DO NCM
                    AnaliseTributariaNCM analiseNCM = (from a in db.Analise_TributariaNCM where a.TE_ID == trib.ID && a.PRODUTO_NCM == trib.PRODUTO_NCM && a.CNPJ_EMPRESA == this.empresa.cnpj && a.UF_ORIGEM == ufOrigem && a.UF_DESTINO == ufDestino && a.CRT_BASE == this.empresa.crt && a.REGIME_TRIB_BASE == this.empresa.regime_trib select a).First();

                    if (trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null)
                    {
                        if (analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE != null)
                        {
                            if (trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL != analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString())
                            {
                                trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                                trib.DT_ALTERACAO = DateTime.Now;
                                db.SaveChanges();
                                regSalv++; //contagem de registros salvos
                            }
                        }
                    }
                    else
                    {
                        analiseRetorno = (analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE == null) ? analiseRetorno : (analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE).ToString();

                        /*o mesmo acontece aqui, se for nulo ele permanece com valor 0.0*/
                        analiseTrib = (analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL == null) ? analiseTrib : (trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL);
                        //analisar se já são iguais
                        if (analiseTrib == analiseRetorno)
                        {
                            regNsalv++;
                        }
                        else
                        {
                            //verificar se a variavel veio 0.0
                            if (analiseRetorno == null)
                            {
                                //se veio 0.0 o valor deve ser atribuido nulo
                                trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = "0.000";
                            }
                            else
                            {
                                trib.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = analiseNCM.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE.ToString().Replace(",", ".");

                            }
                            trib.DT_ALTERACAO = DateTime.Now;
                            db.SaveChanges();
                            regSalv++; //contagem de registros salvos

                        }
                    }
                 

                }
                TempData["analise_trib_Cliente_NCm"] = null;
                TempData.Keep("analise_trib_Cliente_NCm");

                resultado = "Registro Salvo com Sucesso!!";

            }
            catch (Exception e)
            {
                string erro = e.ToString();
                resultado = "Problemas ao salvar o registro: " + erro;

            }




            
                return RedirectToAction("EditClienteAliqIcmsVendaAtaSNMassa", new { param = resultado, qtdSalvos = regSalv, qtdNSalvos = regNsalv });


        }



        /*ACTIONS AUXILIARES*/
        public EmptyResult VerificaTribNMCEmpresa(string crt, string regime)
        {
            if(TempData["analise_trib_Cliente_NCm"] == null)
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
        //VERIFICAR A TEMPDATA DA EMPRESA NO OUTRO BANCO
        public EmptyResult VerificaTempDataEmpresa(string cnpj)
        {
            if(TempData["dadosOutroBanco"] == null)
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
                    TempData["linhas"] = 30;
                    TempData.Keep("linhas");
                    ViewBag.NumeroLinhas = 30;
                }
                else
                {
                    ViewBag.NumeroLinhas = TempData["linhas"];
                }
            }
            return new EmptyResult();
        }

        public EmptyResult VerificarOpcaoAliq(string filtroNulo, string opcao)
        {
            if (filtroNulo != null)
            {
                switch (filtroNulo)
                {
                    case "1":
                        TempData["opcao"] = "Maiores";
                        TempData.Keep("opcao");
                        break;
                    case "2":
                        TempData["opcao"] = "Menores";
                        TempData.Keep("opcao");
                        break;
                    case "3":
                        TempData["opcao"] = "Iguais";
                        TempData.Keep("opcao");
                        break;
                    case "4":
                        TempData["opcao"] = "Nulas Cliente";
                        TempData.Keep("opcao");
                        break;
                    case "5":
                        TempData["opcao"] = "Nulas MTX";
                        TempData.Keep("opcao");
                        break;
                    case "6":
                        TempData["opcao"] = "Nulas Ambos";
                        TempData.Keep("opcao");
                        break;
                    case "7":
                        TempData["opcao"] = "Isentas";
                        TempData.Keep("opcao");
                        break;
                    case "8":
                        TempData["opcao"] = "Não Tributadas";
                        TempData.Keep("opcao");
                        break;
                    case "9":
                        TempData["opcao"] = "Uso Consumo";
                        TempData.Keep("opcao");
                        break;
                    case "10":
                        TempData["opcao"] = "Subst. Tributária";
                        TempData.Keep("opcao");
                        break;
                }
            }
            else
            {

                TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
                                                               //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
                TempData.Keep("opcao");
            }
            return new EmptyResult();
        }

        [HttpGet]
        public List<AnaliseTributariaNCM> ProcuraPorSnPorNCM(long? codBarrasL, string procuraPor, string procuraCEST, string procuraNCM, List<AnaliseTributariaNCM> analiseSnNCM)
        {
            if (procuraCEST == null)
            {
                procuraCEST = "";
            }
            else
            {
                procuraCEST = procuraCEST.Replace(".", ""); //retira os pontos
            }
            if (procuraNCM == null)
            {
                procuraNCM = "";
            }
            else
            {
                procuraNCM = procuraNCM.Replace(".", ""); //retira os pontos
            }


            if (!String.IsNullOrEmpty(procuraPor))
            {
                this.analise_NCM = (codBarrasL != 0) ? (this.analise_NCM.Where(s => s.PRODUTO_COD_BARRAS.ToString().StartsWith(codBarrasL.ToString()))).ToList() : this.analise_NCM = (this.analise_NCM.Where(s => s.PRODUTO_DESCRICAO.ToString().ToUpper().StartsWith(procuraPor.ToUpper()))).ToList();
            }
            if (!String.IsNullOrEmpty(procuraCEST))
            {
                this.analise_NCM = this.analise_NCM.Where(s => s.PRODUTO_CEST == procuraCEST).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }
            if (!String.IsNullOrEmpty(procuraNCM))
            {
                this.analise_NCM = this.analise_NCM.Where(s => s.PRODUTO_NCM == procuraNCM).ToList();
                //analise = analise.Where(s => s.PRODUTO_CEST.ToString().Contains(procuraCEST.ToString())).ToList();
            }

            return this.analise_NCM;

        }

        public EmptyResult VerificarOpcaoRed(string filtroNulo, string opcao)
        {
            if (filtroNulo != null)
            {
                switch (filtroNulo)
                {
                    case "1":
                        TempData["opcao"] = "Maiores";
                        TempData.Keep("opcao");
                        break;
                    case "2":
                        TempData["opcao"] = "Menores";
                        TempData.Keep("opcao");
                        break;
                    case "3":
                        TempData["opcao"] = "Iguais";
                        TempData.Keep("opcao");
                        break;
                    case "4":
                        TempData["opcao"] = "Nulas Cliente";
                        TempData.Keep("opcao");
                        break;
                    case "5":
                        TempData["opcao"] = "Nulas MTX";
                        TempData.Keep("opcao");
                        break;
                    case "6":
                        TempData["opcao"] = "Nulas Ambos";
                        TempData.Keep("opcao");
                        break;
                    case "7":
                        TempData["opcao"] = "Sem Redução";
                        TempData.Keep("opcao");
                        break;

                }
            }
            else
            {

                TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
                                                               //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
                TempData.Keep("opcao");
            }
            return new EmptyResult();
        }

        public EmptyResult VerificarOpcao(string filtroNulo, string opcao)
        {
            if (filtroNulo != null)
            {
                switch (filtroNulo)
                {
                    case "1":
                        TempData["opcao"] = "Iguais";

                        TempData.Keep("opcao");
                        break;
                    case "2":
                        TempData["opcao"] = "Diferentes";

                        TempData.Keep("opcao");
                        break;
                    case "3":
                        TempData["opcao"] = "Nulos Cliente";

                        TempData.Keep("opcao");
                        break;
                    case "4":
                        TempData["opcao"] = "Nulos MTX";

                        TempData.Keep("opcao");
                        break;
                    case "5":
                        TempData["opcao"] = "Nulos Ambos";

                        TempData.Keep("opcao");
                        break;


                }
            }
            else
            {

                TempData["opcao"] = opcao ?? TempData["opcao"];//se a opção for diferente de nula a tempdata recebe o seu valor
                                                               //opcao = (opcao == null) ? TempData["opcao"].ToString() : opcao;//caso venha nula a opcao recebe o valor de tempdata
                                                               //persiste tempdata entre as requisicoes ate que a opcao seja mudada na chamada pelo grafico
                TempData.Keep("opcao");
            }
            return new EmptyResult();
        }
    }
}