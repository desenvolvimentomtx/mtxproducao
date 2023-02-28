using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MatrizTributaria.Models.ViewModels
{
    public class LegislacaoViewModel
    {

        public int id { get; set; }

        [Required(ErrorMessage = "Campo fundamento legal é obrigatório", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "O mínimo são 4 caracteres")]
        public string fundLegal { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tributacao> tributacoes { get; set; }
    }
}