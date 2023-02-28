using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Areas.Cliente.Models
{
    [Table("analise_tributaria_ncm")]
    public class AnaliseTributariaNCM
    {
        [Column("TE_ID")]
        public int? TE_ID { get; set; }

        [Column("CNPJ_EMPRESA")]
        public string CNPJ_EMPRESA { get; set; }

        [Key]
        [Column("PRODUTO_COD_BARRAS_CLIENTE")]
        public string PRODUTO_COD_BARRAS { get; set; }

        [Column("PRODUTO_DESCRICAO_CLIENTE")]
        public string PRODUTO_DESCRICAO { get; set; }

        [Column("PRODUTO_CEST_CLIENTE")]
        public string PRODUTO_CEST { get; set; }

        [Column("PRODUTO_NCM_CLIENTE")]
        public string PRODUTO_NCM { get; set; }

        [Column("PRODUTO_CATEGORIA_CLIENTE")]
        public int? PRODUTO_CATEGORIA { get; set; }

        [Column("FECP_CLIENTE")]
        public double? FECP { get; set; }

        [Column("COD_NAT_RECEITA_CLIENTE")]
        public int? COD_NAT_RECEITA { get; set; }

        [Column("CST_ENTRADA_PIS_COFINS_CLIENTE")]
        public int? CST_ENTRADA_PIS_COFINS { get; set; }

        [Column("CST_SAIDA_PIS_COFINS_CLIENTE")]
        public int? CST_SAIDA_PIS_COFINS { get; set; }

        [Column("ALIQ_ENTRADA_PIS_CLIENTE")]
        public double? ALIQ_ENTRADA_PIS { get; set; }

        [Column("ALIQ_SAIDA_PIS_CLIENTE")]
        public double? ALIQ_SAIDA_PIS { get; set; }

        [Column("ALIQ_ENTRADA_COFINS_CLIENTE")]
        public double? ALIQ_ENTRADA_COFINS { get; set; }

        [Column("ALIQ_SAIDA_COFINS_CLIENTE")]
        public double? ALIQ_SAIDA_COFINS { get; set; }

        [Column("CST_VENDA_ATA_CLIENTE")]
        public int? CST_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_CLIENTE")]
        public double? ALIQ_ICMS_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_CLIENTE")]
        public double? ALIQ_ICMS_ST_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA { get; set; }

        [Column("CST_VENDA_ATA_SIMP_NACIONAL_CLIENTE")]
        public int? CST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_CLIENTE")]
        public double? ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_CLIENTE")]
        public double? ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("CST_VENDA_VAREJO_CONT_CLIENTE")]
        public int? CST_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONT_CLIENTE")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONT_CLIENTE")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_VENDA_VAREJO_CONT_CLIENTE")]
        public double? RED_BASE_CALC_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_ST_VENDA_VAREJO_CONT_CLIENTE")]
        public double? RED_BASE_CALC_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("CST_VENDA_VAREJO_CONS_FINAL_CLIENTE")]
        public int? CST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_CLIENTE")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_CLIENTE")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("CST_COMPRA_DE_IND_CLIENTE")]
        public int? CST_COMPRA_DE_IND { get; set; }

        [Column("ALIQ_ICMS_COMP_DE_IND_CLIENTE")]
        public double? ALIQ_ICMS_COMP_DE_IND { get; set; }

        [Column("ALIQ_ICMS_ST_COMP_DE_IND_CLIENTE")]
        public double? ALIQ_ICMS_ST_COMP_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_IND_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND { get; set; }

        [Column("CST_COMPRA_DE_ATA_CLIENTE")]
        public int? CST_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_ATA_CLIENTE")]
        public double? ALIQ_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_ATA_CLIENTE")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_ATA_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("CST_COMPRA_DE_SIMP_NACIONAL_CLIENTE")]
        public int? CST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL_CLIENTE")]
        public double? ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_CLIENTE")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_CLIENTE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("CST_DA_NFE_DA_IND_FORN_CLIENTE")]
        public int? CST_DA_NFE_DA_IND_FORN { get; set; }

        [Column("CST_DA_NFE_DE_ATA_FORN_CLIENTE")]
        public int? CST_DA_NFE_DE_ATA_FORN { get; set; }

        [Column("CSOSNT_DANFE_DOS_NFOR_CLIENTE")]
        public int? CSOSNT_DANFE_DOS_NFOR { get; set; }

        [Column("ALIQ_ICMS_NFE_CLIENTE")]
        public double? ALIQ_ICMS_NFE { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_ATA_CLIENTE")]
        public double? ALIQ_ICMS_NFE_FOR_ATA { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_SN_CLIENTE")]
        public double? ALIQ_ICMS_NFE_FOR_SN { get; set; }

        [Column("TIPO_MVA_CLIENTE")]
        public string TIPO_MVA { get; set; }

        [Column("VALOR_MVA_IND_CLIENTE")]
        public double? VALOR_MVA_IND { get; set; }

        [Column("INICIO_VIGENCIA_MVA_CLIENTE")]
        public string INICIO_VIGENCIA_MVA { get; set; }

        [Column("FIM_VIGENCIA_MVA_CLIENTE")]
        public string FIM_VIGENCIA_MVA { get; set; }

        [Column("CREDITO_OUTORGADO_CLIENTE")]
        public int? CREDITO_OUTORGADO { get; set; }

        [Column("VALOR_MVA_ATACADO_CLIENTE")]
        public double? VALOR_MVA_ATACADO { get; set; }

        [Column("REGIME_2560_CLIENTE")]
        public int? REGIME_2560 { get; set; }

        [Column("UF_ORIGEM_CLIENTE")]
        public string UF_ORIGEM { get; set; }

        [Column("UF_DESTINO_CLIENTE")]
        public string UF_DESTINO { get; set; }

        [Column("PRODUTO_COD_INTERNO_CLIENTE")]
        public int? PRODUTO_COD_INTERNO { get; set; }

        [Column("ATIVO_CLIENTE")]
        public sbyte ATIVO { get; set; }


        /*BASE*/


        [Column("UF_ORIGEM_BASE")]
        public string UF_ORIGEM_BASE { get; set; }


        [Column("UF_DESTINO_BASE")]
        public string UF_DESTINO_BASE { get; set; }

        [Column("NCM_BASE")]
        public string NCM_BASE { get; set; }

        [Column("CEST_BASE")]
        public string CEST_BASE { get; set; }

        [Column("CATEGORIA_BASE")]
        public int? CATEGORIA_BASE { get; set; }

        [Column("FECP_BASE")]
        public double? FECP_BASE { get; set; }

        [Column("COD_NAT_RECEITA_BASE")]
        public int? COD_NAT_RECEITA_BASE { get; set; }

        [Column("CST_ENTRADA_PISCOFINS_BASE")]
        public int? CST_ENTRADA_PISCOFINS_BASE { get; set; }



        [Column("CST_SAIDA_PISCOFINS_BASE")]
        public int? CST_SAIDA_PISCOFINS_BASE { get; set; }


        [Column("ALIQ_ENT_PIS_BASE")]
        public double? ALIQ_ENT_PIS_BASE { get; set; }

        [Column("ALIQ_SAIDA_PIS_BASE")]
        public double? ALIQ_SAIDA_PIS_BASE { get; set; }


        [Column("ALIQ_ENT_COFINS_BASE")]
        public double? ALIQ_ENT_COFINS_BASE { get; set; }

        [Column("ALIQ_SAIDA_COFINS_BASE")]
        public double? ALIQ_SAIDA_COFINS_BASE { get; set; }


        [Column("CST_VENDA_ATA_CONT_BASE")]
        public int? CST_VENDA_ATA_CONT_BASE { get; set; }

      
        [Column("ALIQ_ICMS_VENDA_ATA_CONT_BASE")]
        public double? ALIQ_ICMS_VENDA_ATA_CONT_BASE { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE")]
        public double? ALIQ_ICMS_ST_VENDA_ATA_CONT_BASE { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA_CONT_BASE { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT_BASE { get; set; }


        [Column("CST_VENDA_ATA_SIMP_NACIONAL_BASE")]
        public int? CST_VENDA_ATA_SIMP_NACIONAL_BASE { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE")]
        public double? ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE")]
        public double? ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL_BASE { get; set; }


        [Column("CST_VENDA_VAREJO_CONT_BASE")]
        public int? CST_VENDA_VAREJO_CONT_BASE { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONT_BASE")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONT_BASE { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONT_BASE { get; set; }

        [Column("RED_BASE_CALC_VENDA_VAREJO_CONT_BASE")]
        public double? RED_BASE_CALC_VENDA_VAREJO_CONT_BASE { get; set; }

        [Column("RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE")]
        public double? RED_BASE_CALC_ST_VENDA_VAREJO_CONT_BASE { get; set; }

        [Column("CST_VENDA_VAREJO_CONS_FINAL_BASE")]
        public int? CST_VENDA_VAREJO_CONS_FINAL_BASE { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL_BASE { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE")]
        public double? RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL_BASE { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL_BASE { get; set; }


        [Column("ID_FUND_LEGAL_SAIDA_ICMS_BASE")]
        public double? ID_FUND_LEGAL_SAIDA_ICMS_BASE { get; set; }


        [Column("ID_FUND_LELGAL_ENTRADA_ICMS_BASE")]
        public double? ID_FUND_LELGAL_ENTRADA_ICMS_BASE { get; set; }


        [Column("CST_COMPRA_DE_IND_BASE")]
        public int? CST_COMPRA_DE_IND_BASE { get; set; }


        [Column("ALIQ_ICMS_COMP_DE_IND_BASE")]
        public double? ALIQ_ICMS_COMP_DE_IND_BASE { get; set; }


        [Column("ALIQ_ICMS_ST_COMP_DE_IND_BASE")]
        public double? ALIQ_ICMS_ST_COMP_DE_IND_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_IND_BASE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_IND_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND_BASE { get; set; }


        [Column("CST_COMPRA_DE_ATA_BASE")]
        public int? CST_COMPRA_DE_ATA_BASE { get; set; }


        [Column("ALIQ_ICMS_COMPRA_DE_ATA_BASE")]
        public double? ALIQ_ICMS_COMPRA_DE_ATA_BASE { get; set; }


        [Column("ALIQ_ICMS_ST_COMPRA_DE_ATA_BASE")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_ATA_BASE { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_ATA_BASE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_ATA_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA_BASE { get; set; }


        [Column("CST_COMPRA_DE_SIMP_NACIONAL_BASE")]
        public int? CST_COMPRA_DE_SIMP_NACIONAL_BASE { get; set; }


        [Column("ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL_BASE")]
        public double? ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL_BASE { get; set; }


        [Column("ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_BASE")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL_BASE")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL_BASE { get; set; }


        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_BASE")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL_BASE { get; set; }


        [Column("CST_DA_NFE_DA_IND_FORN_BASE")]
        public int? CST_DA_NFE_DA_IND_FORN_BASE { get; set; }


        [Column("CST_DA_NFE_DE_ATA_FORN_BASE")]
        public int? CST_DA_NFE_DE_ATA_FORN_BASE { get; set; }


        [Column("CSOSNTDANFEDOSNFOR_BASE")]
        public int? CSOSNTDANFEDOSNFOR_BASE { get; set; }


        [Column("ALIQ_ICMS_NFE_BASE")]
        public double? ALIQ_ICMS_NFE_BASE { get; set; }


        [Column("ALIQ_ICMS_NFE_FOR_SN_BASE")]
        public double? ALIQ_ICMS_NFE_FOR_SN_BASE { get; set; }


        [Column("ALIQ_ICMS_NFE_FOR_ATA_BASE")]
        public double? ALIQ_ICMS_NFE_FOR_ATA_BASE { get; set; }

        
        [Column("SIMP_NACIONAL_BASE")]
        public int? SIMP_NACIONAL_BASE { get; set; }


        [Column("CATEGORIA_DESCRICAO")]
        public string CATEGORIA_DESCRICAO { get; set; }


    }
}