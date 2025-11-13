using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryBase<Producto> ProductoRepository { get; }
        IRepositoryBase<Categoria> CategoriaRepository { get; }
        IRepositoryBase<Pedido> PedidoRepository { get; }
        IRepositoryBase<Carrito> CarritoRepository { get; }
        IRepositoryBase<Resena> ResenaRepository { get; }
        IRepositoryBase<ItemCarrito> ItemCarritoRepository { get; }
        Task<int> SaveAsync();
    }
}
