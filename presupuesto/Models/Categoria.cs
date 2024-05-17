using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class Categoria
    {
        public int id_categorias { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        
        public string Nombre { get; set; }

        public int id_usuarios { get; set; }

        [Display(Name = "Tipo Operacion")]
        public TipoOperacion id_tiposOp  { get; set; }
    }
}
