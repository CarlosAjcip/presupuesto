using System.Security.Claims;

namespace presupuesto.Servicios
{
    public interface IServicioUsuario
    {
        int ObtenerUsuarioServcio();
    }
    public class ServiciosUsuario : IServicioUsuario
    {
        private readonly HttpContext httpContext;
        public ServiciosUsuario(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public int ObtenerUsuarioServcio()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }
            else
            {
                throw new ApplicationException("El usuario no esta autenticado");
            }
            //return 1;
        }
    }
}
