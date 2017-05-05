using System.Threading.Tasks;

namespace Lumle.Data.Data.Abstracts
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
    }
}
