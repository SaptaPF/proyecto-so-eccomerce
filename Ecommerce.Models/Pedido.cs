namespace Ecommerce.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }

        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        public bool Estado { get; set; } 

        public decimal TotalPedido { get; set; }

        public string UsuarioId { get; set; } = null!; 

        public virtual ApplicationUser Usuario { get; set; } = null!;

        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

        public int? DireccionEnvioId { get; set; }

        public virtual Direccion? DireccionEnvio { get; set; }
    }
}
