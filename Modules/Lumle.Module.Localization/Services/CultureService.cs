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
using Lumle.Module.Localization.ViewModels;
using System.Threading.Tasks;
using Lumle.Data.Models;
using Lumle.Module.Audit.Services;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;

namespace Lumle.Module.Localization.Services
{
    public class CultureService : ICultureService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<Culture> _cultureRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuditLogService _auditLogService;

        public CultureService
        (
            IRepository<Culture> cultureRepository,
            IMemoryCache memoryCache,
            IAuditLogService auditLogService
        )
        {
            _cultureRepository = cultureRepository;
            _memoryCache = memoryCache;
            _auditLogService = auditLogService;
        }

        public async Task CreateResource(User loggedUser, Culture selectedCulture)
        {
            try
            {
                selectedCulture.IsEnable = true;
                selectedCulture.IsActive = false;

                await _cultureRepository.UpdateAsync(selectedCulture, selectedCulture.Id);

                #region Culture Audit Log
                var oldCulture = new Culture(); // Storage of this null object shows data before creation = nothing!
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Create,
                    KeyField = selectedCulture.Id.ToString(),
                    OldObject = oldCulture,
                    NewObject = selectedCulture,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };
                #endregion
                await _auditLogService.Create(auditLogModel);
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public IQueryable<Culture> GetAll(Expression<Func<Culture, bool>> predicate)
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

        public CultureSelectListVM GetAllActiveCultures()
        {
            try
            {
                if (_memoryCache.TryGetValue(CacheConstants.LocalizationCultureCache, out CultureSelectListVM cultureList)) return cultureList;

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

                return new CultureSelectListVM { CulturesList = data };
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

        public async Task<Culture> GetSingle(Expression<Func<Culture, bool>> predicate)
        {
            try
            {
                return await _cultureRepository.GetSingleAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Update(Culture entity)
        {
            try
            {
                await _cultureRepository.UpdateAsync(entity, entity.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
