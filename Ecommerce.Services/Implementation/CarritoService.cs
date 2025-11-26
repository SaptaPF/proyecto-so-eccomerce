using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services.Implementation
{
    public class CarritoService : ICarritoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CarritoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CarritoDto> AddItemAsync(string usuarioId, int productoId, int cantidad)
        {
            // 1. Obtener o crear el carrito
            var carrito = await GetOrCreateCarritoAsync(usuarioId);

            // 2. Buscar si el ítem ya existe en el carrito
            var itemExistente = await _unitOfWork.ItemCarritoRepository.GetFirstOrDefaultAsync(
                i => i.CarritoId == carrito.CarritoId && i.ProductoId == productoId
            );

            if (itemExistente != null)
            {
                // Si existe, solo suma la cantidad
                itemExistente.Cantidad += cantidad;
                _unitOfWork.ItemCarritoRepository.Update(itemExistente);
            }
            else
            {
                // Si no existe, crea un nuevo ítem
                var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(productoId);
                if (producto == null)
                    throw new Exception("Producto no encontrado");

                var nuevoItem = new ItemCarrito
                {
                    CarritoId = carrito.CarritoId,
                    ProductoId = productoId,
                    Cantidad = cantidad,
                };
                await _unitOfWork.ItemCarritoRepository.AddAsync(nuevoItem);
            }

            // 3. Guardar cambios
            await _unitOfWork.SaveAsync();

            // 4. Devolver el DTO actualizado (ahora con el total correcto)
            return await GetCarritoDtoAsync(usuarioId);
        }

        public async Task<CarritoDto> GetCarritoDtoAsync(string usuarioId)
        {
            var carrito = await _unitOfWork.CarritoRepository.GetFirstOrDefaultAsync(
                c => c.UsuarioId == usuarioId,
                // Incluimos todos los datos necesarios para los DTOs
                includeProperties: "Items.Producto"
            );

            if (carrito == null)
            {
                // Si no tiene carrito, devuelve un DTO vacío
                return new CarritoDto { Items = new List<ItemCarritoDto>(), TotalGeneral = 0 };
            }

            // 1. Mapeamos la entidad al DTO
            var carritoDto = _mapper.Map<CarritoDto>(carrito);

            // ---
            // ¡¡AQUÍ ESTÁ EL ARREGLO!!
            // 2. Calculamos el TotalGeneral sumando los subtotales de los items
            // (Los subtotales SÍ se calculan en 'ItemCarritoProfile')
            // ---
            carritoDto.TotalGeneral = carritoDto.Items.Sum(i => i.Subtotal);

            // 3. Devolvemos el DTO completo
            return carritoDto;
        }

        public async Task<int> GetItemCountAsync(string usuarioId)
        {
            var carrito = await _unitOfWork.CarritoRepository.GetFirstOrDefaultAsync(
                c => c.UsuarioId == usuarioId,
                includeProperties: "Items"
            );

            if (carrito == null)
                return 0;

            // Devuelve la suma de las cantidades de todos los ítems
            return carrito.Items.Sum(i => i.Cantidad);
        }

        public async Task<CarritoDto> RemoveItemAsync(string usuarioId, int itemCarritoId)
        {
            var item = await _unitOfWork.ItemCarritoRepository.GetFirstOrDefaultAsync(
                i => i.ItemCarritoId == itemCarritoId && i.Carrito.UsuarioId == usuarioId,
                includeProperties: "Carrito"
            );

            if (item != null)
            {
                _unitOfWork.ItemCarritoRepository.Remove(item);
                await _unitOfWork.SaveAsync();
            }

            // Devolver el carrito actualizado (ahora con el total correcto)
            return await GetCarritoDtoAsync(usuarioId);
        }


        // --- Funciones Privadas de Ayuda ---

        private async Task<Carrito> GetOrCreateCarritoAsync(string usuarioId)
        {
            var carrito = await _unitOfWork.CarritoRepository.GetFirstOrDefaultAsync(
                c => c.UsuarioId == usuarioId
            );

            if (carrito == null)
            {
                // No existe, creamos uno nuevo
                carrito = new Carrito { UsuarioId = usuarioId };
                await _unitOfWork.CarritoRepository.AddAsync(carrito);
                await _unitOfWork.SaveAsync();
            }

            return carrito;
        }
    }
}