using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class RemoveFromCartDto
    {
        // El ID del ítem específico en el carrito (NO el ID del producto)
        public int ItemCarritoId { get; set; }
    }
}
