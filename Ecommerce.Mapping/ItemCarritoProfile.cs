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
    public class ItemCarritoProfile : Profile
    {
        public ItemCarritoProfile()
        {
            CreateMap<ItemCarrito, ItemCarritoDto>()
                .ForMember(
                    dest => dest.NombreProducto,
                    opt => opt.MapFrom(src => src.Producto.Nombre)
                )
                .ForMember(
                    dest => dest.PrecioUnitario,
                    opt => opt.MapFrom(src => src.Producto.Precio)
                )
                .ForMember(
                    dest => dest.Subtotal,
                    opt => opt.MapFrom(src => src.Cantidad * src.Producto.Precio)
                )
            .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.Producto.ImagenUrl));
        }
    }
}
