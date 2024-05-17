using presupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class Cuenta
    {
        public int id_cuenta { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerdio")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Descripcion { get; set; }
        [Display(Name = "Tipo Cuenta")]
        public int id_TiposCuen { get; set; }

        public string TipoCuenta { get; set; }
    }
}
