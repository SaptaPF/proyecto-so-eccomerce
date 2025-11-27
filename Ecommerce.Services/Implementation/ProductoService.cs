using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services.Implementation
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMapper _mapper;         

     
        public ProductoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
           
            var producto = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(
                p => p.ProductoId == id,
                includeProperties: "ProductoCategorias.Categoria" 
            );

            if (producto == null)
                return null;

            return _mapper.Map<ProductoDto>(producto);
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync()
        {
            var productos = await _unitOfWork.ProductoRepository.GetAllAsync(
                includeProperties: "ProductoCategorias.Categoria"
            );

            // 2. Mapear la lista de Entidades a una lista de DTOs
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }

        public async Task<IEnumerable<ProductoDto>> GetByCategoriaIdAsync(int categoriaId)
        {
            // 1. Obtener entidades usando un filtro LINQ
            var productos = await _unitOfWork.ProductoRepository.GetAllAsync(
                filter: p => p.ProductoCategorias.Any(pc => pc.CategoriaId == categoriaId),
                includeProperties: "ProductoCategorias.Categoria"
            );

            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
        public async Task AddResenaAsync(string usuarioId, int productoId, int rating, string comentario)
        {
            var existe = await _unitOfWork.ResenaRepository.GetFirstOrDefaultAsync(
                r => r.UsuarioId == usuarioId && r.ProductoId == productoId
            );

            if (existe != null)
            {
                throw new InvalidOperationException("Ya has valorado este producto anteriormente.");
            }

            var resena = new Resena
            {
                ProductoId = productoId,
                UsuarioId = usuarioId,
                Rating = rating,
                Comentario = comentario,
                Fecha = DateTime.UtcNow
            };

            // 3. Guardar
            await _unitOfWork.ResenaRepository.AddAsync(resena);
            await _unitOfWork.SaveAsync();
        }
    }
}
