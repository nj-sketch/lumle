using Lumle.Data.Models;
using Lumle.Module.Audit.Helpers;
using Lumle.Module.Audit.ViewModels;
using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.Audit.Services
{
    public interface ICustomLogService
    {
        DTResult<CustomLogVM> GetDataTableResult(User loggedUser, CustomLogDTParameter parameters);
    }
}
