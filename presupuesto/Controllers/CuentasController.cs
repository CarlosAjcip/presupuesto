using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using presupuesto.Models;
using presupuesto.Servicios;

namespace presupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
		private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReporte servicioReporte;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,IServicioUsuario servicioUsuario,IRepositorioCuentas repositorioCuentas,
            IMapper mapper, IRepositorioTransacciones repositorioTransacciones,IServicioReporte servicioReporte)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuario = servicioUsuario;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
			this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReporte = servicioReporte;
        }

        // GET: CuentasController
        public async Task<IActionResult> Index()
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var CuentasConTipoCuenta = await repositorioCuentas.Buscar(id_usuarios);

            var modelo = CuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasVM
                {
                    TipoCuenta = grupo.Key,
                    Cuenta = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        // GET: CuentasController/Details/5
   

        // GET: CuentasController/Create
        public async  Task<IActionResult> Crear()
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
         
            var modelo = new CuentaCreacionVMD();

            modelo.TiposCuenta = await ObtenerTiposCuentas(id_usuarios);

            return View(modelo);
        }

        // POST: CuentasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CuentaCreacionVMD cuenta)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var TiposCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.id_TiposCuen,id_usuarios);

            if (TiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuenta =  await ObtenerTiposCuentas(id_usuarios);
                return View(cuenta);
            }

            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int id_usuarios)
        {
            var TiposCuentas = await repositorioTiposCuentas.Obtener(id_usuarios);
            return TiposCuentas.Select(x => new SelectListItem(x.Nombre, x.id_tiposCuen.ToString()));

        }


        // GET: CuentasController/Edit/5
        public async Task<IActionResult> Editar(int id)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuarios);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            //forma manual de mappear de una clase a otra
            //var modelo = new CuentaCreacionVMD()
            //{
            //    id_cuenta = cuenta.id_cuenta,
            //    Nombre = cuenta.Nombre,
            //    id_TiposCuen = cuenta.id_TiposCuen,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance
            //};

            var modelo = mapper.Map<CuentaCreacionVMD>(cuenta);

            modelo.TiposCuenta = await ObtenerTiposCuentas(id_usuarios);

            return View(modelo);
        }
        // POST: CuentasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CuentaCreacionVMD cuentaEditar)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.id_cuenta, id_usuarios);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaEditar.id_TiposCuen, id_usuarios);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        // GET: CuentasController/Delete/5
        [HttpGet]
        public async  Task<IActionResult> Borrar(int id)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuarios);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            return View(cuenta);
        }

        // POST: CuentasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarCuenta(int id_cuenta)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var cuenta = await repositorioCuentas.ObtenerPorId(id_cuenta, id_usuarios);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(id_cuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalle(int id, int mes, int anio)
        {
			var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, id_usuarios);

			if (cuenta is null)
			{
				return RedirectToAction("NoEncontrado", "Home");
			}

            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReporte.ObtenerReporteTramsaccionesDetalladasPorCuenta(id_usuarios, id, mes, anio, ViewBag);

            return View(modelo);
        }
    }
}
