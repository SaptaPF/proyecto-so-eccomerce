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
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaDto>()

                .ForMember(
                    dest => dest.CantidadProductos,
                    opt => opt.MapFrom(src =>
                    
                        src.ProductoCategorias != null ? src.ProductoCategorias.Count : 0
                    )
                );
        }
    }
}
