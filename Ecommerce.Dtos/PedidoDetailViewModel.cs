using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class PedidoDetailViewModel
    {
        public int PedidoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal TotalPedido { get; set; }

        public string UsuarioNombre { get; set; } = string.Empty;
        public string UsuarioEmail { get; set; } = string.Empty;

        public string DireccionEnvio { get; set; } = string.Empty;
        public List<DetallePedidoDto> Items { get; set; } = new List<DetallePedidoDto>();
    }

    public class DetallePedidoDto
    {
        public string ProductoNombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
