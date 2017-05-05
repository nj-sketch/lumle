using Lumle.AuthServer.Data.Entities;

namespace Lumle.AuthServer.Data.Store
{
    public interface IUserStore
    {
        bool ValidateCredentials(string username, string password);

        CustomUser FindBySubjectId(string subjectId);

        CustomUser FindByProviderAndSubjectId(string provider, string subjectId);

        CustomUser FindByUsername(string username);

        CustomUser FindByEmail(string email);

        CustomUser FindByProviderAndEmail(string provider, string email);

        bool IsUserAvailable(string provider, string subId);

        bool IsEmailExist(string email);

        void AddNewUser(CustomUser entity);
    }
}
