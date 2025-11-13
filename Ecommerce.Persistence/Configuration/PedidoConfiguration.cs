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

            // Relación 1 a M con DetallePedido
            builder.HasMany(p => p.DetallesPedido)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el pedido, se borran sus detalles

            // Relación 1 a M con Direccion (Opcional, puede ser nulo)
            builder.HasOne(p => p.DireccionEnvio)
                .WithMany() // Una dirección no necesita saber de qué pedidos es
                .HasForeignKey(p => p.DireccionEnvioId)
                .IsRequired(false) // Un pedido puede no tener dirección física (ej. digital)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra la dirección, el pedido queda, pero sin dirección
        }
    }
}
