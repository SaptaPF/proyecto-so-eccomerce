using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ItemCarritoDto
    {
        public int ItemCarritoId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;

        // El precio al momento de añadirlo (o el actual)
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }

        // Propiedad calculada (Precio * Cantidad)
        public decimal Subtotal { get; set; }
        public string? ImagenUrl { get; set; } // <-- AÑADIR

        // (Opcional) URL de la imagen para mostrar en el resumen del carrito
        // public string ImagenUrl { get; set; }
    }
}
