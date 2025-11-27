using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Persistence.Configuration
{
    public class DireccionConfiguration : IEntityTypeConfiguration<Direccion>
    {
        public void Configure(EntityTypeBuilder<Direccion> builder)
        {
            builder.ToTable("Direcciones");

            builder.HasKey(d => d.DireccionId);

            builder.Property(d => d.Calle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Ciudad)
                .HasMaxLength(100);

            builder.Property(d => d.Estado)
                .HasMaxLength(100);

            builder.Property(d => d.CodigoPostal)
                .IsRequired()
                .HasMaxLength(20);
            builder.HasOne(d => d.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.UsuarioId);
        }
    }
}
