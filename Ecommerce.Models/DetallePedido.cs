namespace Ecommerce.Models
{
    public class DetallePedido
    {
        public int DetallePedidoId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; } = null!;

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; } = null!;
    }
}
