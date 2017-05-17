using System.Threading.Tasks;

namespace Lumle.Api.Data.Abstracts
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
    }
}
