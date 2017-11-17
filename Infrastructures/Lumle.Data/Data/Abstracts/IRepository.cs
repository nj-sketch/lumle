using Microsoft.EntityFrameworkCore.Storage;
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

        T GetSingle(int id);

        Task<T> GetSingleAsync(int id);

        T GetSingle(Expression<Func<T, bool>> predicate);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        T Add(T entity);

        Task<T> AddAsync(T entity);

        T Update(T entity, object key);

        Task<T> UpdateAsync(T entity, object key);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        void SaveChanges();

        Task SaveChangesAsync();

        IDbContextTransaction BeginTransaction();
    }
}
