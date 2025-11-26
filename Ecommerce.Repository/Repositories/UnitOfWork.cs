using Ecommerce.Models;
using Ecommerce.Persistence;
using Ecommerce.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        // 1. El único DbContext
        private readonly ApplicationDbContext _context;

        // 2. Variables privadas para los repositorios
        // Usamos 'lazy loading' simple para instanciarlos solo cuando se piden
        private IRepositoryBase<Producto>? _productoRepo;
        private IRepositoryBase<Categoria>? _categoriaRepo;
        private IRepositoryBase<Pedido>? _pedidoRepo;
        private IRepositoryBase<Carrito>? _carritoRepo;
        private IRepositoryBase<Resena>? _resenaRepo;
        private IRepositoryBase<ItemCarrito> _itemCarritoRepo;
        private IRepositoryBase<Direccion> _direccionRepo;
        private IRepositoryBase<DetallePedido> _detallePedidoRepo;
        private IRepositoryBase<ProductoCategoria> _productoCategoriaRepo; // <-- NUEVO CAMPO




        // 3. Inyecta el DbContext
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // 4. Implementación de las propiedades de la interfaz
        // Cuando el Service pida el repositorio, se crea si no existe.
        // Todos usan el MISMO DbContext.
        public IRepositoryBase<ItemCarrito> ItemCarritoRepository
        {
            get
            {
                _itemCarritoRepo ??= new RepositoryBase<ItemCarrito>(_context);
                return _itemCarritoRepo;
            }
        }
        public IRepositoryBase<Producto> ProductoRepository
        {
            get
            {
                _productoRepo ??= new RepositoryBase<Producto>(_context);
                return _productoRepo;
            }
        }

        public IRepositoryBase<Categoria> CategoriaRepository
        {
            get
            {
                _categoriaRepo ??= new RepositoryBase<Categoria>(_context);
                return _categoriaRepo;
            }
        }

        public IRepositoryBase<Pedido> PedidoRepository
        {
            get
            {
                _pedidoRepo ??= new RepositoryBase<Pedido>(_context);
                return _pedidoRepo;
            }
        }

        public IRepositoryBase<Carrito> CarritoRepository
        {
            get
            {
                _carritoRepo ??= new RepositoryBase<Carrito>(_context);
                return _carritoRepo;
            }
        }
        public IRepositoryBase<Resena> ResenaRepository
        {
            get
            {
                _resenaRepo ??= new RepositoryBase<Resena>(_context);
                return _resenaRepo;
            }
        }
        public IRepositoryBase<Direccion> DireccionRepository
        {
            get
            {
                _direccionRepo ??= new RepositoryBase<Direccion>(_context);
                return _direccionRepo;
            }
        }
        public IRepositoryBase<DetallePedido> DetallePedidoRepository
        {
            get
            {
                _detallePedidoRepo ??= new RepositoryBase<DetallePedido>(_context);
                return _detallePedidoRepo;
            }
        }
        public IRepositoryBase<ProductoCategoria> ProductoCategoriaRepository
        {
            get
            {
                _productoCategoriaRepo ??= new RepositoryBase<ProductoCategoria>(_context);
                return _productoCategoriaRepo;
            }
        }
        // ... Implementa los demás repositorios de la misma forma ...

        // 5. El método central de guardado
        public Task<int> SaveAsync()
        {
            // Llama al SaveChanges del DbContext
            return _context.SaveChangesAsync();
        }

        // 6. Implementación de IDisposable (para limpiar el DbContext)
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Libera el DbContext
                    _context.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
