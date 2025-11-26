using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class CheckoutViewModel
    {
        /// <summary>
        /// El resumen del carrito (ítems, total, etc.).
        /// </summary>
        public CarritoDto Carrito { get; set; } = new CarritoDto();

        /// <summary>
        /// La lista de direcciones que el usuario ya tiene guardadas.
        /// </summary>
        public List<DireccionDto> DireccionesGuardadas { get; set; } = new List<DireccionDto>();

        /// <summary>
        /// El DTO que se usará para el binding del formulario "Nueva Dirección".
        /// </summary>
        public DireccionDto NuevaDireccion { get; set; } = new DireccionDto();
    }
}
