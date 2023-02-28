using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatrizTributaria.Models
{
    [Table("categorias_produto")]
    public class CategoriaProduto
    {
        [Key]
        public int id { get; set; }


        [Display(Name = "Descrição")]
        [Column("CategoriaDescricao")]
        public String descricao { get; set; }

        public virtual ICollection<Produto> produtos { get; set; } //lista de produtos, relacionamento com a tabela produtos
    }
}