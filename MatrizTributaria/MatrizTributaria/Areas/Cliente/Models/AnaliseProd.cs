using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Areas.Cliente.Models
{
    [Table("produto_comparar")]
    public class AnaliseProd
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_EMPRESA")]
        public int? ID_EMPRESA { get; set; }

        [Column("CNPJ_EMPRESA")]
        public string CNPJ_EMPRESA { get; set; }

        [Column("PROD_DESCRICAO_CLIENTE")]
        public string PRODUTO_DESCRICAO_CLIENTE { get; set; }


        [Column("NCM_CLIENTE")]
        public string NCM_CLIENTE { get; set; }


        [Column("COD_BARRAS_CLIENTE")]
        public string COD_BARRAS_CLIENTE { get; set; }


        [Column("CEST_CLIENTE")]
        public string CEST_CLIENTE { get; set; }

        [Column("COD_BARRAS_BASE")]
        public string COD_BARRAS_BASE { get; set; }

        [Column("NCM_BASE")]
        public string NCM_BASE { get; set; }

        [Column("CEST_BASE")]
        public string CEST_BASE { get; set; }

        [Column("DESCRICAO_BASE")]
        public string DESCRICAO_BASE { get; set; }



       
    }
}