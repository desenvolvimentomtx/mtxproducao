using System.Data.Entity;

namespace MatrizTributaria.Models
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class MatrizDbContext : DbContext
    {

        public MatrizDbContext() : base("name=MatrizDbContext") { }

        public virtual DbSet<CategoriaProduto> CategoriaProdutos { get; set; }
        public virtual DbSet<ClassificacaoNatReceita> ClassificacaoNatReceitas { get; set; }
        public virtual DbSet<Produto> Produtos { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Nivel> Niveis { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Estado> Estados { get; set; }

        public virtual DbSet<SoftwareHouse> SoftwareHouses { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Versao> Versoes { get; set; }

        public virtual DbSet<Legislacao> Legislacoes { get; set; }
        public virtual DbSet<NaturezaReceita> NaturezaReceitas { get; set; }
        public virtual DbSet<Tributacao> Tributacoes { get; set; }
        public virtual DbSet<TributacaoNCM> TributacoesNcm { get; set; }//31032022

        public virtual DbSet<TributacaoNCMView> TributacoesNcmView { get; set; }//31032022

        public virtual DbSet<TributacaoSN> TributacoesSn { get; set; }//31032022

        public virtual DbSet<CstIcmsGeral> CstIcmsGerais { get; set; }
        public virtual DbSet<CstPisCofinsEntrada> CstPisCofinsEntradas { get; set; }
        public virtual DbSet<CstPisCofinsSaida> CstPisCofinsSaidas { get; set; }

        public virtual DbSet<SetorProdutos> SetorProdutos { get; set; }

        public virtual DbSet<TributacaoEmpresa> TributacaoEmpresas { get; set; }

   
        public virtual DbSet<TributacaoGeralView> Tributacao_GeralView { get; set; }
        public virtual DbSet<TtributacaoGeralViewSN> Tributacao_GeralView_Sn { get; set; }

        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributaria> Analise_Tributaria { get; set; } //Vitor
        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributariaSn> Analise_TributariaSn { get; set; } //Paulo

        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributariaNCM> Analise_TributariaNCM { get; set; } //Paulo
        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributaria2> Analise_Tributaria_2 { get; set; } //Paulo

        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseTributaria3> Analise_Tributaria_3 { get; set; } //Paulo

        public virtual DbSet<MatrizTributaria.Areas.Cliente.Models.AnaliseProd> Analise_Prod { get; set; } //Paulo


    }
}