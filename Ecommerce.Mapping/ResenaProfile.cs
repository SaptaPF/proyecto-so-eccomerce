using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Mapping
{
    public class ResenaProfile : Profile
    {
        public ResenaProfile()
        {
            // Mapea la entidad Resena al ResenaDto
            CreateMap<Resena, ResenaDto>()
                // Rating y Comentario se mapean automáticamente

                // Mapeo personalizado para el nombre del cliente
                .ForMember(
                    dest => dest.NombreCliente,
                    opt => opt.MapFrom(src =>
                        // Asume que Resena.Usuario (ApplicationUser) tiene Nombre y Apellido
                        // (Estos campos los añadimos al ApplicationUser al principio)
                        src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}" : "Anónimo"
                    )
                )

                // Mapeo personalizado para las iniciales (para el avatar)
                .ForMember(
                    dest => dest.InicialesCliente,
                    opt => opt.MapFrom(src =>
                        (src.Usuario != null && !string.IsNullOrEmpty(src.Usuario.Nombre) && !string.IsNullOrEmpty(src.Usuario.Apellido))
                        // Toma la primera letra del nombre y la primera del apellido
                        ? $"{src.Usuario.Nombre.Substring(0, 1)}{src.Usuario.Apellido.Substring(0, 1)}".ToUpper()
                        : "??"
                    )
                );
        }
    }
}
