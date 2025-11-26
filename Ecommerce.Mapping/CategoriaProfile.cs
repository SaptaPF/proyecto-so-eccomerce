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
            // Mapea la entidad Categoria al CategoriaDto
            CreateMap<Categoria, CategoriaDto>()
                // CategoriaId y Nombre se mapean automáticamente

                // Mapeo personalizado para la cantidad de productos
                .ForMember(
                    dest => dest.CantidadProductos,
                    opt => opt.MapFrom(src =>
                        // Asume que la entidad Categoria tiene la colección ProductoCategorias
                        // Cuenta cuántos productos están asociados a esta categoría
                        src.ProductoCategorias != null ? src.ProductoCategorias.Count : 0
                    )
                );
        }
    }
}
