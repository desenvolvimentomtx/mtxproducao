using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrizTributaria.Controllers
{
    public class UtilController : Controller
    {
        // GET: Util
        //aplica a tributação na tempdada
        public  EmptyResult VerificaTributacao(string crt, string regime)
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