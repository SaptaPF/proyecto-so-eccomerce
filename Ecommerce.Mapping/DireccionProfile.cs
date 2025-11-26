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
    public class DireccionProfile : Profile
    {
        public DireccionProfile()
        {
            // Mapeo simple porque las propiedades se llaman igual
            CreateMap<Direccion, DireccionDto>();

            // Mapeo en la otra dirección (para cuando el usuario crea una)
            CreateMap<DireccionDto, Direccion>();
        }
    }
}
