using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services.Implementation
{
    public class PedidoService : IPedidoService
    {
        private readonly IUnitOfWork _unitOfWork;
  
        public PedidoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pedido> CrearPedidoDesdeCarritoAsync(string usuarioId, int direccionId)
        {
            // 1. Obtener el carrito y sus ítems
            var carrito = await _unitOfWork.CarritoRepository.GetFirstOrDefaultAsync(
                c => c.UsuarioId == usuarioId,
                includeProperties: "Items.Producto" 
            );

            if (carrito == null || !carrito.Items.Any())
            {
                throw new InvalidOperationException("El carrito está vacío.");
            }

            // 2. Calcular el total (basado en los precios actuales de los productos)
            decimal totalPedido = 0;
            foreach (var item in carrito.Items)
            {
                
                if (item.Producto.Stock < item.Cantidad)
                {
                    throw new InvalidOperationException($"Stock insuficiente para {item.Producto.Nombre}. Solo quedan {item.Producto.Stock}.");
                }
                totalPedido += item.Producto.Precio * item.Cantidad;
            }

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                FechaPedido = DateTime.UtcNow,
                Estado = EstadoPedido.Procesando,
                TotalPedido = totalPedido,
                DireccionEnvioId = direccionId
            };

            await _unitOfWork.PedidoRepository.AddAsync(pedido);
            await _unitOfWork.SaveAsync();

            // 4. Crear los Detalles del Pedido (los ítems)
            foreach (var item in carrito.Items)
            {
                var detallePedido = new DetallePedido
                {
                    PedidoId = pedido.PedidoId, // Usamos el ID del pedido recién creado
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    // ¡Crítico! Guardamos el precio al momento de la compra
                    PrecioUnitario = item.Producto.Precio
                };

                await _unitOfWork.DetallePedidoRepository.AddAsync(detallePedido);

                // 5. Descontar el Stock del Producto
                var producto = item.Producto; // Ya lo tenemos cargado

                // (La validación ya se hizo arriba, aquí solo restamos)
                producto.Stock -= item.Cantidad;
                _unitOfWork.ProductoRepository.Update(producto);
            }

            // 6. Vaciar el carrito (Eliminar los ItemsCarrito)
            _unitOfWork.ItemCarritoRepository.RemoveRange(carrito.Items);

 
            await _unitOfWork.SaveAsync();

            return pedido;
        }
    }
}