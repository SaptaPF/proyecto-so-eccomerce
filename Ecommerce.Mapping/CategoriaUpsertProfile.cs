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
    public class CategoriaUpsertProfile : Profile
    {
        public CategoriaUpsertProfile()
        {
            // Mapeo bidireccional para el formulario
            CreateMap<Categoria, CategoriaUpsertDto>().ReverseMap();
        }
    }
}