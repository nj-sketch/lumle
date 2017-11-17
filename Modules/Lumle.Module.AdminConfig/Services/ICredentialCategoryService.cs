using Lumle.Module.AdminConfig.Models;
using System.Linq;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ICredentialCategoryService
    {
        IQueryable<CredentialCategoryModel> GetAllCredentialCategory();
    }
}
