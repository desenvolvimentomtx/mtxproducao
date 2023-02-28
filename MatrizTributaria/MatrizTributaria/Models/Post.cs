using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatrizTributaria.Models
{
    public class Post
    {
        public string st_nome_sac { get; set; }
        public string st_nomeref_sac { get; set; }
        public string st_cgc_sac { get; set; }
        public string st_email_sac { get; set; }
        public string st_telefone_sac { get; set; }
        public string st_endereco_sac { get; set; }
        public string st_complemento_sac { get; set; }
        public string st_cidade_sac { get; set; }
        public string st_estado_sac { get; set; }
        public string st_cep_sac { get; set; }
        
        public string st_diavencimento_sac { get; set; }
        
        public string dt_cadastro_sac { get; set; }
        public string dt_alteracao_sincro { get; set; }
       
        public string id_sacado_sac { get; set; } //id do cliente
       
        public string st_senha_sac { get; set; }
       
        public string dt_desativacao_sac { get; set; }
        

        public string st_bairro_sac { get; set; }


        public string st_fax_sac { get; set; }


        public string st_cepentrega_sac { get; set; }


        public string st_cidadeentrega_sac { get; set; }

    }
}