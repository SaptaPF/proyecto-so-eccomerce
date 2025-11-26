using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ecommerce.Models; // ¡Asegúrate de tener este using!

namespace Ecommerce.Persistence.Configuration
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.PedidoId);

            builder.Property(p => p.FechaPedido)
                .IsRequired();

            builder.Property(p => p.TotalPedido)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");
            builder.Property(p => p.Estado)
                .IsRequired()
                .HasConversion<string>() 
                .HasMaxLength(50);

            builder.HasMany(p => p.DetallesPedido)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 
            builder.HasOne(p => p.DireccionEnvio)
                .WithMany() 
                .HasForeignKey(p => p.DireccionEnvioId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}