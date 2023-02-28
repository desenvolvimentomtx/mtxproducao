using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("tributacao_geral")]
    public class TributacaoGeralView
    {
        [Column("ID")]
        public int? ID { get; set; }

        //[Column("UF")]
        //public string UF { get; set; }

        //[Column("ESTADO")]
        //public string ESTADO { get; set; }


        [Column("UF_ORIGEM")]
        public string UF_ORIGEM { get; set; }

        [Column("UF_DESTINO")]
        public string UF_DESTINO { get; set; }
                

        [Column("ID_PRODUTO")]
        public int? ID_PRODUTO { get; set; }

        [Column("ID_SETOR")]
        public int? ID_SETOR { get; set; }

        
        [Column("ID_CATEGORIA")]
        public int? ID_CATEGORIA { get; set; }

        [Column("FECP")]
        public double? FECP { get; set; }

        [Column("COD_NAT_RECEITA")]
        public int? COD_NAT_RECEITA { get; set; }

        [Column("CST_ENTRADA_PISCOFINS")]
        public int? CST_ENTRADA_PISCOFINS { get; set; }

        [Column("CST_SAIDA_PISCOFINS")]
        public int? CST_SAIDA_PISCOFINS { get; set; }

        [Column("ALIQ_ENT_PIS")]
        public double? ALIQ_ENT_PIS { get; set; }

        [Column("ALIQ_SAIDA_PIS")]
        public double? ALIQ_SAIDA_PIS { get; set; }

        [Column("ALIQ_ENT_COFINS")]
        public double? ALIQ_ENT_COFINS { get; set; }

        [Column("ALIQ_SAIDA_COFINS")]
        public double? ALIQ_SAIDA_COFINS { get; set; }

        [Column("ID_FUNDAMENTO_LEGAL")]
        public int? ID_FUNDAMENTO_LEGAL { get; set; }

        [Column("CST_VENDA_ATA_CONT")]
        public int? CST_VENDA_ATA_CONT { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_CONT")]
        public double? ALIQ_ICMS_VENDA_ATA_CONT { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_CONT")]
        public double? ALIQ_ICMS_ST_VENDA_ATA_CONT { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_CONT")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA_CONT { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA_CONT { get; set; }


        [Column("CST_VENDA_ATA_SIMP_NACIONAL")]
        public int? CST_VENDA_ATA_SIMP_NACIONAL{ get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL")]
        public double? ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL")]
        public double? ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }


        [Column("CST_VENDA_VAREJO_CONT")]
        public int? CST_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONT")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONT")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_VENDA_VAREJO_CONT")]
        public double? RED_BASE_CALC_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_ST_VENDA_VAREJO_CONT")]
        public double? RED_BASE_CALC_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("CST_VENDA_VAREJO_CONS_FINAL")]
        public int? CST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL")]
        public double? ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL")]
        public double? ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL")]
        public double? RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ID_FUND_LEGAL_SAIDA_ICMS")]
        public int? ID_FUND_LEGAL_SAIDA_ICMS { get; set; }

        [Column("ID_FUND_LELGAL_ENTRADA_ICMS")]
        public int? ID_FUND_LELGAL_ENTRADA_ICMS { get; set; }

        [Column("CST_COMPRA_DE_IND")]
        public int? CST_COMPRA_DE_IND { get; set; }

        [Column("ALIQ_ICMS_COMP_DE_IND")]
        public double? ALIQ_ICMS_COMP_DE_IND { get; set; }

        [Column("ALIQ_ICMS_ST_COMP_DE_IND")]
        public double? ALIQ_ICMS_ST_COMP_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_IND")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND { get; set; }

        [Column("CST_COMPRA_DE_ATA")]
        public int? CST_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_ATA")]
        public double? ALIQ_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_ATA")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_ATA")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("CST_COMPRA_DE_SIMP_NACIONAL")]
        public int? CST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL")]
        public double? ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL")]
        public double? ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("CST_DA_NFE_DA_IND_FORN")]
        public int? CST_DA_NFE_DA_IND_FORN { get; set; }

        [Column("CST_DA_NFE_DE_ATA_FORN")]
        public int? CST_DA_NFE_DE_ATA_FORN { get; set; }

        [Column("CSOSNTDANFEDOSNFOR")]
        public int? CSOSNTDANFEDOSNFOR { get; set; }

        [Column("ALIQ_ICMS_NFE")]
        public double? ALIQ_ICMS_NFE { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_ATA")]
        public double? ALIQ_ICMS_NFE_FOR_ATA { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_SN")]
        public double? ALIQ_ICMS_NFE_FOR_SN { get; set; }

        [Column("TIPO_MVA")]
        public string TIPO_MVA { get; set; }

        [Column("VALORMVAIND")]
        public double? VALORMVAIND { get; set; }

       
        [Column("INICIO_VIGENCIA_MVA")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> INICIO_VIGENCIA_MVA { get; set; }


        [Column("FIM_VIGENCIA_MVA")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public Nullable<System.DateTime> FIM_VIGENCIA_MVA { get; set; }


        [Column("CREDITO_OUTORGADO")]
        public int? CREDITO_OUTORGADO { get; set; }

        [Column("AUDITADO_POR_NCM")]
        public int? AUDITADO_POR_NCM { get; set; }

        [Column("VALOR_MVA_ATACADO")]
        public double? VALOR_MVA_ATACADO { get; set; }

        [Column("REGIME_2560")]
        public int? REGIME_2560 { get; set; }


        [Column("DESCRICAO_PRODUTO")]
        public string DESCRICAO_PRODUTO { get; set; }

        [Column("DESCRICAO_SETOR_PRODUTO")]
        public string DESCRICAO_SETOR_PRODUTO { get; set; }

        
        [Column("CATEGORIA_DESCRICAO")]
        public string CATEGORIA_DESCRICAO { get; set; }

        [Column("CEST_PRODUTO")]
        public string CEST_PRODUTO { get; set; }

        [Column("NCM_PRODUTO")]
        public string NCM_PRODUTO { get; set; }


        [Column("COD_BARRAS_PRODUTO")]
        public string COD_BARRAS_PRODUTO { get; set; }

        [Column("DATAALT")]
        public DateTime? DATAALT { get; set; }


        public string DataFormatada
        {
            get { return DATAALT?.ToShortDateString(); }
        }
        //[Column("DATAALT")]
        //[DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        //public Nullable<System.DateTime> DATAALT { get; set; }

    }
}