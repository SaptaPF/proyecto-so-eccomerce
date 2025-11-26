using Ecommerce.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface IProductoService
    {
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoDto>> GetAllAsync();
        Task<IEnumerable<ProductoDto>> GetByCategoriaIdAsync(int categoriaId);
        Task AddResenaAsync(string usuarioId, int productoId, int rating, string comentario);
    }
}
