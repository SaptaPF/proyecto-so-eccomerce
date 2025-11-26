namespace Ecommerce.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
        public virtual ICollection<ProductoCategoria> ProductoCategorias { get; set; } = new List<ProductoCategoria>();
        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
        public virtual ICollection<ItemCarrito> ItemsCarrito { get; set; } = new List<ItemCarrito>();
        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    }
}
