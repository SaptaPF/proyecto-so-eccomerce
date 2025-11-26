using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ResenaCreateDto
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una puntuación.")]
        [Range(1, 5, ErrorMessage = "La puntuación debe ser entre 1 y 5 estrellas.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        public string Comentario { get; set; } = string.Empty;
    }
}
