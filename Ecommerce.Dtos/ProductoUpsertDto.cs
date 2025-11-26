using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Dtos
{
    public class ProductoUpsertDto
    {
        public int ProductoId { get; set; } // 0 si es nuevo, > 0 si es para editar

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no debe exceder los 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido.")]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser un valor positivo.")]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es requerido.")]
        [Range(0, 100000, ErrorMessage = "El stock debe ser 0 o un valor positivo.")]
        public int Stock { get; set; }

        /// <summary>
        /// Lista de los IDs de las categorías seleccionadas en el formulario.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar al menos una categoría.")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos una categoría.")]
        public List<int> CategoriaIds { get; set; } = new List<int>();

        // Para mostrar la imagen actual (si existe)
        public string? ImagenUrl { get; set; }

        // Para recibir el archivo del formulario (No va a la BD, solo al controlador)
        [Display(Name = "Imagen del Producto")]
        public IFormFile? ImagenArchivo { get; set; }
    }
}
