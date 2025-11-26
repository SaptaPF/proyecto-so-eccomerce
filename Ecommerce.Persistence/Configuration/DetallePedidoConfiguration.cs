using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Configuration
{
    public class DetallePedidoConfiguration : IEntityTypeConfiguration<DetallePedido>
    {
        public void Configure(EntityTypeBuilder<DetallePedido> builder)
        {
            builder.ToTable("DetallesPedido");

            builder.HasKey(d => d.DetallePedidoId);

            builder.Property(d => d.Cantidad)
                .IsRequired();

            // Importante: Guardar el precio al momento de la compra
            builder.Property(d => d.PrecioUnitario)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            // Relación con Producto
            builder.HasOne(d => d.Producto)
                .WithMany(p => p.DetallesPedido) // Asumiendo que Producto tiene ICollection<DetallePedido>
                .HasForeignKey(d => d.ProductoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // No dejar borrar un producto si está en pedidos
        }
    }
}
