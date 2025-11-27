using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class CarritoDto
    {
        public int CarritoId { get; set; }

        public List<ItemCarritoDto> Items { get; set; } = new List<ItemCarritoDto>();

        public decimal TotalGeneral { get; set; }
    }
}
