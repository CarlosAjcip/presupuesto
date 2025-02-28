﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class RegistroVMD
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo debe ser un correo electronico")]
        public string Email { get; set; }
        [Required(ErrorMessage ="El campo {0} debe ser requerido")]
        public string Password { get; set; }
    }
}
