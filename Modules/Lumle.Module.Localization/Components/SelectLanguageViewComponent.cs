using Lumle.Module.Localization.Services;
using Lumle.Module.Localization.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.Localization.Components
{
    public class SelectLanguageViewComponent : ViewComponent
    {
        private readonly ICultureService _cultureService;
        private readonly IMemoryCache _memoryCache;

        public SelectLanguageViewComponent
        (
            ICultureService cultureService,
            IMemoryCache memoryCache
        )
        {
            _cultureService = cultureService;
            _memoryCache = memoryCache;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {         
            var data =  _cultureService.GetAll(x => x.IsEnable && x.IsActive)
                        .Select(c => new EnabledCultureListVM
                        {
                            Name = c.Name,
                            DisplayName = c.DisplayName
                        }).ToList();

            return Task.FromResult<IViewComponentResult>(View(data));
        }
    }
}
