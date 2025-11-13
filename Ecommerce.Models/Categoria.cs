namespace Ecommerce.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public virtual ICollection<ProductoCategoria> ProductoCategorias { get; set; } = new List<ProductoCategoria>();
    }
}
