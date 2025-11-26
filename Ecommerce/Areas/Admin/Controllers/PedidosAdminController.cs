using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models; // Para EstadoPedido enum
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PedidosAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PedidosAdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /Admin/PedidosAdmin
        public async Task<IActionResult> Index()
        {
            // Incluimos "Usuario" para poder mostrar el email
            var pedidos = await _unitOfWork.PedidoRepository.GetAllAsync(
                orderBy: q => q.OrderByDescending(p => p.FechaPedido),
                includeProperties: "Usuario"
            );

            var dto = _mapper.Map<List<PedidoListDto>>(pedidos);
            return View(dto);
        }

        // GET: /Admin/PedidosAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Necesitamos incluir MUCHAS cosas para el detalle completo
            var pedido = await _unitOfWork.PedidoRepository.GetFirstOrDefaultAsync(
                p => p.PedidoId == id,
                includeProperties: "Usuario,DireccionEnvio,DetallesPedido.Producto"
            );

            if (pedido == null) return NotFound();

            var dto = _mapper.Map<PedidoDetailViewModel>(pedido);
            return View(dto);
        }

        // POST: /Admin/PedidosAdmin/CambiarEstado
        // Acción simple para simular el despacho
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int pedidoId, string nuevoEstado)
        {
            var pedido = await _unitOfWork.PedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null) return NotFound();

            if (Enum.TryParse<EstadoPedido>(nuevoEstado, out var estadoEnum))
            {
                pedido.Estado = estadoEnum;
                _unitOfWork.PedidoRepository.Update(pedido);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Details), new { id = pedidoId });
        }
    }
}