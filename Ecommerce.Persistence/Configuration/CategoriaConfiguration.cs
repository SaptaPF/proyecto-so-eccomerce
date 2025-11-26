using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configuration
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");

            builder.HasKey(c => c.CategoriaId);

            builder.Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Descripcion)
                .HasMaxLength(250);
        }
    }
}
