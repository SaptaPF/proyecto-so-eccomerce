using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ProductoUpsertViewModel
    {
        public ProductoUpsertDto Producto { get; set; } = new ProductoUpsertDto();
        public List<CategoriaCheckboxDto> CategoriasDisponibles { get; set; } = new List<CategoriaCheckboxDto>();
    }
    
}
