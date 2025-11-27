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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Apellido)
                .IsRequired()
                .HasMaxLength(100);


            // 1 a 1 con Carrito
            builder.HasOne(u => u.Carrito)
                .WithOne(c => c.Usuario)
                .HasForeignKey<Carrito>(c => c.UsuarioId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 

            // 1 a M con Pedido
            builder.HasMany(u => u.Pedidos)
                .WithOne(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); 

            // 1 a M con Direccion
            builder.HasMany(u => u.Direcciones)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.UsuarioId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
