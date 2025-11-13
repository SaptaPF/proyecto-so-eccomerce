using Ecommerce.Persistence;
using Ecommerce.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Repository.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        // El DbContext que viene de Persistence
        protected readonly ApplicationDbContext _context;
        // El DbSet genérico para la entidad T
        internal DbSet<T> _dbSet;

        // Inyección de dependencias: Pedimos el DbContext
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // _dbSet será _context.Productos, _context.Categorias, etc.
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            // Adjunta la entidad al contexto y la marca como modificada
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            // Si la entidad no está siendo rastreada, primero la adjunta
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            // 1. Aplicar filtro
            query = query.Where(filter);

            // 2. Aplicar Includes
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            // 1. Aplicar filtro (si existe)
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // 2. Aplicar Includes (si existen)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            // 3. Aplicar Ordenamiento (si existe)
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }
    }
}