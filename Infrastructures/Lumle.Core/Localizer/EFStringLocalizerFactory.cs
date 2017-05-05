using System;
using System.Collections.Generic;
using System.Linq;
using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Constants.Cache;

namespace Lumle.Core.Localizer
{
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
       
        private readonly IMemoryCache _memoryCache;
        private readonly IRepository<Resource> _resourceRepository;
        private IList<ResourceString> _resourceStrings;

        public EFStringLocalizerFactory(IRepository<Resource> resourceRepository, IMemoryCache memoryCache)
        {
            _resourceRepository = resourceRepository;
            _memoryCache = memoryCache;
            LoadResource();
        }
        public IStringLocalizer Create(Type resourceSource)
        {
            LoadResource();
            return new EFStringLocalizer(_resourceStrings);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            LoadResource();
            return new EFStringLocalizer(_resourceStrings);
        }

        private void LoadResource() 
        {
            
            if (_memoryCache.TryGetValue(CacheConstants.LocalizationResourceCache, out _resourceStrings)) return;
           
            _resourceStrings = _resourceRepository.AllIncluding(x => x.Culture).Where(x => !string.IsNullOrEmpty(x.Value)&& x.Culture.Name=="en-US").Select(x => new ResourceString
            {
                Culture = x.Culture.Name,
                Key = x.Key,
                Value = x.Value
            }).ToList();

            var cacheOption = new MemoryCacheEntryOptions()
            {
                Priority=CacheItemPriority.NeverRemove
            };

            _memoryCache.Set(CacheConstants.LocalizationResourceCache, _resourceStrings, cacheOption);
        }
    }
}
