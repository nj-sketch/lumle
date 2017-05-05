using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Lumle.AuthServer.Data.Store;

namespace Lumle.AuthServer.Infrastructures.GrantTypes
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserStore _userStore;

        public CustomResourceOwnerPasswordValidator(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {

            //Use username as Email
            if (_userStore.ValidateCredentials(context.UserName, context.Password))
            {
                var user = _userStore.FindByEmail(context.UserName);
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            }

            return Task.FromResult(0);
        }
    }
}
