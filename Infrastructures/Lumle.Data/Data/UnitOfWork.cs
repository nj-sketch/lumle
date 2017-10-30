using System;
using System.Threading.Tasks;
using Lumle.Data.Data.Abstracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lumle.Data.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BaseContext _baseContext;
        private IDbContextTransaction _transaction;

        public UnitOfWork(BaseContext baseContext)
        {
            _baseContext = baseContext;
        }

        public void Save()
        {
            try
            {
                _transaction = _baseContext.Database.BeginTransaction();
                _baseContext.SaveChanges();
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                _transaction = _baseContext.Database.BeginTransaction();
                await _baseContext.SaveChangesAsync();
                _transaction.Commit();
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
            }
        }
    }
}
