using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
        public EstadoPedido Estado { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPedido { get; set; }

        [Required]
        public string UsuarioId { get; set; } = null!;
        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; } = null!;
        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

        public int? DireccionEnvioId { get; set; }
        [ForeignKey("DireccionEnvioId")]
        public virtual Direccion? DireccionEnvio { get; set; }
    }
    //public enum EstadoPedido
    //{
    //    Pendiente,
    //    Procesando,
    //    Enviado,
    //    Completado,
    //    Cancelado
    //}

}