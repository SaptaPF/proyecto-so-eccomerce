using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Necesario para obtener el User ID
using System.Threading.Tasks;
using System.Linq; // Necesario para .Sum()

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CarritoController : Controller // Hereda de Controller para Vistas y API
    {
        private readonly ICarritoService _carritoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(
            ICarritoService carritoService,
            UserManager<ApplicationUser> userManager)
        {
            _carritoService = carritoService;
            _userManager = userManager;
        }

        // ---
        // ACCIÓN PARA LA VISTA (PÁGINA HTML)
        // ---

        /// <summary>
        /// Muestra la página principal del carrito de compras.
        /// Esta acción SÍ devuelve una Vista (HTML).
        /// </summary>
        [HttpGet] // Se accede por GET (no es parte de la API)
        [Route("~/Carrito")] // Ruta personalizada: /Carrito
        public async Task<IActionResult> Index()
        {
            var usuarioId = GetCurrentUserId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(); // O redirigir al Login
            }

            var carritoDto = await _carritoService.GetCarritoDtoAsync(usuarioId);
            return View(carritoDto); // Devuelve la VISTA
        }


        // ---
        // ACCIONES DE LA API (ENDPOINTS JSON)
        // ---

        /// <summary>
        /// Endpoint para añadir un producto al carrito.
        /// Se llama con un POST a /api/carrito/add
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (addToCartDto == null || addToCartDto.ProductoId <= 0 || addToCartDto.Cantidad <= 0)
            {
                return BadRequest(new { message = "Datos de producto inválidos." });
            }

            var usuarioId = GetCurrentUserId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            try
            {
                // ¡CAMBIO IMPORTANTE!
                // AddItemAsync ahora devuelve el carrito actualizado
                var carritoActualizado = await _carritoService.AddItemAsync(usuarioId, addToCartDto.ProductoId, addToCartDto.Cantidad);

                // ¡Ahora es más eficiente! No necesitamos llamar a GetItemCountAsync
                var nuevoConteo = carritoActualizado.Items.Sum(i => i.Cantidad);

                return Ok(new
                {
                    message = "Producto añadido al carrito",
                    nuevoConteo = nuevoConteo
                });
            }
            catch (System.Exception ex)
            {
                // Loggear el error (ex.Message)
                return StatusCode(500, new { message = "Ocurrió un error al añadir el producto." });
            }
        }

        /// <summary>
        /// Endpoint para eliminar un ítem del carrito.
        /// Se llama con un POST a /api/carrito/remove
        /// </summary>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDto removeFromCartDto)
        {
            if (removeFromCartDto == null || removeFromCartDto.ItemCarritoId <= 0)
            {
                return BadRequest(new { message = "ID de ítem inválido." });
            }

            var usuarioId = GetCurrentUserId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            // Llamamos al nuevo método del servicio
            var carritoActualizado = await _carritoService.RemoveItemAsync(usuarioId, removeFromCartDto.ItemCarritoId);

            // Devolvemos el DTO del carrito completo y actualizado
            // (El frontend usará esto para redibujar el carrito)
            return Ok(carritoActualizado);
        }

        /// <summary>
        /// Endpoint para obtener solo el conteo de items del carrito.
        /// Se llama con un GET a /api/carrito/count
        /// </summary>
        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            var usuarioId = GetCurrentUserId();
            if (string.IsNullOrEmpty(usuarioId))
            {
                // Si no está logueado, no tiene carrito
                return Ok(new { count = 0 });
            }

            var count = await _carritoService.GetItemCountAsync(usuarioId);
            return Ok(new { count = count });
        }

        // --- Método Helper Privado ---

        /// <summary>
        /// Obtiene el ID del usuario actualmente autenticado.
        /// </summary>
        private string GetCurrentUserId()
        {
            // User es una propiedad de Controller que tiene la info de la cookie
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}