using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public class Resena
    {
        [Key]
        public int ResenaId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // La puntuación (ej. 3 estrellas)

        [StringLength(500)]
        public string Comentario { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // --- Relaciones (Claves Foráneas) ---

        // 1. Relación con Producto
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;

        // 2. Relación con Usuario
        public string UsuarioId { get; set; } = null!; // Asumiendo que usas Identity
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; } = null!;
    }
}
