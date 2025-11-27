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
            CreateMap<Resena, ResenaDto>()

                .ForMember(
                    dest => dest.NombreCliente,
                    opt => opt.MapFrom(src =>
                        
                        src.Usuario != null ? $"{src.Usuario.Nombre} {src.Usuario.Apellido}" : "Anónimo"
                    )
                )

                .ForMember(
                    dest => dest.InicialesCliente,
                    opt => opt.MapFrom(src =>
                        (src.Usuario != null && !string.IsNullOrEmpty(src.Usuario.Nombre) && !string.IsNullOrEmpty(src.Usuario.Apellido))
                        ? $"{src.Usuario.Nombre.Substring(0, 1)}{src.Usuario.Apellido.Substring(0, 1)}".ToUpper()
                        : "??"
                    )
                );
        }
    }
}
