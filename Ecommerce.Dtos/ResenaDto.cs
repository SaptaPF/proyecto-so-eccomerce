using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ResenaDto
    {
        public int Rating { get; set; }
        public string Comentario { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public string InicialesCliente { get; set; } = string.Empty;
    }
}
