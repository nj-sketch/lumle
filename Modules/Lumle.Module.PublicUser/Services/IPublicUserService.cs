using Lumle.Data.Models;
using Lumle.Module.PublicUser.Helpers;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;
using System.Threading.Tasks;
using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.PublicUser.Services
{
    public interface IPublicUserService
    {
        Task Update(PublicUserEditVM model, User loggedUser);
        DTResult<PublicUserIndexVM> GetDataTableResult(User loggedUser, PublicUserDTParamaters parameters);
    }
}
