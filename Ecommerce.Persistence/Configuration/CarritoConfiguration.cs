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

            builder.HasIndex(c => c.UsuarioId).IsUnique();

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Carrito)
                .HasForeignKey(i => i.CarritoId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
