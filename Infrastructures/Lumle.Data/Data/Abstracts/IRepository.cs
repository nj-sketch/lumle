using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Data.Data.Abstracts
{
    public interface IRepository<T> where T : class
    {
        int Count();

        int Count(Expression<Func<T, bool>> predicate);

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        Task<T> GetSingle(int id);

        Task<T> GetSingle(Expression<Func<T, bool>> predicate);

        Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task Add(T entity);

        Task Update(T entity);

        Task Delete(T entity);

        Task DeleteWhere(Expression<Func<T, bool>> predicate);
    }
}
