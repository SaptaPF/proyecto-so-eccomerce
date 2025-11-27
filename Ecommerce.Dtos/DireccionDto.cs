using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class DireccionDto
    {
        public int DireccionId { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria")]
        [StringLength(200)]
        public string Calle { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(100)]
        public string Estado { get; set; } = string.Empty; 

        [Required(ErrorMessage = "El código postal es obligatorio")]
        [StringLength(20)]
        public string CodigoPostal { get; set; } = string.Empty;
    }
}