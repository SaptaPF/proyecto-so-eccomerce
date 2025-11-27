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
        public int Rating { get; set; } 

        [StringLength(500)]
        public string Comentario { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public virtual Producto Producto { get; set; } = null!;

        public string UsuarioId { get; set; } = null!; 
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; } = null!;
    }
}
