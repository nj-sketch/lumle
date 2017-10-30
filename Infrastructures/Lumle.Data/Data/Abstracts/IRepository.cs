using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Data.Data.Abstracts
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        int Count();

        int Count(Expression<Func<T, bool>> predicate);

        T GetSingle(int id);

        T GetSingle(Expression<Func<T, bool>> predicate);

        T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        //Task<IQueryable<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        //Task<IQueryable<T>> GetAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        //Task<IQueryable<T>> GetAll();

        //Task<IQueryable<T>> GetAll(Expression<Func<T, bool>> predicate);

        //Task<T> GetSingle(int id);

        //Task<T> GetSingle(Expression<Func<T, bool>> predicate);

        //Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        //Task Add(T entity);

        //Task Update(T entity);

        //Task Delete(T entity);

        //Task DeleteWhere(Expression<Func<T, bool>> predicate);
    }
}
