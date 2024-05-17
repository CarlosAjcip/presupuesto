using Microsoft.AspNetCore.Mvc;
using presupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class TiposCuentas
    {
        public int id_tiposCuen { get; set; }
        [Required(ErrorMessage = "El campo {0} es querido")]
        [Display(Name = "Nombre del tipo Cuenta")] //esto cambiar el valor del modelo o del label 
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public int id_usuarios { get; set; }

    }
}
