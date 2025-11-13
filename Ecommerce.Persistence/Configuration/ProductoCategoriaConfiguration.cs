using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configuration
{
    public class ProductoCategoriaConfiguration : IEntityTypeConfiguration<ProductoCategoria>
    {
        public void Configure(EntityTypeBuilder<ProductoCategoria> builder)
        {
            builder.ToTable("ProductoCategorias");

            // 1. Clave Primaria Compuesta
            builder.HasKey(pc => new { pc.ProductoId, pc.CategoriaId });

            // 2. Relación M-M (Lado de Producto)
            builder.HasOne(pc => pc.Producto)
                .WithMany(p => p.ProductoCategorias)
                .HasForeignKey(pc => pc.ProductoId);

            // 3. Relación M-M (Lado de Categoria)
            builder.HasOne(pc => pc.Categoria)
                .WithMany(c => c.ProductoCategorias)
                .HasForeignKey(pc => pc.CategoriaId);
        }
    }
}
