using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class PedidoListDto
    {
        public int PedidoId { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty; 
        public DateTime FechaPedido { get; set; }
        public decimal TotalPedido { get; set; }
        public string Estado { get; set; } = string.Empty; 
    }
}
