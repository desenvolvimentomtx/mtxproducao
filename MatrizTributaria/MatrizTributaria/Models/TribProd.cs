using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
/**
 MODEL PARA SUBSTITUIR O MODEL VIGENTE, DE BUSCA E ANALISE POR NCM
 FOI SOLICITADO QUE A BUSCA E A MANUNTENÇÃO FOSSE FEITO NOVAMENTE POR PRODUTO
 11/07/2023 
 */
namespace MatrizTributaria.Models
{
    [Table("TRIB_PRODUTOS")]
    public class TribProd
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("UF_ORIGEM")]
        public string Uf_Origem { get; set; }

        [Column("UF_DESTINO")]
        public string Uf_Destino { get; set; }

        [Column("ID_CATEGORIA")]
        public Nullable<int> Categoria { get; set; }

        [Column("ID_SETOR")]
        public Nullable<int> Setor { get; set; }

        [Column("FECP")]
        public Nullable<decimal> Fecp { get; set; }

        //PIS COFINS
        [Column("CST_S_PISCOFINS")]
        public Nullable<int> CstSaidaPisCofins { get; set; }

        [Column("ALQ_S_PIS")]
        public Nullable<decimal> AliqSaidaPis { get; set; }
               
        [Column("ALQ_S_COFINS")]
        public Nullable<decimal> AliqSaidaCofins { get; set; }

        //VENDA VAREJO PARA CONSUMIDOR FINAL

        [Column("CST_VD_VRJ_CF")]
        public Nullable<int> CstVendaVarejoCF { get; set; }

        [Column("ALQ_VD_VRJ_CF")]
        public Nullable<decimal> AliqIcmsVendaVarejoCF { get; set; }

        [Column("ALQ_ST_VD_VRJ_CF")]
        public Nullable<decimal> AliqIcmsSTVendaVarejoCF { get; set; }

        [Column("RED_BC_VD_VRJ_CF")]
        public Nullable<decimal> RedBaseCalcIcmsVendaVarejoCF { get; set; }

        [Column("RED_ST_BC_VD_VRJ_CF")]
        public Nullable<decimal> RedBaseCalcIcmsSTVendaVarejoCF { get; set; }

        //VENDA VAREJO PARA CONTRIBUINTE

        [Column("CST_VD_VRJ_CONT")]
        public Nullable<int> CstVendaVarejoCont { get; set; }

        [Column("ALQ_VD_VRJ_CONT")]
        public Nullable<decimal> AliqIcmsVendaVarejoCont { get; set; }

        [Column("ALQ_ST_VD_VRJ_CONT")]
        public Nullable<decimal> AliqIcmsSTVendaVarejoCont { get; set; }

        [Column("RED_BC_VD_VRJ_CONT")]
        public Nullable<decimal> RedBaseCalcIcmsVendaVarejoCont { get; set; }

        [Column("RED_ST_BC_VD_VRJ_CONT")]
        public Nullable<decimal> RedBaseCalcIcmsSTVendaVarejoCont { get; set; }

        //VENDA ATACADO PARA CONTRIBUINTE
        [Column("CST_VD_ATA_CONT")]
        public Nullable<int> CstVendaAtaCont { get; set; }

        [Column("ALQ_VD_ATA_CONT")]
        public Nullable<decimal> AliqIcmsVendaAtaCont { get; set; }

        [Column("ALQ_ST_VD_ATA_CONT")]
        public Nullable<decimal> AliqIcmsSTVendaAtaCont { get; set; }

        [Column("RED_BC_VD_ATA_CONT")]
        public Nullable<decimal> RedBaseCalcIcmsVendaAtaCont { get; set; }

        [Column("RED_ST_BC_VD_ATA_CONT")]
        public Nullable<decimal> RedBaseCalcIcmsSTVendaAtaCont { get; set; }


        //VENDA ATACADO PARA SIMPLES NACIONAL
        [Column("CST_VD_ATA_SN")]
        public Nullable<int> CstVendaAtaSN { get; set; }

        [Column("ALQ_VD_ATA_SN")]
        public Nullable<decimal> AliqIcmsVendaAtaSN { get; set; }

        [Column("ALQ_ST_VD_ATA_SN")]
        public Nullable<decimal> AliqIcmsSTVendaAtaSN { get; set; }

        [Column("RED_BC_VD_ATA_SN")]
        public Nullable<decimal> RedBaseCalcIcmsVendaAtaSN { get; set; }

        [Column("RED_ST_BC_VD_ATA_SN")]
        public Nullable<decimal> RedBaseCalcIcmsSTVendaAtaSN { get; set; }

        //OUTROS DADOS
        [Column("DATA_CAD")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public DateTime? DataCad { get; set; }


        [Column("DATA_ALT")]
        [DisplayFormat(DataFormatString = "{MM/dd/yyyy}")]
        public DateTime? DataAlt { get; set; }

        [Column("AUDITADO")]
        public Nullable<sbyte> Auditado { get; set; }

        [Column("CRT")]
        public Nullable<int> Crt { get; set; }

        [Column("REGIME_TRIB")]
        public Nullable<int> RegimeTrib { get; set; }



    }
}