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
    public class CarritoProfile : Profile
    {
        public CarritoProfile()
        {
            // Mapea Carrito a CarritoDto
            CreateMap<Carrito, CarritoDto>()
                // Ignoramos TotalGeneral, ya que lo calcularemos 
                // manualmente en el servicio.
                .ForMember(dest => dest.TotalGeneral, opt => opt.Ignore());
        }
    }
}
