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

        // Datos del cliente que escribió la reseña
        public string NombreCliente { get; set; } = string.Empty;

        // (Opcional) Iniciales del cliente para el avatar (Ej. "CM" de Carlos Mendoza)
        public string InicialesCliente { get; set; } = string.Empty;
    }
}
