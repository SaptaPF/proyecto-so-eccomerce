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
    public class CarritoConfiguration : IEntityTypeConfiguration<Carrito>
    {
        public void Configure(EntityTypeBuilder<Carrito> builder)
        {
            builder.ToTable("Carritos");

            builder.HasKey(c => c.CarritoId);

            // Índice único para asegurar 1 a 1 con Usuario
            builder.HasIndex(c => c.UsuarioId).IsUnique();

            // Relación 1 a M con ItemCarrito
            builder.HasMany(c => c.Items)
                .WithOne(i => i.Carrito)
                .HasForeignKey(i => i.CarritoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Si se borra el carrito, se borran sus items
        }
    }
}
