using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Localization.Models;
using NLog;
using System.Threading.Tasks;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;
using Lumle.Data.Models;
using Lumle.Module.Audit.Services;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.Localization.Helpers;
using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.Localization.Services
{
    public class ResourceService : IResourceService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<Culture> _cultureRespository;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IAuditLogService _auditLogService;

        public ResourceService
        (
            IRepository<Resource> resourceRepository,
            IRepository<Culture> cultureRespository,
            IAuditLogService auditLogService
        )
        {
            _resourceRepository = resourceRepository;
            _cultureRespository = cultureRespository;
            _auditLogService = auditLogService;
        }

        public async Task<Resource> AddAsync(Resource entity)
        {
            try
            {
                return await _resourceRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public IEnumerable<ResourceModel> GetAllResource(string culture)
        {
            try
            {
                var cultureEntity = _cultureRespository.GetSingle(x => x.Name == culture.Trim());
                if (cultureEntity == null) return null;

                var resource = (from e in _resourceRepository.GetAll()
                                group e by e.Key into f
                                select new ResourceModel
                                {
                                    CultureId = cultureEntity.Id,
                                    ResourceCategoryId = f.Select(x => x.ResourceCategoryId).FirstOrDefault(),
                                    Key = f.Key,
                                    Value = f.FirstOrDefault(x => x.CultureId == cultureEntity.Id).Value ?? ""
                                });

                return resource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IEnumerable<ResourceModel> GetAllResource(int resourceCategoryId, string culture)
        {
            try
            {
                var cultureEntity = _cultureRespository.GetSingle(x => x.Name == culture.Trim());
                if (cultureEntity == null) return null;

                var resource = (from e in _resourceRepository.GetAll()
                                where e.ResourceCategoryId == resourceCategoryId
                                group e by e.Key into f
                                select new ResourceModel
                                {
                                    CultureId = cultureEntity.Id,
                                    ResourceCategoryId = f.Select(x => x.ResourceCategoryId).FirstOrDefault(),
                                    Key = f.Key,
                                    Value = f.FirstOrDefault(x => x.CultureId == cultureEntity.Id).Value ?? ""
                                });
                return resource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<Resource> GetSingle(Expression<Func<Resource, bool>> predicate)
        {
            try
            {
                return await _resourceRepository.GetSingleAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task UpdateAsync(Resource entity)
        {
            try
            {
                await _resourceRepository.UpdateAsync(entity, entity.Id);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }

        public bool IsCultureContainKey(string culture, int resourceCategoryId, string key)
        {
            try
            {
                var cultureContainKey = _resourceRepository.GetSingle(x => x.Culture.Name.ToLower() == culture.Trim().ToLower() && x.Key.ToLower() == key.Trim().ToLower() && x.ResourceCategoryId == resourceCategoryId);
                if (cultureContainKey != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task CreateResource(ResourceModel model, User loggedUser)
        {
            try
            {
                using (var transaction = _resourceRepository.BeginTransaction())
                {
                    var entity = await GetSingle(x => x.CultureId == model.CultureId && x.ResourceCategoryId == model.ResourceCategoryId && x.Key.Trim() == model.Key.Trim());

                    if (entity != null)
                    {
                        if (entity.Value.Trim() == model.Value.Trim())
                            return;

                        // Get old entity data
                        var oldRecord = new Resource
                        {
                            Id = entity.Id,
                            CultureId = entity.CultureId,
                            Key = entity.Key,
                            Value = entity.Value
                        };

                        // update in the database
                        entity.Value = model.Value.Trim();
                        await UpdateAsync(entity);

                        #region Resource Audit Log

                        var auditLogModel = new AuditLogModel
                        {
                            AuditActionType = AuditActionType.Update,
                            KeyField = oldRecord.Id.ToString(),
                            OldObject = oldRecord,
                            NewObject = entity,
                            LoggedUserEmail = loggedUser.Email,
                            ComparisonType = ComparisonType.ObjectCompare
                        };
                        await _auditLogService.Create(auditLogModel);

                        #endregion
                        await _resourceRepository.SaveChangesAsync();
                    }
                    var isCultureContainKey = IsCultureContainKey(DefaultCultureConstants.DefaultCultureName, model.ResourceCategoryId, model.Key);
                    if (isCultureContainKey)
                    {
                        var newResource = new Resource
                        {
                            CultureId = model.CultureId,
                            ResourceCategoryId = model.ResourceCategoryId,
                            Key = model.Key.Trim(),
                            Value = model.Value.Trim(),
                            CreatedDate = DateTime.UtcNow,
                            LastUpdated = DateTime.UtcNow
                        };
                        await _resourceRepository.AddAsync(newResource);
                        await _resourceRepository.SaveChangesAsync();

                        #region Resource Create AuditLog
                        var oldResource = new Resource(); // dummy resource
                        var resourceAuditLogModel = new AuditLogModel
                        {
                            AuditActionType = AuditActionType.Create,
                            KeyField = newResource.Id.ToString(),
                            OldObject = oldResource,
                            NewObject = newResource,
                            LoggedUserEmail = loggedUser.Email,
                            ComparisonType = ComparisonType.ObjectCompare
                        };
                        await _auditLogService.Create(resourceAuditLogModel);
                        await _resourceRepository.SaveChangesAsync();
                    }
                    #endregion
                    transaction.Commit();
                }
               
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public DataTableHelper.DTResult<ResourceModel> GetDataTableResult(CultureDTParameters parameters)
        {
            try
            {
                IEnumerable<ResourceModel> resources;
                switch (parameters.ResourceCategoryId)
                {
                    case 1:// Get only label
                        resources = GetAllResource(1, parameters.Culture.Trim());
                        break;
                    case 2:// get only message
                        resources = GetAllResource(2, parameters.Culture.Trim());
                        break;
                    default:
                        resources = GetAllResource(parameters.Culture.Trim());
                        break;
                }
                List<ResourceModel> filteredResource;

                int totalResource;

                var enumerable = resources as ResourceModel[] ?? resources.ToArray();
                var resourceModels = resources as ResourceModel[] ?? enumerable.ToArray();
                if (!string.IsNullOrEmpty(parameters.Search.Value)
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value))

                {
                    Expression<Func<ResourceModel, bool>> search =
                        x => (x.Key ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Value ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());

                    totalResource = enumerable.AsQueryable().Count(search);
                    resources = resourceModels.AsQueryable().Where(search);
                    resources = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), resources);
                    filteredResource = resources.Skip(parameters.Start).Take(parameters.Length).ToList();

                }
                else
                {
                    totalResource = resourceModels.Count();
                    resources = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), resourceModels);
                    filteredResource = resources.Skip(parameters.Start).Take(parameters.Length).ToList();
                }

                var i = parameters.Start + 1;
                var models = filteredResource.Select(x => { x.SN = i++; return x; }).ToList();

                var datatableResult = new DTResult<ResourceModel>
                {
                    Draw = parameters.Draw,
                    Data = models,
                    RecordsFiltered = totalResource,
                    RecordsTotal = totalResource
                };

                return datatableResult;
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        /// <summary>
        /// Serverside dataTable Sorting
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDirection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IEnumerable<ResourceModel> SortByColumnWithOrder(int order, string orderDirection, IEnumerable<ResourceModel> data)
        {
            var sortByColumnWithOrder = data as ResourceModel[] ?? data.ToArray();
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? sortByColumnWithOrder.OrderByDescending(p => p.Key) : sortByColumnWithOrder.OrderBy(p => p.Key);

                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? sortByColumnWithOrder.OrderByDescending(p => p.Value) : sortByColumnWithOrder.OrderBy(p => p.Value);

                    default:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? sortByColumnWithOrder.OrderByDescending(p => p.Key) : sortByColumnWithOrder.OrderBy(p => p.Key);
                }
            }
            catch (Exception)
            {
                return sortByColumnWithOrder;
            }
        }

        
    }
}
