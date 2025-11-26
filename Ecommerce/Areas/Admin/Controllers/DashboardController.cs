using Ecommerce.Dtos;
using Ecommerce.Models; // Para EstadoPedido
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Obtener todos los pedidos para calcular ventas y pendientes
            var pedidos = await _unitOfWork.PedidoRepository.GetAllAsync();

            // 2. Obtener productos y categorías
            var productos = await _unitOfWork.ProductoRepository.GetAllAsync();
            var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();

            // 3. Calcular métricas
            var viewModel = new DashboardViewModel
            {
                // Sumar solo los pedidos que no están cancelados (o solo los completados, según tu lógica)
                TotalVentas = pedidos
                    .Where(p => p.Estado != EstadoPedido.Cancelado)
                    .Sum(p => p.TotalPedido),

                // Contar pedidos en estado "Pendiente" o "Procesando"
                PedidosPendientes = pedidos
                    .Count(p => p.Estado == EstadoPedido.Procesando || p.Estado == EstadoPedido.Pendiente),

                TotalProductos = productos.Count(),

                TotalCategorias = categorias.Count(),

                // Contar usuarios (usando UserManager)
                TotalUsuarios = _userManager.Users.Count()
            };

            return View(viewModel);
        }
    }
}