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
    public class PedidoAdminProfile : Profile
    {
        public PedidoAdminProfile()
        {
            CreateMap<Pedido, PedidoListDto>()
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));

            CreateMap<Pedido, PedidoDetailViewModel>()
                .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
                .ForMember(dest => dest.UsuarioEmail, opt => opt.MapFrom(src => src.Usuario.Email))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.DireccionEnvio, opt => opt.MapFrom(src =>
                    src.DireccionEnvio != null
                    ? $"{src.DireccionEnvio.Calle}, {src.DireccionEnvio.Ciudad}, {src.DireccionEnvio.CodigoPostal}"
                    : "Sin dirección registrada"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DetallesPedido));

            CreateMap<DetallePedido, DetallePedidoDto>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre));
        }
    }
}
