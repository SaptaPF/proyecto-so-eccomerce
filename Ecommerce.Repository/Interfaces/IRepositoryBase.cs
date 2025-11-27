using System.Linq.Expressions;

namespace Ecommerce.Repository.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
      
        Task<T?> GetByIdAsync(int id); 
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,string? includeProperties = null
        );
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}