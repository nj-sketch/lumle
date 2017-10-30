using System;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Lumle.Data.Data
{
    public class Repository<T> : IRepository<T>
        where T : EntityBase, new()
    {
        private readonly BaseContext _context;

        public Repository(BaseContext context)
        {
            _context = context;
        }

        public virtual void Add(T entity)
        {
            try
            {
                EntityEntry dbEntityEntry = _context.Entry(entity);
                _context.Set<T>().Add(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                var query = _context.Set<T>().AsNoTracking();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual int Count()
        {
            try
            {
                return _context.Set<T>().AsNoTracking().Count();
            }
            catch (Exception)
            {
                throw;

            }
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _context.Set<T>().AsNoTracking().Where(predicate).Count();
            }
            catch (Exception)
            {
                throw;

            }
        }

        public IQueryable<T> GetAll()
        {
            try
            {
                return _context.Set<T>().AsNoTracking().AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _context.Set<T>().AsNoTracking().Where(predicate).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetSingle(int id)
        {
            try
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.AsNoTracking().Where(predicate).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void Update(T entity)
        {
            try
            {
                EntityEntry dbEntityEntry = _context.Entry(entity);
                dbEntityEntry.State = EntityState.Modified;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void Delete(T entity)
        {
            try
            {
                EntityEntry dbEntityEntry = _context.Entry<T>(entity);
                dbEntityEntry.State = EntityState.Deleted;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var entites = _context.Set<T>().Where(predicate);
                foreach (var entity in entites)
                {
                    _context.Entry<T>(entity).State = EntityState.Deleted;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _context.Set<T>().AsNoTracking().Where(predicate).AsQueryable();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IQueryable<T> GetAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.AsNoTracking().Where(predicate).AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<IQueryable<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        //{
        //    try
        //    {
        //        var query = _context.Set<T>().AsNoTracking();
        //        foreach (var includeProperty in includeProperties)
        //        {
        //            query = query.Include(includeProperty);
        //        }

        //        return await query.AsQueryable;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task Add(T entity)
        //{
        //    try
        //    {
        //        EntityEntry dbEntityEntry = _context.Entry(entity);
        //        _context.Set<T>().Add(entity);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task<T> GetSingle(Expression<Func<T, bool>> predicate)
        //{
        //    try
        //    {
        //        return await _context.Set<T>().AsNoTracking().Where(predicate).FirstOrDefaultAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task<T> GetSingle(int id)
        //{
        //    try
        //    {
        //        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task<T> GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        //{
        //    try
        //    {
        //        IQueryable<T> query = _context.Set<T>();
        //        foreach (var includeProperty in includeProperties)
        //        {
        //            query = query.Include(includeProperty);
        //        }

        //        return await query.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task Update(T entity)
        //{
        //    try
        //    {
        //        EntityEntry dbEntityEntry = _context.Entry(entity);
        //        dbEntityEntry.State = EntityState.Modified;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task Delete(T entity)
        //{
        //    try
        //    {
        //        EntityEntry dbEntityEntry = _context.Entry<T>(entity);
        //        dbEntityEntry.State = EntityState.Deleted;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public async Task DeleteWhere(Expression<Func<T, bool>> predicate)
        //{
        //    try
        //    {
        //        var entites = _context.Set<T>().Where(predicate);
        //        foreach (var entity in entites)
        //        {
        //            _context.Entry<T>(entity).State = EntityState.Deleted;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
