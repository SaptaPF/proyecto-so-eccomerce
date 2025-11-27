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
        Task<Pedido> CrearPedidoDesdeCarritoAsync(string usuarioId, int direccionId);
    }
}
