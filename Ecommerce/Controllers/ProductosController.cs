using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class ProductosController : Controller
    {
        // 1. El controlador solo conoce la INTERFAZ del servicio
        private readonly IProductoService _productoService;

        // 2. Inyección de Dependencias (DI)
        // Gracias a que lo registramos en Program.cs,
        // ASP.NET Core nos "inyecta" el ProductoService automáticamente.
        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: /Productos/Index (o /Productos)
        public async Task<IActionResult> Index()
        {
            // 3. Llama al servicio (que devuelve DTOs)
            var listaProductos = await _productoService.GetAllAsync();

            // 4. Pasa los DTOs a la Vista
            return View(listaProductos);
        }

        // GET: /Productos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // 1. Llama al servicio
            var producto = await _productoService.GetByIdAsync(id);

            // 2. Lógica simple del controlador
            if (producto == null)
            {
                return NotFound(); // Devuelve un 404
            }

            // 3. Pasa el DTO a la Vista
            return View(producto);
        }

        // ... (Aquí irían tus métodos Create [GET], Create [POST], etc.)
    
    }
}
