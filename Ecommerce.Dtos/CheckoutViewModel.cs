using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class CheckoutViewModel
    {
        public CarritoDto Carrito { get; set; } = new CarritoDto();

     
        public List<DireccionDto> DireccionesGuardadas { get; set; } = new List<DireccionDto>();

      
        public DireccionDto NuevaDireccion { get; set; } = new DireccionDto();
    }
}
