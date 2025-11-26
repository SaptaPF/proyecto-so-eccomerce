using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public List<string> NombresCategorias { get; set; } = new List<string>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string? ImagenUrl { get; set; }
        public List<ResenaDto> Resenas { get; set; } = new List<ResenaDto>();
    }
}