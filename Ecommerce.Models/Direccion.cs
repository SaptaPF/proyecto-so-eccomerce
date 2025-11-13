namespace Ecommerce.Models
{
    public class Direccion
    {
        public int DireccionId { get; set; }

        public string Calle { get; set; } = string.Empty;

        public string Ciudad { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string CodigoPostal { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = null!;
        public virtual ApplicationUser Usuario { get; set; } = null!;
    }
}
