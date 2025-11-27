using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class HomeViewModel
    {
        public ProductoDto? ProductoDelMes { get; set; }

        public IEnumerable<ProductoDto> ProductosDestacados { get; set; } = new List<ProductoDto>();

        public IEnumerable<CategoriaDto> CategoriasPopulares { get; set; } = new List<CategoriaDto>();

        public IEnumerable<ResenaDto> ResenasRecientes { get; set; } = new List<ResenaDto>();
    }
}
