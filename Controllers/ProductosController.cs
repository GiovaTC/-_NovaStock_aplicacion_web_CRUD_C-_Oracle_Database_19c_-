using CrudProductosOracle19c.Data;
using CrudProductosOracle19c.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudProductosOracle19c.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoRepository _repository;

        public ProductosController(ProductoRepository repository)
        => _repository = repository;

        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            ViewBag.TotalProductos = await _repository.ContarAsync();
            return View(await _repository.ObtenerTodosAsync(buscar));
        }
        
        public IActionResult Crear() => View(new Producto());
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            await _repository.CrearAsync(producto);
            TempData["Mensaje"] = "el producto se creo correctamente.";
            return RedirectToAction(nameof(Index));
        }   

        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);
            return producto is null ? NotFound() : View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            var actualizado = await _repository.ActualizarAsync(producto);
            if (!actualizado) return NotFound();

            TempData["Mensaje"] = "el producto se actualizo correctamente!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _repository.ObtenerPorIdAsync(id);
            return producto is null ? NotFound() : View(producto);
        }   
    }
}
