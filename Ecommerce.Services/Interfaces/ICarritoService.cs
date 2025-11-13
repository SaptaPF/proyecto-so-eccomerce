using Ecommerce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface ICarritoService
    {
        /// <summary>
        /// Obtiene el DTO del carrito para un usuario.
        /// </summary>
        Task<CarritoDto> GetCarritoDtoAsync(string usuarioId);

        /// <summary>
        /// Obtiene solo el NÚMERO de items en el carrito (para el ícono de la barra de navegación).
        /// </summary>
        Task<int> GetItemCountAsync(string usuarioId);

        /// <summary>
        /// Añade un producto al carrito de un usuario.
        /// Devuelve el estado actualizado del carrito.
        /// </summary>
        Task<CarritoDto> AddItemAsync(string usuarioId, int productoId, int cantidad);

        /// <summary>
        /// Elimina un ítem (por su ItemCarritoId) del carrito de un usuario.
        /// Devuelve el estado actualizado del carrito.
        /// </summary>
        Task<CarritoDto> RemoveItemAsync(string usuarioId, int itemCarritoId);
    }
}