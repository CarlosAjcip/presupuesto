﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace presupuesto.Models
{
    public class TransaccionCreacionVMD : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }
       
    }
}
