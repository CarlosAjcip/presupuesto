using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class Transaccion
    {
        public int id_transacciones { get; set; }
        [Display(Name = "Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime fechaTransaccion { get; set; } = DateTime.Today;
        public decimal monto { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no puede pasar de {1} caracteres")]
        public string nota { get; set; }
        public int id_usuarios { get; set; }
        [Display(Name = "Cuenta")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una Cuenta")]
        public int id_cuenta { get; set; }
        [Display(Name = "Categoria")]
        [Range(1,maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una Categoria")]
        public int id_categorias { get; set; }
        [Display(Name = "Tipo Operacion")]
        public TipoOperacion id_TiposOp { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }

    }
}
