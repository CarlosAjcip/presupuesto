using Microsoft.AspNetCore.Mvc.Rendering;

namespace presupuesto.Models
{
    public class CuentaCreacionVMD : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuenta { get; set; }
    }
}
