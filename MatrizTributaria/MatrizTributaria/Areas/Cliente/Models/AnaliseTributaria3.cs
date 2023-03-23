using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
/*Representa no banco a tabela da view que busca os produtos que estão cadastrados na tabela de produtos mas nao tem tributações*/
namespace MatrizTributaria.Areas.Cliente.Models
{
    [Table("analise_tributaria_3")]
    public class AnaliseTributaria3
    {
        [Column("TE_ID")]
        public int? TE_ID { get; set; }

        [Column("CNPJ_EMPRESA")]
        public string CNPJ_EMPRESA { get; set; }

        [Key]
        [Column("PRODUTO_COD_BARRAS")]
        public string PRODUTO_COD_BARRAS { get; set; }

        [Column("PRODUTO_DESCRICAO")]
        public string PRODUTO_DESCRICAO { get; set; }

        [Column("PRODUTO_CEST")]
        public string PRODUTO_CEST { get; set; }

        [Column("PRODUTO_NCM")]
        public string PRODUTO_NCM { get; set; }

        [Column("PRODUTO_CATEGORIA")]
        public int? PRODUTO_CATEGORIA { get; set; }

        [Column("FECP")]
        public double? FECP { get; set; }

        [Column("COD_NAT_RECEITA")]
        public int? COD_NAT_RECEITA { get; set; }

        [Column("CST_ENTRADA_PIS_COFINS")]
        public int? CST_ENTRADA_PIS_COFINS { get; set; }

        [Column("CST_SAIDA_PIS_COFINS")]
        public int? CST_SAIDA_PIS_COFINS { get; set; }

        [Column("ALIQ_ENTRADA_PIS")]
        public double? ALIQ_ENTRADA_PIS { get; set; }

        [Column("ALIQ_SAIDA_PIS")]
        public double? ALIQ_SAIDA_PIS { get; set; }

        [Column("ALIQ_ENTRADA_COFINS")]
        public double? ALIQ_ENTRADA_COFINS { get; set; }

        [Column("ALIQ_SAIDA_COFINS")]
        public double? ALIQ_SAIDA_COFINS { get; set; }

        [Column("CST_VENDA_ATA")]
        public int? CST_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_VENDA_ATA")]
        public double? ALIQ_ICMS_VENDA_ATA { get; set; }

        [Column("ALIQ_ICMS_ST_VENDA_ATA")]
        public double? ALIQ_ICMS_ST_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_VENDA_ATA")]
        public double? RED_BASE_CALC_ICMS_VENDA_ATA { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_VENDA_ATA")]
        public double? RED_BASE_CALC_ICMS_ST_VENDA_ATA { get; set; }

        [Column("CST_VENDA_ATA_SIMP_NACIONAL")]
        public int? CST_VENDA_ATA_SIMP_NACIONAL { get; set; }

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

        [Column("RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_COMPRA_SIMP_NACIONAL { get; set; }

        [Column("RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL")]
        public double? RED_BASE_CALC_ICMS_ST_COMPRA_DE_SIMP_NACIONAL { get; set; }

        [Column("CST_DA_NFE_DA_IND_FORN")]
        public int? CST_DA_NFE_DA_IND_FORN { get; set; }

        [Column("CST_DA_NFE_DE_ATA_FORN")]
        public int? CST_DA_NFE_DE_ATA_FORN { get; set; }

        [Column("CSOSNT_DANFE_DOS_NFOR")]
        public int? CSOSNT_DANFE_DOS_NFOR { get; set; }

        [Column("ALIQ_ICMS_NFE")]
        public double? ALIQ_ICMS_NFE { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_ATA")]
        public double? ALIQ_ICMS_NFE_FOR_ATA { get; set; }

        [Column("ALIQ_ICMS_NFE_FOR_SN")]
        public double? ALIQ_ICMS_NFE_FOR_SN { get; set; }


        [Column("TIPO_MVA")]
        public string TIPO_MVA { get; set; }

        [Column("VALOR_MVA_IND")]
        public double? VALOR_MVA_IND { get; set; }

        [Column("INICIO_VIGENCIA_MVA")]
        public string INICIO_VIGENCIA_MVA { get; set; }

        [Column("FIM_VIGENCIA_MVA")]
        public string FIM_VIGENCIA_MVA { get; set; }

        [Column("CREDITO_OUTORGADO")]
        public int? CREDITO_OUTORGADO { get; set; }

        [Column("VALOR_MVA_ATACADO")]
        public double? VALOR_MVA_ATACADO { get; set; }

        [Column("REGIME_2560")]
        public int? REGIME_2560 { get; set; }

        [Column("UF_ORIGEM")]
        public string UF_ORIGEM { get; set; }

        [Column("ATIVO")]
        public sbyte ATIVO { get; set; }

        [Column("UF_DESTINO")]
        public string UF_DESTINO { get; set; }
        [Column("DT_ALTERACAO")]

        public DateTime? DT_ALTERACAO { get; set; }
        [Column("Descricao_INTERNO")]
        public string Descricao_INTERNO { get; set; }

        [Column("Cest_INTERNO")]
        public string Cest_INTERNO { get; set; }



        [Column("Cod_Barras_INTERNO")]
        public string Cod_Barras_INTERNO { get; set; }

        [Column("Id_Produto_INTERNO")]
        public double? Id_Produto_INTERNO { get; set; }

        [Column("NCM_INTERNO")]
        public string NCM_INTERNO { get; set; }


        public string DataFormatada
        {
            get { return DT_ALTERACAO?.ToShortDateString(); }
        }

    }
}