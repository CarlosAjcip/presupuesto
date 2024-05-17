using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using presupuesto.Models;
using presupuesto.Servicios;

namespace presupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuario servicioUsuario;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuario servicioUsuario)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuario = servicioUsuario;
        }
        // GET: TiposCuentasController
        public async Task<IActionResult> Index()
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(id_usuarios);
                return View(tiposCuentas);
        }

        // GET: TiposCuentasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TiposCuentasController/Create
        public ActionResult Crear()
        {
            
            return View();
        }

        // POST: TiposCuentasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TiposCuentas tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var yaexistetpocuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre,tipoCuenta.id_usuarios);

            if (yaexistetpocuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");
                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta); 

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var id_usuario = servicioUsuario.ObtenerUsuarioServcio();
            var yaexisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, id_usuario);

            if (yaexisteTipoCuenta)
            {
                return Json($"el nombre {nombre} ya existe");
            }

            return Json(true);
        }

        // GET: TiposCuentasController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
          
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, id_usuarios);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        // POST: TiposCuentasController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Editar(TiposCuentas tiposCuentas)
        {

            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tiposCuentas.id_tiposCuen, id_usuarios);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tiposCuentas);
            return RedirectToAction("Index");
        }

        // GET: TiposCuentasController/Delete/5
        [HttpGet]
        public async Task<ActionResult> Borrar(int id)
        {
            
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, id_usuarios);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        // POST: TiposCuentasController/Delete/5
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id_tiposCuen)
        {
           
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id_tiposCuen, id_usuarios);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Borrar(id_tiposCuen);
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var tipoCuenta = await repositorioTiposCuentas.Obtener(id_usuarios);
            var idsTiposCuentas = tipoCuenta.Select(x => x.id_tiposCuen);
            var tiposCuenNopertenencealUsuario = ids.Except(idsTiposCuentas).ToList();

            if (tiposCuenNopertenencealUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor,indice) =>
            new TiposCuentas()
            {
                id_tiposCuen = valor,
                Orden = indice + 1
            }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
