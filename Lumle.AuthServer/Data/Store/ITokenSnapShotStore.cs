using System.Threading.Tasks;

namespace Lumle.AuthServer.Data.Store
{
    public interface ITokenSnapShotStore
    {

        Task UpdateUserStatusAsync(string subjectId);
        Task UpdateUserStatusAsync(string subjectId, string jwtId);

    }
}
