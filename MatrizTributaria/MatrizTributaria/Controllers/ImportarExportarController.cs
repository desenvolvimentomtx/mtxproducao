using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MatrizTributaria.Models;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Remoting.Contexts;

namespace MatrizTributaria.Controllers
{
    public class ImportarExportarController : Controller
    {

        readonly MatrizDbContext db;

        public ImportarExportarController()
        {
            db = new MatrizDbContext();
        }


        // GET: ImportarExportar
        public ActionResult Index()
        {
            //verifica se a sessão do usuário está ativa
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login","Home");
            }

            ViewBag.Display = "none";
            ViewBag.Display2 = "none";
            return View();
        }

        //importar json
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase jsonFile)
        {
            StreamReader streamReader;
            //verificar se o arquivo json é nullo
            if (jsonFile != null)
            {
                //caso o arquivo json nao esteja nullo
                try
                {

                    //verificando tabela
                    var tribEmpresas = from s in db.TributacaoEmpresas select s; //select na tabela
                    var qtd = tribEmpresas.Count(); //quantadidade de registreos
                    if (qtd <= 0) //se a quantidade for menor ou igual a zero, quer dizer que a tabela está limpa, dai entra no laço
                    {
                        //if para verificar o nome do arquivo, se ele nao tiver na extensão correta
                        if (!Path.GetFileName(jsonFile.FileName).EndsWith(".json"))
                        {
                            ViewBag.Error = "Arquivo Inválido. Favor importar um arquivo no formato JSON"; //caso nao seja um json
                            ViewBag.Display = "true";
                            ViewBag.Display2 = "none";
                        }
                        else
                        {

                            //salva na pasta do servidor
                            jsonFile.SaveAs(Server.MapPath("~/Jsons/" + Path.GetFileName(jsonFile.FileName)));

                            //passando para streamReader
                            streamReader = new StreamReader(Server.MapPath("~/Jsons/" + Path.GetFileName(jsonFile.FileName)));

                            //passando para string
                            //passando para string
                            string data = streamReader.ReadToEnd(); //leitura ate o fim

                            //objeto do tipo lista da entidade que queremos salvar no banco
                            List<TributacaoEmpresa> tribEmpresa = JsonConvert.DeserializeObject<List<TributacaoEmpresa>>(data);

                            //criando o objeto para receber os dados e salvar no banco (laço para percorrer toda a lista)
                            tribEmpresa.ForEach(p =>
                            {
                                TributacaoEmpresa tribEmp = new TributacaoEmpresa()
                                {
                                    CNPJ_EMPRESA = p.CNPJ_EMPRESA,

                                    PRODUTO_COD_BARRAS = p.PRODUTO_COD_BARRAS,
                                    PRODUTO_DESCRICAO = p.PRODUTO_DESCRICAO,
                                    PRODUTO_CEST = p.PRODUTO_CEST,
                                    PRODUTO_NCM = p.PRODUTO_NCM,
                                    PRODUTO_CATEGORIA = p.PRODUTO_CATEGORIA,
                                    FECP = p.FECP,
                                    COD_NAT_RECEITA = p.COD_NAT_RECEITA,

                                    CST_ENTRADA_PIS_COFINS = p.CST_ENTRADA_PIS_COFINS,
                                    CST_SAIDA_PIS_COFINS = p.CST_SAIDA_PIS_COFINS,
                                    ALIQ_ENTRADA_PIS = p.ALIQ_ENTRADA_PIS,
                                    ALIQ_SAIDA_PIS = p.ALIQ_SAIDA_PIS,
                                    ALIQ_ENTRADA_COFINS = p.ALIQ_ENTRADA_COFINS,
                                    ALIQ_SAIDA_COFINS = p.ALIQ_SAIDA_COFINS,

                                    CST_VENDA_ATA = p.CST_VENDA_ATA,
                                    ALIQ_ICMS_VENDA_ATA = p.ALIQ_ICMS_VENDA_ATA,
                                    ALIQ_ICMS_ST_VENDA_ATA = p.ALIQ_ICMS_ST_VENDA_ATA,
                                    RED_BASE_CALC_ICMS_VENDA_ATA = p.RED_BASE_CALC_ICMS_VENDA_ATA,
                                    RED_BASE_CALC_ICMS_ST_VENDA_ATA = p.RED_BASE_CALC_ICMS_ST_VENDA_ATA,

                                    CST_VENDA_ATA_SIMP_NACIONAL = p.CST_VENDA_ATA_SIMP_NACIONAL,
                                    ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = p.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL,
                                    ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = p.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL,
                                    RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = p.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL,
                                    RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = p.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL,

                                    CST_VENDA_VAREJO_CONT = p.CST_VENDA_VAREJO_CONT,
                                    ALIQ_ICMS_VENDA_VAREJO_CONT = p.ALIQ_ICMS_VENDA_VAREJO_CONT,
                                    ALIQ_ICMS_ST_VENDA_VAREJO_CONT = p.ALIQ_ICMS_ST_VENDA_VAREJO_CONT,
                                    RED_BASE_CALC_VENDA_VAREJO_CONT = p.RED_BASE_CALC_VENDA_VAREJO_CONT,
                                    RED_BASE_CALC_ST_VENDA_VAREJO_CONT = p.RED_BASE_CALC_ST_VENDA_VAREJO_CONT,

                                    CST_VENDA_VAREJO_CONS_FINAL = p.CST_VENDA_VAREJO_CONS_FINAL,
                                    ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = p.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL,
                                    ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = p.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL,
                                    RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = p.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL,
                                    RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = p.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL,

                                    CST_COMPRA_DE_IND = p.CST_COMPRA_DE_IND,
                                    ALIQ_ICMS_COMP_DE_IND = p.ALIQ_ICMS_COMP_DE_IND,
                                    ALIQ_ICMS_ST_COMP_DE_IND = p.ALIQ_ICMS_ST_COMP_DE_IND,
                                    RED_BASE_CALC_ICMS_COMPRA_DE_IND = p.RED_BASE_CALC_ICMS_COMPRA_DE_IND,
                                    RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = p.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND,

                                    CST_COMPRA_DE_ATA = p.CST_COMPRA_DE_ATA,
                                    ALIQ_ICMS_COMPRA_DE_ATA = p.ALIQ_ICMS_COMPRA_DE_ATA,
                                    ALIQ_ICMS_ST_COMPRA_DE_ATA = p.ALIQ_ICMS_ST_COMPRA_DE_ATA,
                                    RED_BASE_CALC_ICMS_COMPRA_DE_ATA = p.RED_BASE_CALC_ICMS_COMPRA_DE_ATA,
                                    RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = p.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA,

                                    CST_COMPRA_DE_SIMP_NACIONAL = p.CST_COMPRA_DE_SIMP_NACIONAL,
                                    ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = p.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL,
                                    ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = p.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL,
                                    RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = p.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL,
                                    RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = p.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL,

                                    CST_DA_NFE_DA_IND_FORN = p.CST_DA_NFE_DA_IND_FORN,
                                    CST_DA_NFE_DE_ATA_FORN = p.CST_DA_NFE_DE_ATA_FORN,
                                    CSOSNT_DANFE_DOS_NFOR = p.CSOSNT_DANFE_DOS_NFOR,
                                    ALIQ_ICMS_NFE = p.ALIQ_ICMS_NFE,
                                    TIPO_MVA = p.TIPO_MVA,
                                    VALOR_MVA_IND = p.VALOR_MVA_IND,
                                    INICIO_VIGENCIA_MVA = p.INICIO_VIGENCIA_MVA,
                                    FIM_VIGENCIA_MVA = p.FIM_VIGENCIA_MVA,
                                    CREDITO_OUTORGADO = p.CREDITO_OUTORGADO,
                                    VALOR_MVA_ATACADO = p.VALOR_MVA_ATACADO,
                                    REGIME_2560 = p.REGIME_2560

                                };
                                db.TributacaoEmpresas.Add(tribEmp); //add o objeto ja populado
                                db.SaveChanges(); //salva no banco
                            });
                            ViewBag.Sucesso = "Registro salvo com sucesso";
                            ViewBag.Display2 = "true";
                            ViewBag.Display = "none";
                            streamReader.Close();
                            System.IO.File.Delete(Server.MapPath("~/Jsons/" + (jsonFile.FileName)));


                        } //fim do else
                    }
                    else
                    { //fim do if que verifica a qdt de registros
                        ViewBag.Error = "Tabela ja contem registros. Favor cliar em TABELA para eliminar os dados contidos";
                        ViewBag.Display = "true";
                        ViewBag.Display2 = "none";
                                               
                        return View("Index");
                    }

                }
                catch (Exception e)
                {

                    ViewBag.Error = "Arquivo Vazio. Favor selecionar um arquivo .json : " + e.ToString();
                    ViewBag.Display = "true";
                  
                    System.IO.File.Delete(Server.MapPath("~/Jsons/" + (jsonFile.FileName)));
                    return View("Index");
                }
            }
            else
            {
                ViewBag.Error = "Favor selecionar um arquivo  do tipo Json no botão acima";
                ViewBag.Display = "true";
                ViewBag.Display2 = "none";
                return View("Index");
            }



            return View("Index");
        } //fim da acttion

      
        public ActionResult Dados()
        {
            //verificando tabela
            var tribEmpresas = from s in db.TributacaoEmpresas select s; //select na tabela
            var qtd = tribEmpresas.Count(); //quantadidade de registreos
            if(qtd <= 0)
            {
                ViewBag.Error = "Tabela sem registros";
                ViewBag.Display = "true";
                ViewBag.Display2 = "none";
            }
            else
            {
                //limpa a tabela para importação de um novo arquivo json
                /*Ainda será necessário validar o cnpj para que ele apague somente o cnpj especifico*/
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE tributacao_empresa");
                ViewBag.Sucesso = "Registros excluidos com sucesso";
                ViewBag.Display2 = "true";
                ViewBag.Display = "none";
            }

            return View("Index");
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