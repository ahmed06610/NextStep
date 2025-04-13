using Microsoft.EntityFrameworkCore;
using NextStep.Core.Interfaces;
using NextStep.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> FindTWithIncludes<T>(int id, string keyname, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(e => Microsoft.EntityFrameworkCore.EF.Property<int>(e, keyname) == id);
        }

        public async Task<T> FindTWithQueryIncludes<T>(int id, string keyname, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                query = includes(query);
            }
            return await query.FirstOrDefaultAsync(e => Microsoft.EntityFrameworkCore.EF.Property<int>(e, keyname) == id);
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllWithIncludes<T>(Expression<Func<T, bool>>? criteria, params Expression<Func<T, object>>[]? includeProperties) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllWithQueryIncludes<T>(Expression<Func<T, bool>>? criteria = null, params Func<IQueryable<T>, IQueryable<T>>[] includes) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            foreach (var include in includes)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public async Task<T> FindTWithExpression<T>(params Expression<Func<T, bool>>[] expressions) where T : class
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var expression in expressions)
            {
                query = query.Where(expression);
            }
            return await query.FirstOrDefaultAsync();
        }
        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? criteria)
        {
            if (criteria == null)
            {
                return _context.Set<T>();
            }
            return _context.Set<T>().Where(criteria);
        }

        // Add appropriate indexes on database tables

    }
}
