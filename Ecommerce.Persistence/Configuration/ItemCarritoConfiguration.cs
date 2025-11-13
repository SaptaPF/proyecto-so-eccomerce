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
    public class ItemCarritoConfiguration : IEntityTypeConfiguration<ItemCarrito>
    {
        public void Configure(EntityTypeBuilder<ItemCarrito> builder)
        {
            builder.ToTable("ItemsCarrito");

            builder.HasKey(i => i.ItemCarritoId);

            builder.Property(i => i.Cantidad)
                .IsRequired();

            // Relación con Producto
            builder.HasOne(i => i.Producto)
                .WithMany(p => p.ItemsCarrito) // Asumiendo que Producto tiene ICollection<ItemCarrito>
                .HasForeignKey(i => i.ProductoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // No dejar borrar un producto si está en carritos
        }
    }
}
