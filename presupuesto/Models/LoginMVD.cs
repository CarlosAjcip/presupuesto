using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class LoginMVD
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo electronico")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} debe ser requerido")]
        public string Password { get; set; }
        public bool Recuerdame { get; set; }
    }
}
