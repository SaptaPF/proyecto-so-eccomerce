namespace Ecommerce.Models
{
    public class ItemCarrito
    {
        public int ItemCarritoId { get; set; }
        public int Cantidad { get; set; }

        public int CarritoId { get; set; }
        public virtual Carrito Carrito { get; set; } = null!;

        public int ProductoId { get; set; }
        public virtual Producto Producto { get; set; } = null!;
    }
}
