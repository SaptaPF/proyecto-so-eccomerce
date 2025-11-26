using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class DashboardViewModel
    {
        public decimal TotalVentas { get; set; }
        public int PedidosPendientes { get; set; }
        public int TotalProductos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalCategorias { get; set; }
    }
}
