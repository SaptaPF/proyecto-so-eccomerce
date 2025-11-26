using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    // ¡Toda esta sección requiere que el usuario esté logueado!
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICarritoService _carritoService;
        private readonly IPedidoService _pedidoService; // <-- 1. INYECTAR EL NUEVO SERVICIO


        public CheckoutController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IPedidoService pedidoService, // <-- 2. AÑADIR AL CONSTRUCTOR
            ICarritoService carritoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _carritoService = carritoService;
            _pedidoService = pedidoService; // <-- 3. ASIGNAR

        }

        // --- Método Helper Privado ---
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ---
        // PASO 1: Muestra la página de Checkout
        // GET: /Checkout/Index
        // ---
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = GetCurrentUserId();
            var carrito = await _carritoService.GetCarritoDtoAsync(usuarioId);

            // 2. Si el carrito está vacío, no puede hacer checkout. Redirigir.
            if (carrito == null || !carrito.Items.Any())
            {
                // (Opcional: puedes añadir un TempData con un mensaje de error)
                return RedirectToAction("Index", "Carrito");
            }

            // 3. Obtener las direcciones guardadas
            var direcciones = await _unitOfWork.DireccionRepository.GetAllAsync(
                d => d.UsuarioId == usuarioId
            );

            // 4. Crear el ViewModel "contenedor"
            var viewModel = new CheckoutViewModel
            {
                Carrito = carrito,
                DireccionesGuardadas = _mapper.Map<List<DireccionDto>>(direcciones),
                NuevaDireccion = new DireccionDto() // Para el formulario de nueva dirección
            };

            return View(viewModel);
        }

        // ---
        // PASO 2: Maneja el formulario de "Añadir Nueva Dirección"
        // POST: /Checkout/Index
        // ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel viewModel)
        {
            var usuarioId = GetCurrentUserId();

            // Solo valida el sub-modelo de NuevaDireccion
            if (ModelState.IsValid)
            {
                var nuevaDireccion = _mapper.Map<Direccion>(viewModel.NuevaDireccion);
                nuevaDireccion.UsuarioId = usuarioId;
                await _unitOfWork.DireccionRepository.AddAsync(nuevaDireccion);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si falla, recargar la página con los datos
            var carrito = await _carritoService.GetCarritoDtoAsync(usuarioId);
            var direcciones = await _unitOfWork.DireccionRepository.GetAllAsync(d => d.UsuarioId == usuarioId);
            viewModel.Carrito = carrito;
            viewModel.DireccionesGuardadas = _mapper.Map<List<DireccionDto>>(direcciones);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int direccionId) // 4. Recibe el ID de la dirección seleccionada
        {
            var usuarioId = GetCurrentUserId();

            // Validación simple
            if (direccionId <= 0)
            {
                // (Opcional: añadir TempData de error)
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 5. ¡AQUÍ ESTÁ LA MAGIA!
                // Llamamos al servicio que creamos, que hace la transacción.
                var nuevoPedido = await _pedidoService.CrearPedidoDesdeCarritoAsync(usuarioId, direccionId);

                // 6. Si todo sale bien, redirigimos a la página de confirmación
                return RedirectToAction(nameof(Confirmacion), new { id = nuevoPedido.PedidoId });
            }
            catch (InvalidOperationException ex)
            {
                // Un error de negocio (ej. "Stock insuficiente" o "Carrito vacío")
                // (Opcional: añadir TempData con ex.Message)
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex)
            {
                // Un error inesperado del sistema
                // (Loggear ex)
                // (Opcional: añadir TempData con un error genérico)
                return RedirectToAction(nameof(Index));
            }
        }

        // ---
        // ¡¡NUEVA ACCIÓN!!
        // PASO 4: Muestra la página de "Gracias por tu compra"
        // GET: /Checkout/Confirmacion/5
        // ---
        [HttpGet]
        public async Task<IActionResult> Confirmacion(int id)
        {
            // Opcional: Podrías buscar el pedido para asegurarte
            // de que pertenece al usuario actual, pero por ahora,
            // solo pasaremos el ID a la vista.
            ViewBag.PedidoId = id;
            return View();
        }
    }
}