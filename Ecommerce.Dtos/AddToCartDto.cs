using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class AddToCartDto
    {
        // El ID del producto que se quiere añadir
        public int ProductoId { get; set; }

        // La cantidad (normalmente 1)
        public int Cantidad { get; set; }
    }
}
