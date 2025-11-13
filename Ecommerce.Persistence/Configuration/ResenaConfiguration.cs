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
    public class ResenaConfiguration : IEntityTypeConfiguration<Resena>
    {
        public void Configure(EntityTypeBuilder<Resena> builder)
        {
            // Define el nombre de la tabla
            builder.ToTable("Resenas");

            // Clave Primaria
            builder.HasKey(r => r.ResenaId);

            // Configuración de Propiedades
            builder.Property(r => r.Rating)
                .IsRequired(); // La puntuación es obligatoria

            builder.Property(r => r.Comentario)
                .HasMaxLength(500); // Límite de 500 caracteres

            builder.Property(r => r.Fecha)
                .IsRequired();

            // --- Configuración de Relaciones (Foreign Keys) ---

            // 1. Relación con Producto (Un Producto tiene Muchas Reseñas)
            builder.HasOne(r => r.Producto)           // Una Reseña tiene un Producto
                .WithMany(p => p.Resenas)           // Un Producto tiene muchas Reseñas
                .HasForeignKey(r => r.ProductoId)   // La clave foránea es ProductoId
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);  // Si se borra un Producto, se borran sus reseñas.

            // 2. Relación con Usuario (Un Usuario tiene Muchas Reseñas)
            builder.HasOne(r => r.Usuario)            // Una Reseña tiene un Usuario
                .WithMany(u => u.Resenas)           // Un Usuario tiene muchas Reseñas
                .HasForeignKey(r => r.UsuarioId)    // La clave foránea es UsuarioId
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // NO se borra la reseña si se borra el usuario
                                                    // (o cámbialo a SetNull si quieres anonimizarla)
        }
    }
}
