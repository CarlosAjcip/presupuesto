using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using presupuesto.Models;
using presupuesto.Servicios;

namespace presupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuario servicioUsuario;

        public CategoriasController(IRepositorioCategorias repositorioCategorias, IServicioUsuario servicioUsuario)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuario = servicioUsuario;
        }
        // GET: CategoriasController
        public async Task<IActionResult> Index()
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categorias = await repositorioCategorias.Obtener(id_usuarios);

            return View(categorias);
        }

        // GET: CategoriasController/Create
        [HttpGet]
        public  IActionResult Crear()
        {
            return View();
        }

        // POST: CategoriasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            categoria.id_usuarios = id_usuarios;

            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");

        }

        // GET: CategoriasController/Edit/5
        public async  Task<IActionResult> Editar(int id)
        {
          
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categorias = await repositorioCategorias.ObtenerPorId(id, id_usuarios);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categorias);
        }

        // POST: CategoriasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaEditar);
            }

            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categoria = await repositorioCategorias.ObtenerPorId(categoriaEditar.id_categorias, id_usuarios);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.id_usuarios = id_usuarios;
            await repositorioCategorias.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
        }

        // GET: CategoriasController/Delete/5
        public async Task<IActionResult> Borrar(int id)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categorias = await repositorioCategorias.ObtenerPorId(id, id_usuarios);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categorias);
        }

        // POST: CategoriasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrarCategoria(int id_categorias)
        {
            var id_usuarios = servicioUsuario.ObtenerUsuarioServcio();
            var categorias = await repositorioCategorias.ObtenerPorId(id_categorias, id_usuarios);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategorias.Borrar(id_categorias);
            return RedirectToAction("Index");
        }
    }
}
