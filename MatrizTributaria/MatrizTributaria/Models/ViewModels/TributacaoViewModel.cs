using System;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class TributacaoViewModel
    {

        public int id { get; set; }


        //[Required(ErrorMessage = "O campo Estado é obrigatório", AllowEmptyStrings = false)]
        //public string estado { get; set; }

        [Required(ErrorMessage = "O campo UF Origem é obrigatório", AllowEmptyStrings = false)]
        public string UF_Origem { get; set; }

        [Required(ErrorMessage = "O campo UF Destino é obrigatório", AllowEmptyStrings = false)]
        public string UF_Destino { get; set; }


        [Required(ErrorMessage = "O campo Id do Produto é obrigatório", AllowEmptyStrings = false)]
        public int idProduto { get; set; }

        [Required(ErrorMessage = "O campo Id do Setor é obrigatório", AllowEmptyStrings = false)]
        public int idSetor { get; set; }


        public Nullable<decimal> fecp { get; set; }


        public Nullable<int> codNatReceita { get; set; }


        public Nullable<int> cstEntradaPisCofins { get; set; }


        public Nullable<int> cstSaidaPisCofins { get; set; }


        public Nullable<decimal> aliqEntPis { get; set; }


        public Nullable<decimal> aliqSaidaPis { get; set; }


        public Nullable<decimal> aliqEntCofins { get; set; }


        public Nullable<decimal> aliqSaidaCofins { get; set; }



        public Nullable<int> idFundamentoLegal { get; set; }



        public Nullable<int> cstVendaAtaCont { get; set; }


        public Nullable<decimal> aliqIcmsVendaAtaCont { get; set; }


        public Nullable<decimal> aliqIcmsSTVendaAtaCont { get; set; }


        public Nullable<decimal> redBaseCalcIcmsVendaAtaCont { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTVendaAtaCont { get; set; }



        public Nullable<int> cstVendaAtaSimpNacional { get; set; }


        public Nullable<decimal> aliqIcmsVendaAtaSimpNacional { get; set; }


        public Nullable<decimal> aliqIcmsSTVendaAtaSimpNacional { get; set; }


        public Nullable<decimal> redBaseCalcIcmsVendaAtaSimpNacional { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTVendaAtaSimpNacional { get; set; }



        public Nullable<int> cstVendaVarejoCont { get; set; }


        public Nullable<decimal> aliqIcmsVendaVarejoCont { get; set; }


        public Nullable<decimal> aliqIcmsSTVendaVarejo_Cont { get; set; }


        public Nullable<decimal> redBaseCalcVendaVarejoCont { get; set; }


        public Nullable<decimal> RedBaseCalcSTVendaVarejo_Cont { get; set; }



        public Nullable<int> cstVendaVarejoConsFinal { get; set; }


        public Nullable<decimal> aliqIcmsVendaVarejoConsFinal { get; set; }


        public Nullable<decimal> aliqIcmsSTVendaVarejoConsFinal { get; set; }


        public Nullable<decimal> redBaseCalcIcmsVendaVarejoConsFinal { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTVendaVarejoConsFinal { get; set; }



        public Nullable<int> idFundLegalSaidaICMS { get; set; }



        public Nullable<int> idFundLelgalEntradaICMS { get; set; }



        public Nullable<int> cstCompraDeInd { get; set; }


        public Nullable<decimal> aliqIcmsCompDeInd { get; set; }


        public Nullable<decimal> aliqIcmsSTCompDeInd { get; set; }


        public Nullable<decimal> redBaseCalcIcmsCompraDeInd { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTCompraDeInd { get; set; }



        public Nullable<int> cstCompradeAta { get; set; }


        public Nullable<decimal> aliqIcmsCompradeAta { get; set; }


        public Nullable<decimal> aliqIcmsSTCompraDeAta { get; set; }


        public Nullable<decimal> redBaseCalcIcmsCompraDeAta { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTCompraDeAta { get; set; }



        public Nullable<int> cstCompradeSimpNacional { get; set; }


        public Nullable<decimal> aliqIcmsCompradeSimpNacional { get; set; }


        public Nullable<decimal> aliqIcmsSTCompradeSimpNacional { get; set; }


        public Nullable<decimal> redBaseCalcIcmsCompradeSimpNacional { get; set; }


        public Nullable<decimal> redBaseCalcIcmsSTCompradeSimpNacional { get; set; }



        public Nullable<int> cstdaNfedaIndFORN { get; set; }



        public Nullable<int> cstdaNfedeAtaFORn { get; set; }



        public Nullable<int> CsosntdaNfedoSnFOR { get; set; }


        public Nullable<decimal> aliqIcmsNFE { get; set; }

        
        public Nullable<decimal> aliqIcmsNfeSN { get; set; }

        public Nullable<decimal> aliqIcmsNfeAta { get; set; }


        public string tipoMVA { get; set; }


        public Nullable<decimal> valorMVAInd { get; set; }


        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> inicioVigenciaMVA { get; set; }


        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> fimVigenciaMVA { get; set; }


        public Nullable<sbyte> creditoOutorgado { get; set; }


        public Nullable<decimal> valorMVAAtacado { get; set; }


        public Nullable<sbyte> regime2560 { get; set; }


        public DateTime? dataCad { get; set; }


        public DateTime? dataAlt { get; set; }

        public enum TipoMVA
        {
            PAUTA,
            IVA

        }



    }
}