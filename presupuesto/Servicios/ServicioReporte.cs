using presupuesto.Models;

namespace presupuesto.Servicios
{
    public interface IServicioReporte
    {
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemana(int id_usuario, int mes, int anio, dynamic ViewBag);
        Task<ReporteTransccionesDetallas> ObtenerReportesTransaccionesDetalladas(int id_usuarios, int mes, int anio, dynamic ViewBag);
        Task<ReporteTransccionesDetallas> ObtenerReporteTramsaccionesDetalladasPorCuenta(int id_usuarios, int id_cuentas, int mes, int anio, dynamic ViewBag);
    }
    public class ServicioReporte : IServicioReporte
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;

        public ServicioReporte(IRepositorioTransacciones repositorioTransacciones, IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<ReporteTransccionesDetallas>ObtenerReportesTransaccionesDetalladas(int id_usuarios, int mes, int anio, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaIncioyFin(mes, anio);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                id_usuarios = id_usuarios,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };


            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(parametro);
            var modelo = GenerarReporteTransaccionesDetalladas(FechaInicio, FechaFin, transacciones);
            AsignarValoresAlViewBag(ViewBag, FechaInicio);
            return modelo; 
        }

        public async Task<ReporteTransccionesDetallas> ObtenerReporteTramsaccionesDetalladasPorCuenta(int id_usuarios, int id_cuentas,int mes,int anio, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaIncioyFin(mes, anio);

            var obtenerTransaccionesPorCuenta = new ObtenerTransccionesPorCuenta()
            {
                id_cuenta = id_cuentas,
                id_usuarios = id_usuarios,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporteTransaccionesDetalladas(FechaInicio, FechaFin, transacciones);

            AsignarValoresAlViewBag(ViewBag, FechaInicio);

            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic ViewBag, DateTime FechaInicio)
        {
            ViewBag.mesAnterior = FechaInicio.AddMonths(-1).Month;
            ViewBag.anioAnterior = FechaInicio.AddMonths(-1).Year;

            ViewBag.mesPosterior = FechaInicio.AddMonths(1).Month;
            ViewBag.anioPosterior = FechaInicio.AddMonths(1).Year;

            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransccionesDetallas GenerarReporteTransaccionesDetalladas(DateTime FechaInicio, DateTime FechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransccionesDetallas();


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.fechaTransaccion)
                 .GroupBy(x => x.fechaTransaccion)
                 .Select(grupo => new ReporteTransccionesDetallas.TranaccionesPorFecha()
                 {
                     FechaTransaccion = grupo.Key,
                     Transacciones = grupo.AsEnumerable()
                 });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = FechaInicio;
            modelo.FechaFin = FechaFin;
            return modelo;
        }

        private (DateTime FechaInicio,DateTime FechaFin) GenerarFechaIncioyFin(int mes , int anio)
        {
            DateTime FechaInicio;
            DateTime FechaFin;

            if (mes <= 0 || mes > 12 || anio <= 1900)
            {
                var hoy = DateTime.Today;
                FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                FechaInicio = new DateTime(anio, mes, 1);
            }
            FechaFin = FechaInicio.AddMonths(1).AddDays(-1);

            return (FechaInicio, FechaFin);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemana(int id_usuario, int mes, int anio, dynamic ViewBag)
        {
            (DateTime FechaInicio, DateTime FechaFin) = GenerarFechaIncioyFin(mes, anio);
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                id_usuarios = id_usuario,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };

            AsignarValoresAlViewBag(ViewBag, FechaInicio);
            var modelo = await repositorioTransacciones.ObtenerPorSemana(parametro);
            return modelo;
        }
    }
}
