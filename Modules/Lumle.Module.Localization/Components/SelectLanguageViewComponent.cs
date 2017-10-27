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
            //if (_memoryCache.TryGetValue(CacheConstants.LocalizationCultureCache, out IList<EnabledCultureListVM> cultureList)) return Task.FromResult<IViewComponentResult>(View(cultureList));

            var data =  _cultureService.GetAll(x => x.IsEnable && x.IsActive)
                        .Select(c => new EnabledCultureListVM
                        {
                            Name = c.Name,
                            DisplayName = c.DisplayName
                        }).ToList();

            //var cacheOption = new MemoryCacheEntryOptions()
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            //    Priority = CacheItemPriority.High
            //};

            //_memoryCache.Set(CacheConstants.LocalizationCultureCache, data, cacheOption);

            return Task.FromResult<IViewComponentResult>(View(data));
        }
    }
}
