using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> GetByIdAsync(int id);
        T Update(T entity);
        Task DeleteAsync(T entity);

        Task<T> FindTWithIncludes<T>(int id, string keyname, params Expression<Func<T, object>>[] includeProperties) where T : class;
        Task<T> FindTWithQueryIncludes<T>(int id, string keyname, Func<IQueryable<T>, IQueryable<T>> includes = null) where T : class;
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria);
        public Task<IEnumerable<T>> FindAllWithIncludes<T>(Expression<Func<T, bool>>? criteria, params Expression<Func<T, object>>[]? includeProperties) where T : class;
        Task<IEnumerable<T>> FindAllWithQueryIncludes<T>(Expression<Func<T, bool>>? criteria = null, params Func<IQueryable<T>, IQueryable<T>>[] includes) where T : class;

        public Task<T> FindTWithExpression<T>(params Expression<Func<T, bool>>[] expressions) where T : class;
        public IQueryable<T> GetQueryable(Expression<Func<T, bool>>? criteria);


    }

}
