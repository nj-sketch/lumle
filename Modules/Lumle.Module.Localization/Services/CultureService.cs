using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.Localization.Models;
using Lumle.Core.Models;
using Lumle.Infrastructure.Constants.LumleLog;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using Lumle.Infrastructure.Constants.Cache;

namespace Lumle.Module.Localization.Services
{
    public class CultureService : ICultureService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<Culture> _cultureRepository;
        private readonly IMemoryCache _memoryCache;

        public CultureService
        (
            IRepository<Culture> cultureRepository,
            IMemoryCache memoryCache
        )
        {
            _cultureRepository = cultureRepository;
            _memoryCache = memoryCache;
        }
        public void Add(Culture entity)
        {
            try
            {
                _cultureRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public void DeleteWhere(Expression<Func<Culture, bool>> predicate)
        {
            try
            {
                _cultureRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw;
            }
        }

        public IEnumerable<Culture> GetAll()
        {
            try
            {
                return _cultureRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IEnumerable<Culture> GetAll(Expression<Func<Culture, bool>> predicate)
        {
            try
            {
                return _cultureRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IEnumerable<CultureModel> GetAllCulture()
        {
            try
            {
                var i = 0;
                var data = (from e in _cultureRepository.GetAll()
                            select new CultureModel
                            {
                                Name = e.Name,
                                DisplayName = e.DisplayName,
                                IsEnable = e.IsEnable,
                                IsActive=e.IsActive
                            }).OrderBy(x => x.IsEnable == false).ThenBy(x => x.DisplayName).ToList();

                return data.Select(x => { x.Sn = ++i; return x; }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }


        public IEnumerable<SelectListItem> GetAllCultureSelectListItem()
        {
            try
            {
                if (_memoryCache.TryGetValue(CacheConstants.LocalizationCultureCache, out List<SelectListItem> cultureList)) return cultureList;

                var data = (from e in _cultureRepository.GetAll(x => x.IsEnable && x.IsActive)
                            select new SelectListItem
                            {
                                Value = e.Name,
                                Text = e.DisplayName
                            }).ToList();

                var cacheOption = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                    Priority = CacheItemPriority.High
                };

                _memoryCache.Set(CacheConstants.LocalizationCultureCache, data, cacheOption);

                return data;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public Culture GetSingle(Expression<Func<Culture, bool>> predicate)
        {
            try
            {
                return _cultureRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public void Update(Culture entity)
        {
            try
            {
                _cultureRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
