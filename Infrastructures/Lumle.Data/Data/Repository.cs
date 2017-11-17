using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lumle.Data.Data
{
    public class Repository<T> : IRepository<T> where T : EntityBase, new()
    {

        public Repository(BaseContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        protected DbContext Context { get; }
        protected DbSet<T> DbSet { get; }

        public T Add(T entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            try
            {
                DbSet.Add(entity);
                await Context.SaveChangesAsync();
                return entity;
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
                var query = DbSet.AsNoTracking();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<T> GetAllIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = DbSet;
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.AsNoTracking().Where(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Count()
        {
            try
            {
                return DbSet.AsNoTracking().Count();
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
                return DbSet.AsNoTracking().Where(predicate).Count();
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
                return DbSet.AsNoTracking().AsQueryable();
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
                return DbSet.AsNoTracking().Where(predicate).AsQueryable();
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
                return DbSet.AsNoTracking().FirstOrDefault(x => x.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetSingleAsync(int id)
        {
            try
            {
                return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
                return DbSet.AsNoTracking().SingleOrDefault(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await DbSet.AsNoTracking().SingleOrDefaultAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            try
            {
                IQueryable<T> query = DbSet;
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return await query.AsNoTracking().SingleOrDefaultAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T Update(T entity, object key)
        {
            if (entity == null)
                return null;
            T exist = DbSet.Find(key);
            if (exist != null)
            {
                Context.Entry(exist).CurrentValues.SetValues(entity);
                Context.SaveChanges();
            }

            return exist;
        }

        public async Task<T> UpdateAsync(T entity, object key)
        {
            if (entity == null)
                return null;
            T exist = await DbSet.FindAsync(key);
            if (exist != null)
            {
                Context.Entry(exist).CurrentValues.SetValues(entity);
                await Context.SaveChangesAsync();
            }
            return exist;
        }


        public IDbContextTransaction BeginTransaction()
        {
            try
            {
                return Context.Database.BeginTransaction();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                DbSet.Remove(entity);
                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var entites = DbSet.Where(predicate);
                foreach (var entity in entites)
                {
                    DbSet.Remove(entity);
                }

                Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
