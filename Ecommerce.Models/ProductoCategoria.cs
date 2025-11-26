namespace Ecommerce.Models
{
    public class ProductoCategoria
    {
        public int ProductoId { get; set; }
        public int CategoriaId { get; set; }
        public virtual Producto Producto { get; set; } = null!;
        public virtual Categoria Categoria { get; set; } = null!;
    }
}
