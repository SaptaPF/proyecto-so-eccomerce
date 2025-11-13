namespace Ecommerce.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public string UsuarioId { get; set; } = null!;
        public virtual ApplicationUser Usuario { get; set; } = null!;
        public virtual ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
    }
}
