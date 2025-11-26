using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [Authorize] // Solo usuarios logueados
    public class PedidosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PedidosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Helper para obtener el ID del usuario actual
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: /Pedidos/Index (Mis Pedidos)
        public async Task<IActionResult> Index()
        {
            var usuarioId = GetCurrentUserId();

            // Obtener SOLO los pedidos de este usuario
            var pedidos = await _unitOfWork.PedidoRepository.GetAllAsync(
                filter: p => p.UsuarioId == usuarioId,
                orderBy: q => q.OrderByDescending(p => p.FechaPedido)
            );

            // Reusamos el DTO que creamos para el Admin, ¡nos sirve igual!
            var dto = _mapper.Map<List<PedidoListDto>>(pedidos);
            return View(dto);
        }

        // GET: /Pedidos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var usuarioId = GetCurrentUserId();

            // Buscar el pedido, asegurándonos que pertenezca al usuario (Seguridad)
            var pedido = await _unitOfWork.PedidoRepository.GetFirstOrDefaultAsync(
                p => p.PedidoId == id && p.UsuarioId == usuarioId,
                includeProperties: "DireccionEnvio,DetallesPedido.Producto"
            );

            if (pedido == null) return NotFound();

            var dto = _mapper.Map<PedidoDetailViewModel>(pedido);
            return View(dto);
        }
    }
}