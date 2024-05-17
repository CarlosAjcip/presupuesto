using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using presupuesto.Models;
using presupuesto.Servicios;
using System.Data;
using System.Reflection;
using System.Transactions;

namespace presupuesto.Controllers
{
   
    public class TransaccionController : Controller
    {
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReporte servicioReporte;

        public TransaccionController(IServicioUsuario servicioUsuario, IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias,
            IRepositorioTransacciones repositorioTransacciones,IMapper mapper,IServicioReporte servicioReporte)
        {
            this.servicioUsuario = servicioUsuario;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReporte = servicioReporte;
        }
        // GET: TransaccionController
        
        public async Task<IActionResult> Index(int mes , int anio)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var modelo = await servicioReporte.ObtenerReportesTransaccionesDetalladas(id_usuarios, mes ,anio, ViewBag);


            return View(modelo);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int id_usuarios)
        {
            var cuentas = await repositorioCuentas.Buscar(id_usuarios);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.id_cuenta.ToString()));
        }

        // GET: TransaccionController/Create
        public async Task<IActionResult> Crear()
        {
            var is_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var modelo = new TransaccionCreacionVMD();
            modelo.Cuentas = await ObtenerCuentas(is_usuarios);
            modelo.Categorias = await ObtenerCategorias(is_usuarios, modelo.id_TiposOp);
            return View(modelo);
        }

        // POST: TransaccionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TransaccionCreacionVMD modelo)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(id_usuarios);
                modelo.Categorias = await ObtenerCategorias(id_usuarios, modelo.id_TiposOp);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.id_cuenta, id_usuarios);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.id_categorias, id_usuarios);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.id_usuarios = id_usuarios;
            if (modelo.id_TiposOp == TipoOperacion.Gastos)
            {
                modelo.monto *= -1;
            }
            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");
        }

        // GET: TransaccionController/Edit/5
        public async  Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, id_usuarios);

            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionVMD>(transaccion);

            if (modelo.id_TiposOp == TipoOperacion.Gastos)
            {
                modelo.MontoAnterior = modelo.monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.id_cuenta;
            modelo.Categorias = await ObtenerCategorias(id_usuarios, transaccion.id_TiposOp);
            modelo.Cuentas = await ObtenerCuentas(id_usuarios);
            modelo.UrlRetorno = urlRetorno;

            return View(modelo);
        }

        // POST: TransaccionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(TransaccionActualizacionVMD modelo)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(id_usuarios);
                modelo.Categorias = await ObtenerCategorias(id_usuarios, modelo.id_TiposOp);
                return View(modelo);
            }

            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.id_cuenta, id_usuarios);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategorias.ObtenerPorId(modelo.id_categorias, id_usuarios);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<Transaccion>(modelo);
            modelo.MontoAnterior = modelo.monto;

            if(modelo.id_TiposOp == TipoOperacion.Gastos)
            {
                transaccion.monto *= -1;
            }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);
            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
       

        }

       
        // POST: TransaccionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrar(int id_transacciones, string urlRetorno = null)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id_transacciones, id_usuarios);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTransacciones.Borrar(id_transacciones);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
            
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int id_usuarios, TipoOperacion id_TiposOp)
        {
            var categorias = await repositorioCategorias.Obtener(id_usuarios, id_TiposOp);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.id_categorias.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion id_TiposOp)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categorias = await ObtenerCategorias(id_usuarios, id_TiposOp);
            return Ok(categorias);

        }


        public async Task<IActionResult> Semanal(int mes, int anio)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = await servicioReporte.ObtenerReporteSemana(id_usuario, mes, anio, ViewBag);

            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new ResultadoObtenerPorSemana()
            {
                Semana = x.Key,
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                Gastos = x.Where(x => x.TipoOperacionId == TipoOperacion.Gastos).Select(x => x.Monto).FirstOrDefault()
            }).ToList();

            if (anio == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                anio = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(anio, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for (int i = 0; i < diasSegmentados.Count(); i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(anio, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(anio, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if (grupoSemana is null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                }
                else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }

            }
            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();
            var modelo = new ReporteSemanVMD();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaRefencia = fechaReferencia;

            return View(modelo);
        }

        public async Task<IActionResult> Mensual(int anio)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            if (anio == 0)
            {
                anio = DateTime.Today.Year;
            }
            var transaccionsePorMes = await repositorioTransacciones.ObtenerPorMes(id_usuario,anio);
            var transaccionesAgrupadas = transaccionsePorMes.GroupBy(x => x.Mes)
                .Select(x => new ResultadoObtenerPorMes()
                {
                    Mes = x.Key,
                    Ingreso = x.Where(x => x.id_tiposOp == TipoOperacion.Ingreso)
                    .Select(x => x.Monto).FirstOrDefault(),
                    Gasto = x.Where(x => x.id_tiposOp == TipoOperacion.Gastos)
                    .Select(x => x.Monto).FirstOrDefault()
                }).ToList();

            for(int mes = 1; mes <= 12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaFerencia = new DateTime(anio, mes, 1);

                if (transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaFerencia
                    });
                }
                else
                {
                    transaccion.FechaReferencia = fechaFerencia;
                }
            }

            transaccionesAgrupadas =transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();
            var modelo = new ReporteMensualVMD();
            modelo.Anio = anio;
            modelo.TransaccionPorMes = transaccionesAgrupadas;

            return View(modelo);
        }

        //generar reporte de excel
        [HttpGet]
        public async Task<FileResult> ExportarExcelPorMes(int mes,int anio)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(
                new ParametroObtenerTransaccionesPorUsuario
                {
                    id_usuarios = id_usuario,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                });
            var nombreArchivo = $"Manejo Presupuesto - {fechaInicio.ToString("MMM yyyy")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);
        }

        private FileResult GenerarExcel(string nombeArchivo, IEnumerable<Transaccion> transaccions)
        {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),
            });
            
           foreach(var transaccion in transaccions)
            {
                dataTable.Rows.Add(transaccion.fechaTransaccion,
                    transaccion.Cuenta,
                    transaccion.Categoria,
                    transaccion.nota,
                    transaccion.monto,
                    transaccion.id_TiposOp);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombeArchivo);
                }
            }
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelPorAnio(int anio)
        {
            var fechaInicio = new DateTime(anio, 1, 1);
            var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
       
            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(
                new ParametroObtenerTransaccionesPorUsuario
                {
                    id_usuarios = id_usuario,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                });

            var monbreArchivo = $"Manejo Presujpuesto = {fechaInicio.ToString("yyyy")}.xlsx";
            return GenerarExcel(monbreArchivo, transacciones);
        }

        [HttpGet]
        public async Task<FileResult> ExportarExcelTodo()
        {
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(1000);
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();

            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(
                new ParametroObtenerTransaccionesPorUsuario
                {
                    id_usuarios = id_usuario,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                });

            var monbreArchivo = $"Manejo Presujpuesto = {fechaInicio.ToString("dd-MM-yyyy")}.xlsx";
            return GenerarExcel(monbreArchivo, transacciones);
        }
        public IActionResult ExcelReporte()
        {
            //var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            return View();
        }
        //calendario
        public async Task<JsonResult> ObtenerTransaccionCalendario(DateTime start,DateTime end)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();

            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(
           new ParametroObtenerTransaccionesPorUsuario
           {
               id_usuarios = id_usuario,
               FechaInicio = start,
               FechaFin = end
           });

            var enventosCalendario = transacciones.Select(transacciones => new EventoCalendario()
            {
                Title = transacciones.monto.ToString("N"),
                Start = transacciones.fechaTransaccion.ToString("yyyy-MM-dd"),
                End = transacciones.fechaTransaccion.ToString("yyyy-MM-dd"),
                Color = (transacciones.id_TiposOp == TipoOperacion.Gastos) ? "Red" : null
            });

            return Json(enventosCalendario);
        }

        public async Task<JsonResult> ObtenerTransaccionesPorfecha(DateTime fecha)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();

            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(
           new ParametroObtenerTransaccionesPorUsuario
           {
               id_usuarios = id_usuario,
               FechaInicio = fecha,
               FechaFin = fecha
           });

            return Json(transacciones);
        }
        public IActionResult Calendario()
        {
            //var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            return View();
        }
    }
}
