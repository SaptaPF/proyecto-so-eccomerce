using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class AddToCartDto
    {
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }
    }
}
