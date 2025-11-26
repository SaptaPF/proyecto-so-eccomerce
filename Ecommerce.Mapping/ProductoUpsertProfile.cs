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
            // Mapeo: Entidad Producto -> DTO (Para mostrar en el formulario de "Editar")
            CreateMap<Producto, ProductoUpsertDto>()
                .ForMember(
                    dest => dest.CategoriaIds,
                    opt => opt.MapFrom(src =>
                        // Transforma la lista de entidades de unión en una simple lista de IDs
                        src.ProductoCategorias.Select(pc => pc.CategoriaId).ToList())

                )
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.ImagenUrl)); // Asegúrate de tener ImagenUrl en tu Modelo Producto.



            // Mapeo: DTO -> Entidad Producto (Para guardar en la BD)
            CreateMap<ProductoUpsertDto, Producto>()
                .ForMember(
                    dest => dest.ProductoCategorias,
                    opt => opt.Ignore() // Ignoramos esto. El controlador lo manejará manualmente.
                )
                // Ignoramos todas las otras propiedades de navegación
                .ForMember(dest => dest.DetallesPedido, opt => opt.Ignore())
                .ForMember(dest => dest.ItemsCarrito, opt => opt.Ignore())
                .ForMember(dest => dest.Resenas, opt => opt.Ignore())
            .ForMember(dest => dest.ImagenUrl, opt => opt.Ignore());
        }
    }
}
