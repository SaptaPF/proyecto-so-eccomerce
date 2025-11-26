using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class HomeViewModel
    {
        // 1. Para la sección "Hero" (Producto del Mes)
        public ProductoDto? ProductoDelMes { get; set; }

        // 2. Para la sección "Productos Destacados"
        public IEnumerable<ProductoDto> ProductosDestacados { get; set; } = new List<ProductoDto>();

        // 3. Para la sección "Categorías Populares"
        public IEnumerable<CategoriaDto> CategoriasPopulares { get; set; } = new List<CategoriaDto>();

        // 4. Para la sección "Lo que dicen nuestros clientes"
        public IEnumerable<ResenaDto> ResenasRecientes { get; set; } = new List<ResenaDto>();
    }
}
