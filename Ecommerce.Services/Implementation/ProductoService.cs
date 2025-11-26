using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services.Implementation
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork; // De Repository
        private readonly IMapper _mapper;         // De AutoMapper

        // Inyección de dependencias en el constructor
        public ProductoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            // 1. Obtener la entidad del repositorio
            // ¡¡CRUCIAL!! Debemos incluir las propiedades de navegación
            // que el Mapeo (ProductoProfile) necesita para funcionar.
            var producto = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(
                p => p.ProductoId == id,
                includeProperties: "ProductoCategorias.Categoria" // M-M a Categoría
            );

            if (producto == null)
                return null;

            // 2. Mapear la Entidad a un DTO
            return _mapper.Map<ProductoDto>(producto);
        }

        public async Task<IEnumerable<ProductoDto>> GetAllAsync()
        {
            // 1. Obtener entidades (incluyendo datos para el mapeo)
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

            // 2. Mapear
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
        public async Task AddResenaAsync(string usuarioId, int productoId, int rating, string comentario)
        {
            // 1. Verificar si el usuario ya opinó sobre este producto (Opcional: evitar spam)
            var existe = await _unitOfWork.ResenaRepository.GetFirstOrDefaultAsync(
                r => r.UsuarioId == usuarioId && r.ProductoId == productoId
            );

            if (existe != null)
            {
                throw new InvalidOperationException("Ya has valorado este producto anteriormente.");
            }

            // 2. Crear la entidad
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
