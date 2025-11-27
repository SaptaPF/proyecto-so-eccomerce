using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace Ecommerce.Mapping
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile()
        {
            CreateMap<Producto, ProductoDto>()
                  .ForMember(
                      dest => dest.NombresCategorias,
                      opt => opt.MapFrom(src =>
                          src.ProductoCategorias.Select(pc => pc.Categoria.Nombre).ToList()
                      )
                  )
                  .ForMember(
                      dest => dest.AverageRating,
                      opt => opt.MapFrom(src =>
                          src.Resenas.Any() ? src.Resenas.Average(r => r.Rating) : 0)
                  )
                  .ForMember(
                      dest => dest.ReviewCount,
                      opt => opt.MapFrom(src => src.Resenas.Count)
                  )
                  .ForMember(dest => dest.Resenas, opt => opt.MapFrom(src => src.Resenas));
        }
    }
}
