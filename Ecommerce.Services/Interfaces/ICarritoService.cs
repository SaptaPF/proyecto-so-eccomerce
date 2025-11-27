using Ecommerce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface ICarritoService
    {

        Task<CarritoDto> GetCarritoDtoAsync(string usuarioId);
        Task<int> GetItemCountAsync(string usuarioId);
        Task<CarritoDto> AddItemAsync(string usuarioId, int productoId, int cantidad);
        Task<CarritoDto> RemoveItemAsync(string usuarioId, int itemCarritoId);
    }
}