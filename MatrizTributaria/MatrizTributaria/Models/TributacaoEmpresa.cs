using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MatrizTributaria.Models
{
    [Table("tributacao_empresa")]
    public class TributacaoEmpresa
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CNPJ_EMPRESA")]
        public string CNPJ_EMPRESA { get; set; }

        [Column("PRODUTO_COD_BARRAS")]
        public string PRODUTO_COD_BARRAS { get; set; }

        [Column("PRODUTO_DESCRICAO")]
        public string PRODUTO_DESCRICAO { get; set; }

        [Column("PRODUTO_CEST")]
        public string PRODUTO_CEST { get; set; }

        [Column("PRODUTO_NCM")]
        public string PRODUTO_NCM { get; set; }

        [Column("PRODUTO_CATEGORIA")]
        public string PRODUTO_CATEGORIA { get; set; }

        [Column("FECP")]
        public string FECP { get; set; }

        [Column("COD_NAT_RECEITA")]
        public string COD_NAT_RECEITA { get; set; }

        [Column("CST_ENTRADA_PIS_COFINS")]
        public string CST_ENTRADA_PIS_COFINS { get; set; }

        [Column("CST_SAIDA_PIS_COFINS")]
        public string CST_SAIDA_PIS_COFINS { get; set; }

        [Column("ALIQ_ENTRADA_PIS")]
        public string ALIQ_ENTRADA_PIS { get; set; }

        [Column("ALIQ_SAIDA_PIS")]
        public string ALIQ_SAIDA_PIS { get; set; }

        [Column("ALIQ_ENTRADA_COFINS")]
        public string ALIQ_ENTRADA_COFINS { get; set; }

        [Column("ALIQ_SAIDA_COFINS")]
        public string ALIQ_SAIDA_COFINS { get; set; }

        [Column("CST_VENDA_ATA")]
        public string CST_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA")]
        public string ALIQ_ICMS_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA")]
        public string ALIQ_ICMS_ST_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA")]
        public string RED_BASE_CALC_ICMS_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA")]
        public string RED_BASE_CALC_ICMS_ST_VENDA_ATA { get; set; }

        [Column("CST_VENDA_ATA_SIMP_NACIONAL")]
        public string CST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL")]
        public string ALIQ_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL")]
        public string ALIQ_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL")]
        public string RED_BASE_CALC_ICMS_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL")]
        public string RED_BASE_CALC_ICMS_ST_VENDA_ATA_SIMP_NACIONAL { get; set; }

        [Column("CST_VENDA_VAREJO_CONT")]
        public string CST_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONT")]
        public string ALIQ_ICMS_VENDA_VAREJO_CONT { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONT")]
        public string ALIQ_ICMS_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_VENDA_VAREJO_CONT")]
        public string RED_BASE_CALC_VENDA_VAREJO_CONT { get; set; }

        [Column("RED_BASE_CALC_ST_VENDA_VAREJO_CONT")]
        public string RED_BASE_CALC_ST_VENDA_VAREJO_CONT { get; set; }

        [Column("CST_VENDA_VAREJO_CONS_FINAL")]
        public string CST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL")]
        public string ALIQ_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL")]
        public string ALIQ_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL")]
        public string RED_BASE_CALC_ICMS_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL")]
        public string RED_BASE_CALC_ICMS_ST_VENDA_VAREJO_CONS_FINAL { get; set; }

        [Column("CST_COMPRA_DE_IND")]
        public string CST_COMPRA_DE_IND { get; set; }

        [Column("ALIQ_ICMS_COMP_DE_IND")]
        public string ALIQ_ICMS_COMP_DE_IND { get; set; }

        [Column("ALIQ_ICMS_ST_COMP_DE_IND")]
        public string ALIQ_ICMS_ST_COMP_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_IND")]
        public string RED_BASE_CALC_ICMS_COMPRA_DE_IND { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND")]
        public string RED_BASE_CALC_ICMS_ST_COMPRA_DE_IND { get; set; }

        [Column("CST_COMPRA_DE_ATA")]
        public string CST_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_ATA")]
        public string ALIQ_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_ATA")]
        public string ALIQ_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_DE_ATA")]
        public string RED_BASE_CALC_ICMS_COMPRA_DE_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA")]
        public string RED_BASE_CALC_ICMS_ST_COMPRA_DE_ATA { get; set; }

        [Column("CST_COMPRA_DE_SIMP_NACIONAL")]
        public string CST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL")]
        public string ALIQ_ICMS_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL")]
        public string ALIQ_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL")]
        public string RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL")]
        public string RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("CST_DA_NFE_DA_IND_FORN")]
        public string CST_DA_NFE_DA_IND_FORN { get; set; }

        [Column("CST_DA_NFE_DE_ATA_FORN")]
        public string CST_DA_NFE_DE_ATA_FORN { get; set; }

        [Column("CSOSNT_DANFE_DOS_NFOR")]
        public string CSOSNT_DANFE_DOS_NFOR { get; set; }

        [Column("ALIQ_ICMS_NFE")]
        public string ALIQ_ICMS_NFE { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_SN")]
        public string ALIQ_ICMS_NFE_FOR_SN { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_ATA")]
        public string ALIQ_ICMS_NFE_FOR_ATA { get; set; }


        [Column("TIPO_MVA")]
        public string TIPO_MVA { get; set; }

        [Column("VALOR_MVA_IND")]
        public string VALOR_MVA_IND { get; set; }

        [Column("INICIO_VIGENCIA_MVA")]
        public string INICIO_VIGENCIA_MVA { get; set; }

        [Column("FIM_VIGENCIA_MVA")]
        public string FIM_VIGENCIA_MVA { get; set; }

        [Column("CREDITO_OUTORGADO")]
        public string CREDITO_OUTORGADO { get; set; }

        [Column("VALOR_MVA_ATACADO")]
        public string VALOR_MVA_ATACADO { get; set; }

        [Column("REGIME_2560")]
        public string REGIME_2560 { get; set; }

        [Column("DT_ALTERACAO")]
        public DateTime? DT_ALTERACAO { get; set; }


        //[Column("ESTADO")]
        //public string ESTADO { get; set; }
        [Column("UF_ORIGEM")]
        public string UF_ORIGEM { get; set; }

        [Column("UF_DESTINO")]
        public string UF_DESTINO { get; set; }

        [Column("ATIVO")]
        public sbyte ATIVO { get; set; }






    }
}