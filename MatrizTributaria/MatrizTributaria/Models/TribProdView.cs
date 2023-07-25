using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models
{
    [Table("TRIB_PROD_VIEW")]
    public class TribProdView
    {
        [Column("ID")]
        public int? ID { get; set; }

        [Column("UF_ORIGEM")]
        public string UF_ORIGEM { get; set; }

        [Column("UF_DESTINO")]
        public string UF_DESTINO { get; set; }

        [Column("ID_PRODUTO")]
        public int? ID_PRODUTO { get; set; }

        [Column("ID_SETOR")]
        public int? ID_SETOR { get; set; }

        [Column("DESCRICAO_SETOR")]
        public string DESCRICAO_SETOR { get; set; }

        [Column("FECP")]
        public double? FECP { get; set; }

        [Column("CST_S_PISCOFINS")]
        public int? CST_S_PISCOFINS { get; set; }


        [Column("ALQ_S_PIS")]
        public double? ALQ_S_PIS { get; set; }

       
        [Column("ALQ_S_COFINS")]
        public double? ALQ_S_COFINS { get; set; }

        //VENDA VAREJO CONSUMIDOR FINAL

        [Column("CST_VD_VRJ_CF")]
        public int? CST_VD_VRJ_CF { get; set; }

        [Column("ALQ_VD_VRJ_CF")]
        public double? ALQ_VD_VRJ_CF { get; set; }

        [Column("ALQ_ST_VD_VRJ_CF")]
        public double? ALQ_ST_VD_VRJ_CF { get; set; }

        [Column("RED_BC_VD_VRJ_CF")]
        public double? RED_BC_VD_VRJ_CF { get; set; }

        [Column("RED_ST_BC_VD_VRJ_CF")]
        public double? RED_ST_BC_VD_VRJ_CF { get; set; }

        //VENDA VAREJO PARA CONTRIBUINTE

        [Column("CST_VD_VRJ_CONT")]
        public int? CST_VD_VRJ_CONT { get; set; }

        [Column("ALQ_VD_VRJ_CONT")]
        public double? ALQ_VD_VRJ_CONT { get; set; }

        [Column("ALQ_ST_VD_VRJ_CONT")]
        public double? ALQ_ST_VD_VRJ_CONT { get; set; }

        [Column("RED_BC_VD_VRJ_CONT")]
        public double? RED_BC_VD_VRJ_CONT { get; set; }

        [Column("RED_ST_BC_VD_VRJ_CONT")]
        public double? RED_ST_BC_VD_VRJ_CONT { get; set; }

        //VENDA NO ATACADO PARA CONTRIBUINTE

        [Column("CST_VD_ATA_CONT")]
        public int? CST_VD_ATA_CONT { get; set; }

        [Column("ALQ_VD_ATA_CONT")]
        public double? ALQ_VD_ATA_CONT { get; set; }

        [Column("ALQ_ST_VD_ATA_CONT")]
        public double? ALQ_ST_VD_ATA_CONT { get; set; }

        [Column("RED_BC_VD_ATA_CONT")]
        public double? RED_BC_VD_ATA_CONT { get; set; }

        [Column("RED_ST_BC_VD_ATA_CONT")]
        public double? RED_ST_BC_VD_ATA_CONT { get; set; }


        //VENDA NO ATACADO PARA SIMPLES NACIONAL 

        [Column("CST_VD_ATA_SN")]
        public int? CST_VD_ATA_SN { get; set; }

        [Column("ALQ_VD_ATA_SN")]
        public double? ALQ_VD_ATA_SN { get; set; }

        [Column("ALQ_ST_VD_ATA_SN")]
        public double? ALQ_ST_VD_ATA_SN { get; set; }

        [Column("RED_BC_VD_ATA_SN")]
        public double? RED_BC_VD_ATA_SN { get; set; }

        [Column("RED_ST_BC_VD_ATA_SN")]
        public double? RED_ST_BC_VD_ATA_SN { get; set; }


        [Column("DATA_CAD")]
        public DateTime? DATA_CAD { get; set; }
        public string DataCadFormatada
        {
            get { return DATA_CAD?.ToShortDateString(); }
        }

        [Column("DATA_ALT")]
        public DateTime? DATA_ALT { get; set; }
        public string DataAltFormatada
        {
            get { return DATA_ALT?.ToShortDateString(); }
        }



        [Column("AUDITADO")]
        public sbyte? AUDITADO { get; set; }

        [Column("CRT")]
        public int? CRT { get; set; }

        [Column("REGIME_TRIB")]
        public int? REGIME_TRIB { get; set; }

        [Column("ID_DO_PRODUTO")]
        public int? ID_DO_PRODUTO { get; set; }

        [Column("CODIGO_BARRAS")]
        public Int64? CODIGO_BARRAS { get; set; }

        [Column("DESCRICAO_PRODUTO")]
        public string DESCRICAO_PRODUTO { get; set; }

        [Column("NCM_PRODUTO")]
        public string NCM_PRODUTO { get; set; }

        [Column("CEST_PRODUTO")]
        public string CEST_PRODUTO { get; set; }


        [Column("COD_BARRAS_GERADO_PRODUTO")]
        public Int64? COD_BARRAS_GERADO_PRODUTO { get; set; }

        [Column("ID_CATEGORIA_PRODUTO")]
        public int ID_CATEGORIA_PRODUTO { get; set; }

        [Column("DESCRICAO_CATEGORIA")]
        public string DESCRICAO_CATEGORIA { get; set; }

        [Column("DESCRICAO_REGIME")]
        public string DESCRICAO_REGIME { get; set; }

        [Column("DESCRICAO_DO_CRT")]
        public string DESCRICAO_DO_CRT { get; set; }
               

    }
}