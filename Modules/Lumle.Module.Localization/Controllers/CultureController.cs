using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Services;
using Lumle.Module.Localization.Helpers;
using Lumle.Module.Localization.Models;
using Lumle.Module.Localization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Microsoft.Extensions.Localization;
using Lumle.Module.Localization.ViewModels;
using Lumle.Infrastructure.Constants.Localization;
using OfficeOpenXml;
using System.IO;
using Lumle.Infrastructure.Constants.Cache;
using Lumle.Core.Services.Abstracts;
using System.Security.Claims;
using Lumle.Infrastructure.Constants.ActionConstants;

namespace Lumle.Module.Localization.Controllers
{
    [Route("localization/[Controller]")]
    [Authorize]
    public class CultureController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly ICultureService _cultureService;
        private readonly IResourceService _resourceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuditLogService _auditLogService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;
        private IList<ResourceString> _resourceStrings;
        private readonly IRepository<Resource> _resourceRepository;

        public CultureController(
            IBaseRoleClaimService baseRoleClaimService,
            ICultureService cultureService,
            IResourceService resourceService,
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache,
            IAuditLogService auditLogService,
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer,
            IRepository<Resource> resourceRepository
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _userManager = userManager;
            _cultureService = cultureService;
            _resourceService = resourceService;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _auditLogService = auditLogService;
            _localizer = localizer;
            _resourceRepository = resourceRepository;
        }

        [HttpPost("setlanguage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(culture) && string.IsNullOrWhiteSpace(culture)) return LocalRedirect(returnUrl);

                var supportedCulture = _cultureService.GetAll(x => x.Name == culture.Trim());
                if (supportedCulture == null) return LocalRedirect(returnUrl);

                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
                    );

                var user = await _userManager.GetUserAsync(HttpContext.User);
                user.Culture = culture.Trim();
                await _userManager.UpdateAsync(user);

                ReloadLocalizationResourceCache(culture.Trim());

                return LocalRedirect(returnUrl);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.Undefinederror);
                return LocalRedirect(returnUrl);
            }

        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureView)]
        public async Task<IActionResult> Index()
        {
            #region action-privelege
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.CreateAction, new Claim("permission", Permissions.LocalizationCultureCreate) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
            #endregion

            var cultures = _cultureService.GetAllCulture();

            // Get inactive cultures
            var inActiveCultures = cultures.Where(x => !x.IsEnable).ToList();

            // Get enabled cultures
            var enabledCultures = cultures.Where(x => x.IsEnable).ToList();

            var cultureListVm = new CultureListVM
            {
                EnabledCultures = enabledCultures,
                InActiveCultures = inActiveCultures,
                InActiveCultureCount = inActiveCultures.Count,
                CreateAction = actionClaimResult[OperationActionConstant.CreateAction]
            };

            return View(cultureListVm);
        }

        [HttpGet("{culture}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureView)]
        public async Task<IActionResult> Resource(string culture)
        {
            #region action-privelege
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.LocalizationCultureUpdate) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
            #endregion

            var model = new ImportResourceModel { Culture = culture };

            // Send permission edit previlege in view
            ViewBag.UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction];

            return View("Resource", model);
        }

        [HttpPost("addorupdateresource")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureUpdate)]
        public async Task<JsonResult> AddOrUpdateResource([FromBody] ResourceModel resource)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync(); //Get current logged in user

                var entity = _resourceService.GetSingle(x => x.CultureId == resource.CultureId && x.ResourceCategoryId == resource.ResourceCategoryId && x.Key.Trim() == resource.Key.Trim());

                if (entity != null)
                {
                    if (entity.Value.Trim() == resource.Value.Trim())
                        return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });

                    // Get old entity data
                    var oldRecord = new Resource
                    {
                        Id = entity.Id,
                        CultureId = entity.CultureId,
                        Key = entity.Key,
                        Value = entity.Value
                    };

                    // update in the database
                    entity.Value = resource.Value.Trim();
                    _resourceService.Update(entity);

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
                    _auditLogService.Add(auditLogModel);

                    #endregion
                    _unitOfWork.Save();

                    ReloadLocalizationResourceCache(loggedUser.Culture);

                    return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });
                }
                var isCultureContainKey = _resourceService.IsCultureContainKey(DefaultCultureConstants.DefaultCultureName, resource.ResourceCategoryId, resource.Key);
                if (isCultureContainKey)
                {
                    var newResource = new Resource
                    {
                        CultureId = resource.CultureId,
                        ResourceCategoryId = resource.ResourceCategoryId,
                        Key = resource.Key.Trim(),
                        Value = resource.Value.Trim(),
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };
                    _resourceService.Add(newResource);

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
                    _auditLogService.Add(resourceAuditLogModel);
                    _unitOfWork.Save();
                }
                #endregion

                ReloadLocalizationResourceCache(loggedUser.Culture);

                return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.AddUpdateError);
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value });
            }
        }

        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureCreate)]
        public async Task<IActionResult> AddNewCulture(string selectedCulture)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync(); //Get current logged user

                if (string.IsNullOrEmpty(selectedCulture) || string.IsNullOrWhiteSpace(selectedCulture))
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.SelectValidItemErrorMessage].Value;
                    return RedirectToAction("Index");
                }
                var culture = _cultureService.GetSingle(x => x.Name == selectedCulture.Trim() && !x.IsEnable);

                if (culture == null)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                    return RedirectToAction("Index");
                }

                culture.IsEnable = true;
                culture.IsActive = false;

                _cultureService.Update(culture);

                #region Culture Audit Log
                var oldCulture = new Culture(); // Storage of this null object shows data before creation = nothing!
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Create,
                    KeyField = culture.Id.ToString(),
                    OldObject = oldCulture,
                    NewObject = culture,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };

                _auditLogService.Add(auditLogModel);

                #endregion
                _unitOfWork.Save();

                _memoryCache.Remove(CacheConstants.LocalizationCultureCache);
                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                return RedirectToAction("Index");
            }
        }

        [HttpPost("updateCulture")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureUpdate)]
        public async Task<IActionResult> UpdateCulture([FromBody] CultureViewModel cultureViewModel)
        {
            try
            {
                if (cultureViewModel.Name == DefaultCultureConstants.DefaultCultureName) return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value });

                var culture = _cultureService.GetSingle(x => x.IsEnable && x.Name == cultureViewModel.Name.Trim());
                if (culture == null)
                {
                    return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value });
                }
                _memoryCache.Remove(CacheConstants.LocalizationCultureCache);

                culture.IsActive = cultureViewModel.IsActive;
                _cultureService.Update(culture);
                _unitOfWork.Save();

                var loggedUser = await _userManager.GetUserAsync(User);
                if (loggedUser.Culture != cultureViewModel.Name.Trim() || cultureViewModel.IsActive)
                    return Json(new
                    {
                        success = true,
                        messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value,
                        message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value
                    });
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(DefaultCultureConstants.DefaultCultureName)),
                    new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
                );

                ReloadLocalizationResourceCache(DefaultCultureConstants.DefaultCultureName);

                return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value });
            }

        }


        /// <summary>
        /// Server Side DataTable Handler for resource
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("DataHandler")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureView)]
        public JsonResult DataHandler([FromBody] CultureDTParameters parameters)
        {
            try
            {
                IEnumerable<ResourceModel> resources;
                switch (parameters.ResourceCategoryId)
                {
                    case 1:// Get only label
                        resources = _resourceService.GetAllResource(1, parameters.Culture.Trim());
                        break;
                    case 2:// get only message
                        resources = _resourceService.GetAllResource(2, parameters.Culture.Trim());
                        break;
                    default:
                        resources = _resourceService.GetAllResource(parameters.Culture.Trim());
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

                var resource = new DTResult<ResourceModel>
                {
                    Draw = parameters.Draw,
                    Data = models,
                    RecordsFiltered = totalResource,
                    RecordsTotal = totalResource
                };
                return Json(resource);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.LocalizationCultureUpdate)]
        public async Task<IActionResult> ImportExcel(ImportResourceModel model)
        {
            var loggedUser = await GetCurrentUserAsync();
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ErrorOccured].Value;
                return RedirectToAction("Resource", new { model.Culture });
            }
            var count = 0;
            if (model.files.Count > 0)
            {
                var culture = _cultureService.GetSingle(x => x.Name == model.Culture.Trim());
                if (culture == null)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                    return RedirectToAction("Resource", new { model.Culture });
                }
                var excelDataModel = new List<ResourceKeyValueHeper>();
                foreach (var file in model.files)
                {
                    if (file.Length <= 0) continue;
                    var fileStream = file.OpenReadStream();
                    var mappedDataModel = MapExcelDataToResourceKeyValueHeper(fileStream);
                    if (mappedDataModel != null)
                    {
                        excelDataModel.AddRange(mappedDataModel);
                    }
                }
                if (excelDataModel.Count > 0)
                {
                    foreach (var data in excelDataModel)
                    {
                        var entity = _resourceService.GetSingle(x => x.CultureId == culture.Id && x.ResourceCategoryId == model.ResourceCategoryId && x.Key.Trim() == data.Key.Trim());
                        if (entity != null)
                        {
                            // Get old entity data
                            var oldRecord = new Resource
                            {
                                Id = entity.Id,
                                CultureId = entity.CultureId,
                                Key = entity.Key,
                                Value = entity.Value
                            };

                            // update in the database
                            entity.Value = data.Value.Trim();
                            _resourceService.Update(entity);
                            count++;

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
                            _auditLogService.Add(auditLogModel);
                            _unitOfWork.Save();
                            #endregion 
                        }
                        else
                        {
                            var isCultureContainKey = _resourceService.IsCultureContainKey(DefaultCultureConstants.DefaultCultureName, model.ResourceCategoryId, data.Key);
                            if (isCultureContainKey)
                            {
                                var newResource = new Resource
                                {
                                    CultureId = culture.Id,
                                    ResourceCategoryId = model.ResourceCategoryId,
                                    Key = data.Key.Trim(),
                                    Value = data.Value.Trim(),
                                    CreatedDate = DateTime.UtcNow,
                                    LastUpdated = DateTime.UtcNow
                                };
                                _resourceService.Add(newResource);
                                _unitOfWork.Save();
                                count++;
                           

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
                            _auditLogService.Add(resourceAuditLogModel);
                            _unitOfWork.Save();

                                #endregion
                            }
                        }
                    }
                }
                if (count == 0)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidExcelFormatError].Value;
                    return RedirectToAction("Resource", new { model.Culture });
                }
                ReloadLocalizationResourceCache(loggedUser.Culture);
                TempData["SuccessMsg"] = _localizer[count + $" " + ActionMessageConstants.Of + $" " + excelDataModel.Count + $" " + ActionMessageConstants.DataInsertedSuccessfully].Value;
                return RedirectToAction("Resource", new { model.Culture });
            }
            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToInsertDataError].Value;
            return RedirectToAction("Resource", new { model.Culture });
        }

        #region Helpers
        private void ReloadLocalizationResourceCache(string cultureName)
        {
            _resourceStrings = _resourceRepository.AllIncluding(x => x.Culture).Where(x => !string.IsNullOrEmpty(x.Value) && x.Culture.Name == cultureName).Select(x => new ResourceString
            {
                Culture = x.Culture.Name,
                Key = x.Key,
                Value = x.Value
            }).ToList();

            var cacheOption = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.NeverRemove
            };
            _memoryCache.Remove(CacheConstants.LocalizationResourceCache);
            _memoryCache.Set(CacheConstants.LocalizationResourceCache, _resourceStrings, cacheOption);
        }

        private List<ResourceKeyValueHeper> MapExcelDataToResourceKeyValueHeper(Stream fileStream)
        {
            try
            {
                var resourceKeyValueHelperList = new List<ResourceKeyValueHeper>();
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;
                    if (colCount != 3) return null;
                    // bool bHeaderRow = true;
                    for (var row = 1; row < rowCount; row++)
                    {
                        var resourceKeyValueHelper =
                            new ResourceKeyValueHeper
                            {
                                Key = worksheet.Cells[row + 1, 2].Value.ToString(),
                                Value = worksheet.Cells[row + 1, 3].Value.ToString()
                            };


                        resourceKeyValueHelperList.Add(resourceKeyValueHelper);
                    }
                    return resourceKeyValueHelperList;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.ExcelError);
                return null;
            }
        }

        private class ResourceKeyValueHeper
        {
            public string Key { get; set; }
            public string Value { get; set; }
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

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
