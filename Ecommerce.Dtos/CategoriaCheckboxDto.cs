using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class CategoriaCheckboxDto
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool IsChecked { get; set; } 
    }
}
