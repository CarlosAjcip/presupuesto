using AutoMapper;
using presupuesto.Models;
using System.Runtime.InteropServices;

namespace presupuesto.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionVMD>();
            CreateMap<TransaccionActualizacionVMD, Transaccion>().ReverseMap();
        }
    }
}
