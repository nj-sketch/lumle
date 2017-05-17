using Lumle.Api.Data.Abstracts;
using Lumle.Api.Data.Contexts;
using System.Threading.Tasks;

namespace Lumle.Api.Data.Repositiries
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BaseContext _context;

        public UnitOfWork(BaseContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
