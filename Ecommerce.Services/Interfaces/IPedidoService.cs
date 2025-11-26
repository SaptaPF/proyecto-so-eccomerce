using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface IPedidoService
    {
        /// <summary>
        /// Crea un Pedido en la base de datos basado en el carrito actual del usuario.
        /// Esta es la operación de negocio más importante.
        /// </summary>
        /// <param name="usuarioId">El ID del usuario que compra.</param>
        /// <param name="direccionId">El ID de la dirección de envío seleccionada.</param>
        /// <returns>El Pedido que se acaba de crear.</returns>
        Task<Pedido> CrearPedidoDesdeCarritoAsync(string usuarioId, int direccionId);
    }
}
