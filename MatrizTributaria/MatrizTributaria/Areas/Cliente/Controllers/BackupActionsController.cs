using MatrizTributaria.Areas.Cliente.Models;
using MatrizTributaria.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Areas.Cliente.Controllers
{
    /*Actions antigas que estavam sem uso na controller TributaçãoEmpresaController.cs*/
    public class BackupActionsController : Controller
    {
        readonly private MatrizDbContext db = new MatrizDbContext();
        // GET: Cliente/BackupActions
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EditAll()
        {
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();

            List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            analise.ForEach(a =>
            {
                TributacaoEmpresa TE = db.TributacaoEmpresas.Find(a.TE_ID);
                if (a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO)
                {
                    TE.ALIQ_ENTRADA_COFINS = a.Aliq_Ent_Cofins_INTERNO.ToString();
                }

                if (a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO)
                {
                    TE.ALIQ_ENTRADA_PIS = a.Aliq_Ent_Pis_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_ATA = a.Aliq_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMP_DE_IND = a.Aliq_Icms_Comp_de_Ind_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO)
                {
                    TE.ALIQ_ICMS_NFE = a.Aliq_Icms_NFE_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_ATA = a.Aliq_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Aliq_Icms_ST_Comp_de_Ind_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_ATA = a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA = a.Aliq_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA = a.Aliq_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_VAREJO_CONT = a.Aliq_Icms_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO)
                {
                    TE.ALIQ_SAIDA_COFINS = a.Aliq_Saida_Cofins_INTERNO.ToString();
                }
                if (a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO)
                {
                    TE.ALIQ_SAIDA_PIS = a.Aliq_Saida_Pis_INTERNO.ToString();
                }
                if (a.CSOSNT_DANFE_DOS_NFOR != a.CsosntdaNfedoSnFOR_INTERNO)
                {
                    TE.CSOSNT_DANFE_DOS_NFOR = a.CsosntdaNfedoSnFOR_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_ATA != a.Cst_Compra_de_Ata_INTERNO)
                {
                    TE.CST_COMPRA_DE_ATA = a.Cst_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_IND != a.Cst_Compra_de_Ind_INTERNO)
                {
                    TE.CST_COMPRA_DE_IND = a.Cst_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_SIMP_NACIONAL != a.Cst_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.CST_COMPRA_DE_SIMP_NACIONAL = a.Cst_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.CST_DA_NFE_DA_IND_FORN != a.Cst_da_Nfe_da_Ind_FORN_INTERNO)
                {
                    TE.CST_DA_NFE_DA_IND_FORN = a.Cst_da_Nfe_da_Ind_FORN_INTERNO.ToString();
                }
                if (a.CST_DA_NFE_DE_ATA_FORN != a.Cst_da_Nfe_de_Ata_FORn_INTERNO)
                {
                    TE.CST_DA_NFE_DE_ATA_FORN = a.Cst_da_Nfe_de_Ata_FORn_INTERNO.ToString();
                }
                if (a.CST_VENDA_ATA_SIMP_NACIONAL != a.Cst_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.CST_VENDA_ATA_SIMP_NACIONAL = a.Cst_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.CST_VENDA_VAREJO_CONS_FINAL != a.Cst_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.CST_VENDA_VAREJO_CONS_FINAL = a.Cst_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.CST_VENDA_VAREJO_CONT != a.Cst_Venda_Varejo_Cont_INTERNO)
                {
                    TE.CST_VENDA_VAREJO_CONT = a.Cst_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_DE_IND = a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_ATA = a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_VENDA_ATA = a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_VENDA_VAREJO_CONT = a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO.ToString();
                }
                TE.DT_ALTERACAO = DateTime.Now;
            });
            db.SaveChanges();
            return RedirectToAction("AnaliseTributaria");
        }

        public ActionResult EditAliquota()
        {
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();

            List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            analise.ForEach(a =>
            {
                TributacaoEmpresa TE = db.TributacaoEmpresas.Find(a.TE_ID);
                if (a.ALIQ_ENTRADA_COFINS > a.Aliq_Ent_Cofins_INTERNO)
                {
                    TE.ALIQ_ENTRADA_COFINS = a.Aliq_Ent_Cofins_INTERNO.ToString();
                }

                if (a.ALIQ_ENTRADA_PIS > a.Aliq_Ent_Pis_INTERNO)
                {
                    TE.ALIQ_ENTRADA_PIS = a.Aliq_Ent_Pis_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMPRA_DE_ATA > a.Aliq_Icms_Compra_de_Ata_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_ATA = a.Aliq_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Aliq_Icms_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_COMP_DE_IND > a.Aliq_Icms_Comp_de_Ind_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMP_DE_IND = a.Aliq_Icms_Comp_de_Ind_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_NFE > a.Aliq_Icms_NFE_INTERNO)
                {
                    TE.ALIQ_ICMS_NFE = a.Aliq_Icms_NFE_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_COMPRA_DE_ATA > a.Aliq_Icms_ST_Compra_de_Ata_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_ATA = a.Aliq_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Aliq_Icms_ST_Comp_de_Ind_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Aliq_Icms_ST_Comp_de_Ind_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_ATA > a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_ATA = a.Aliq_Icms_ST_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = a.Aliq_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL = a.Aliq_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_ST_VENDA_VAREJO_CONT > a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = a.Aliq_Icms_ST_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA = a.Aliq_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA > a.Aliq_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA = a.Aliq_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = a.Aliq_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL > a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = a.Aliq_Icms_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.ALIQ_ICMS_VENDA_VAREJO_CONT > a.Aliq_Icms_Venda_Varejo_Cont_INTERNO)
                {
                    TE.ALIQ_ICMS_VENDA_VAREJO_CONT = a.Aliq_Icms_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.ALIQ_SAIDA_COFINS > a.Aliq_Saida_Cofins_INTERNO)
                {
                    TE.ALIQ_SAIDA_COFINS = a.Aliq_Saida_Cofins_INTERNO.ToString();
                }
                if (a.ALIQ_SAIDA_PIS > a.Aliq_Saida_Pis_INTERNO)
                {
                    TE.ALIQ_SAIDA_PIS = a.Aliq_Saida_Pis_INTERNO.ToString();
                }
                TE.DT_ALTERACAO = DateTime.Now;
            });
            db.SaveChanges();
            return RedirectToAction("AnaliseTributaria");
        }

        public ActionResult EditCST()
        {
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();

            List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            analise.ForEach(a =>
            {
                TributacaoEmpresa TE = db.TributacaoEmpresas.Find(a.TE_ID);
                if (a.CSOSNT_DANFE_DOS_NFOR != a.CsosntdaNfedoSnFOR_INTERNO)
                {
                    TE.CSOSNT_DANFE_DOS_NFOR = a.CsosntdaNfedoSnFOR_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_ATA != a.Cst_Compra_de_Ata_INTERNO)
                {
                    TE.CST_COMPRA_DE_ATA = a.Cst_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_IND != a.Cst_Compra_de_Ind_INTERNO)
                {
                    TE.CST_COMPRA_DE_IND = a.Cst_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.CST_COMPRA_DE_SIMP_NACIONAL != a.Cst_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.CST_COMPRA_DE_SIMP_NACIONAL = a.Cst_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.CST_DA_NFE_DA_IND_FORN != a.Cst_da_Nfe_da_Ind_FORN_INTERNO)
                {
                    TE.CST_DA_NFE_DA_IND_FORN = a.Cst_da_Nfe_da_Ind_FORN_INTERNO.ToString();
                }
                if (a.CST_DA_NFE_DE_ATA_FORN != a.Cst_da_Nfe_de_Ata_FORn_INTERNO)
                {
                    TE.CST_DA_NFE_DE_ATA_FORN = a.Cst_da_Nfe_de_Ata_FORn_INTERNO.ToString();
                }
                if (a.CST_VENDA_ATA_SIMP_NACIONAL != a.Cst_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.CST_VENDA_ATA_SIMP_NACIONAL = a.Cst_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.CST_VENDA_VAREJO_CONS_FINAL != a.Cst_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.CST_VENDA_VAREJO_CONS_FINAL = a.Cst_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.CST_VENDA_VAREJO_CONT != a.Cst_Venda_Varejo_Cont_INTERNO)
                {
                    TE.CST_VENDA_VAREJO_CONT = a.Cst_Venda_Varejo_Cont_INTERNO.ToString();
                }
                TE.DT_ALTERACAO = DateTime.Now;

            });
            db.SaveChanges();
            return RedirectToAction("AnaliseTributaria");
        }
        public ActionResult EditBaseCalc()
        {
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();
            Empresa empresa = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();

            List<AnaliseTributaria> analise = (from a in db.Analise_Tributaria where a.CNPJ_EMPRESA == empresa.cnpj select a).ToList();
            analise.ForEach(a =>
            {
                TributacaoEmpresa TE = db.TributacaoEmpresas.Find(a.TE_ID);
                if (a.RED_BASE_CALC_ICMS_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = a.Red_Base_Calc_Icms_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_COMPRA_DE_IND > a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_DE_IND = a.Red_Base_Calc_Icms_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA > a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = a.Red_Base_Calc_Icms_ST_Compra_de_Ata_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND > a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = a.Red_Base_Calc_Icms_ST_Compra_de_Ind_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = a.Red_Base_Calc_Icms_ST_Compra_de_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_ATA > a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_ATA = a.Red_Base_Calc_Icms_ST_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_ST_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL > a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = a.Red_Base_Calc_Icms_ST_Venda_Varejo_Cons_Final_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_VENDA_ATA > a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_VENDA_ATA = a.Red_Base_Calc_Icms_Venda_Ata_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL > a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO)
                {
                    TE.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = a.Red_Base_Calc_Icms_Venda_Ata_Simp_Nacional_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_ST_VENDA_VAREJO_CONT > a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_ST_VENDA_VAREJO_CONT = a.Red_Base_Calc_ST_Venda_Varejo_Cont_INTERNO.ToString();
                }
                if (a.RED_BASE_CALC_VENDA_VAREJO_CONT > a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO)
                {
                    TE.RED_BASE_CALC_VENDA_VAREJO_CONT = a.Red_Base_Calc_Venda_Varejo_Cont_INTERNO.ToString();
                }
                TE.DT_ALTERACAO = DateTime.Now;
            });
            db.SaveChanges();
            return RedirectToAction("AnaliseTributaria");
        }

        // GET: TributacaoEmpresa
        [HttpGet]
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page, string LinhasNum)
        {
            if (Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            //Pegando empresa do Usuario
            string user = Session["usuario"].ToString();
            Usuario usuario = (from a in db.Usuarios where a.nome == user select a).FirstOrDefault();
            Empresa emp = (from a in db.Empresas where a.cnpj == usuario.empresa.cnpj select a).FirstOrDefault();

            ViewBag.CSTPCE = db.CstPisCofinsEntradas.ToList();
            ViewBag.CSTPCS = db.CstPisCofinsSaidas.ToList();
            ViewBag.CSTGERAIS = db.CstIcmsGerais.ToList();

            //Iniciando lista de Tributos
            List<TributacaoEmpresa> Tributacao = new List<TributacaoEmpresa>();



            //Paginação 
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProdutoParam = String.IsNullOrEmpty(sortOrder) ? "Produto_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var trib = from s in db.TributacaoEmpresas select s;
            if (!String.IsNullOrEmpty(searchString))
            {

                trib = trib.Where(s => s.PRODUTO_DESCRICAO.ToString().ToUpper().Contains(searchString.ToUpper()));


            }
            Tributacao = (from a in db.TributacaoEmpresas orderby a.PRODUTO_DESCRICAO select a).ToList();
            int pageSize = 0;

            if (String.IsNullOrEmpty(LinhasNum))
            {
                pageSize = 10;
            }
            else
            {

                ViewBag.Texto = LinhasNum;
                pageSize = Int32.Parse(LinhasNum);
            }


            int pageNumber = (page ?? 1);

            //var produtos = db.Produtos.ToList();
            return View(Tributacao.ToPagedList(pageNumber, pageSize)); //retorna a view com o numero de paginas e tamanho

        }
        [HttpPost]
        public ActionResult editManual()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(TributacaoEmpresa tributacaoEmpresa, string convertID, string cstEntradaPisCofins, double aliqPisE, double alicCofinsE, string idCstSaiPC,
           double aliqPisS, double aliqConfinsS, string idCstVeAtaCont, double aliqICMSV, double aliqICMSST, double rBcVaC, double rBcSTVaC, string idCstVeAtaSN, double alVSN,
           double alVSNSt, double rBcVSN, double rBcSTVSN, string idCstVeVarCont, double alVeVarCont, double alVeVarContSt, double rBcVeVarCont, double rBcSTVeVarCont, string idCstVeVarCF,
           double alVeVarCF, double alVeVarCFSt, double rBcVeVarCF, double rBcSTVeVarCF, string idCstCompInd, double aliqCompInd, double aliqSTCompInd, double rBcalCompInd, double rBcalSTCompInd,
           string idCstCompAta, double aliqNfeInd, string idCstCompSN, double aliqCompAta, double aliqSTCompAta, double rBcalCompAta, double rBcalSTCompAta, string idCstNfeSN, double aliqNfeAta,
           string idCstNfeAta, double aliqCompSN, double aliqSTCompSN, double rBcalCompSN, double rBcalSTCompSN, string idCstNfeInd, double aliqNfeSN, string idTipoMva, double valorMvaInd,
           double valorMvaAta, string iniVig, string fimVig, double legisPC, double legisEIcms, double legisSIcms)

        {
            try
            {
                string[] split = convertID.Split('|');
                List<TributacaoEmpresa> tributacao = new List<TributacaoEmpresa>();

                //Adicionar Produtos a Lista
                for (int i = 1; i < split.Length; i++)
                {
                    int id = int.Parse(split[i]);
                    tributacao.Add((from a in db.TributacaoEmpresas where a.ID == id select a).FirstOrDefault());
                }

                //Conersões CSTs
                int cstentradapis = (from a in db.CstPisCofinsEntradas where a.descricao == cstEntradaPisCofins select a.codigo).FirstOrDefault();
                int cstsaidapis = (from a in db.CstPisCofinsSaidas where a.descricao == idCstSaiPC select a.codigo).FirstOrDefault();
                int cstgerais = (from a in db.CstIcmsGerais where a.descricao == idCstVeAtaCont select a.codigo).FirstOrDefault();
                int cstgeraisA = (from a in db.CstIcmsGerais where a.descricao == idCstVeVarCont select a.codigo).FirstOrDefault();
                int cstgeraisB = (from a in db.CstIcmsGerais where a.descricao == idCstVeVarCF select a.codigo).FirstOrDefault();
                int cstgeraisC = (from a in db.CstIcmsGerais where a.descricao == idCstCompInd select a.codigo).FirstOrDefault();
                int cstgeraisD = (from a in db.CstIcmsGerais where a.descricao == idCstNfeInd select a.codigo).FirstOrDefault();
                int cstgeraisE = (from a in db.CstIcmsGerais where a.descricao == idCstCompAta select a.codigo).FirstOrDefault();
                int cstgeraisF = (from a in db.CstIcmsGerais where a.descricao == idCstNfeAta select a.codigo).FirstOrDefault();

                //EDIT List 
                tributacao.ForEach(x =>
                {
                    x.CST_ENTRADA_PIS_COFINS = cstentradapis.ToString();

                    x.CST_SAIDA_PIS_COFINS = cstsaidapis.ToString();

                    x.ALIQ_ENTRADA_PIS = aliqPisE.ToString();

                    x.ALIQ_ENTRADA_COFINS = alicCofinsE.ToString();

                    x.ALIQ_SAIDA_PIS = aliqPisS.ToString();

                    x.ALIQ_SAIDA_COFINS = aliqConfinsS.ToString();

                    x.CST_VENDA_ATA = cstgerais.ToString();

                    x.ALIQ_ICMS_VENDA_ATA = aliqICMSV.ToString();

                    x.RED_BASE_CALC_ICMS_VENDA_ATA = rBcVaC.ToString();

                    x.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = rBcSTVaC.ToString();

                    x.ALIQ_ICMS_ST_VENDA_ATA = alVSN.ToString();

                    x.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = alVSN.ToString();

                    x.ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL = alVSN.ToString();

                    x.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = alVSN.ToString();

                    x.ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL = alVSNSt.ToString();

                    x.RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL = rBcSTVSN.ToString();

                    x.CST_VENDA_VAREJO_CONT = cstgeraisA.ToString();

                    x.ALIQ_ICMS_VENDA_VAREJO_CONT = alVeVarCont.ToString();

                    x.ALIQ_ICMS_ST_VENDA_VAREJO_CONT = alVeVarContSt.ToString();

                    x.RED_BASE_CALC_ICMS_ST_VENDA_ATA = rBcVeVarCont.ToString();

                    x.CST_VENDA_VAREJO_CONS_FINAL = cstgeraisB.ToString();

                    x.ALIQ_ICMS_VENDA_VAREJO_CONT = alVeVarCF.ToString();

                    x.ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL = alVeVarCFSt.ToString();

                    x.RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL = rBcVeVarCF.ToString();

                    x.RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL = rBcSTVeVarCF.ToString();

                    x.CST_COMPRA_DE_IND = cstgeraisC.ToString();

                    x.ALIQ_ICMS_COMP_DE_IND = aliqCompInd.ToString();

                    x.ALIQ_ICMS_ST_COMP_DE_IND = aliqSTCompInd.ToString();

                    x.RED_BASE_CALC_ICMS_COMPRA_DE_IND = rBcalCompInd.ToString();

                    x.RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND = rBcalSTCompInd.ToString();

                    x.CST_DA_NFE_DA_IND_FORN = cstgeraisD.ToString();

                    x.ALIQ_ICMS_NFE = aliqNfeInd.ToString();

                    x.CST_COMPRA_DE_ATA = cstgeraisE.ToString();

                    x.ALIQ_ICMS_COMPRA_DE_ATA = aliqCompAta.ToString();

                    x.ALIQ_ICMS_ST_COMPRA_DE_ATA = aliqSTCompAta.ToString();

                    x.RED_BASE_CALC_ICMS_COMPRA_DE_ATA = rBcalCompAta.ToString();

                    x.RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA = rBcalSTCompAta.ToString();

                    x.CST_DA_NFE_DE_ATA_FORN = cstgeraisF.ToString();

                    x.ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL = aliqCompSN.ToString();

                    x.ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = aliqSTCompSN.ToString();

                    x.RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL = rBcalSTCompSN.ToString();

                    x.RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL = rBcalCompSN.ToString();

                    x.TIPO_MVA = idTipoMva;

                    x.VALOR_MVA_ATACADO = valorMvaAta.ToString();

                    x.INICIO_VIGENCIA_MVA = iniVig;

                    x.FIM_VIGENCIA_MVA = fimVig;

                });

                return View("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return View("Index");
            }

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