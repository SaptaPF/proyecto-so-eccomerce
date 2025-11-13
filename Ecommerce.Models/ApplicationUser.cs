using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public virtual Carrito? Carrito { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();

    }
}
