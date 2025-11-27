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
    public class ProductoUpsertProfile : Profile
    {
        public ProductoUpsertProfile()
        {
            CreateMap<Producto, ProductoUpsertDto>()
                .ForMember(
                    dest => dest.CategoriaIds,
                    opt => opt.MapFrom(src =>
                        src.ProductoCategorias.Select(pc => pc.CategoriaId).ToList())

                )
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.ImagenUrl));



            CreateMap<ProductoUpsertDto, Producto>()
                .ForMember(
                    dest => dest.ProductoCategorias,
                    opt => opt.Ignore() 
                )
                .ForMember(dest => dest.DetallesPedido, opt => opt.Ignore())
                .ForMember(dest => dest.ItemsCarrito, opt => opt.Ignore())
                .ForMember(dest => dest.Resenas, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenUrl, opt => opt.Ignore());
        }
    }
}
