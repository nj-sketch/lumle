using Lumle.AuthServer.Data.Entities;
using System.Threading.Tasks;

namespace Lumle.AuthServer.Data.Store
{
    public interface IUserStore
    {
        bool ValidateCredentials(string username, string password);

        MobileUser FindBySubjectId(string subjectId);
        Task<MobileUser> FindBySubjectIdAsync(string subjectId);

        MobileUser FindByProviderAndSubjectId(string provider, string subjectId);

        MobileUser FindByUsername(string username);

        MobileUser FindByEmail(string email);

        MobileUser FindByProviderAndEmail(string provider, string email);

        bool IsUserAvailable(string provider, string subId);

        bool IsEmailExist(string email);

        void AddNewUser(MobileUser entity);
    }
}
