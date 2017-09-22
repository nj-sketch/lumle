using System;
using System.Collections.Generic;
using System.Net;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.Blog.Models;
using Lumle.Module.Blog.Services;
using Lumle.Module.Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Module.Blog.Entities;
using Lumle.Module.Audit.Services;
using Lumle.Module.Audit.Enums;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Lumle.Module.Blog.Helpers;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Identity;
using Lumle.Module.Audit.Models;
using Microsoft.Extensions.Localization;
using Lumle.Core.Localizer;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Infrastructure.Utilities.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Lumle.Module.Blog.Controllers
{
    [Route("blog/[controller]")]
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;
        private readonly ITimeZoneHelper _timeZoneHelper;
        private readonly IFileHandler _fileHandler;
        private IHostingEnvironment _environment;
        private IUrlHelper _urlHelper;

        public ArticleController(
            UserManager<User> userManager,
            IArticleService articleService,
            IAuditLogService auditLogService,
            IUnitOfWork unitOfWork,
            IStringLocalizer<ResourceString> localizer,
            ITimeZoneHelper timeZoneHelper,
            IFileHandler fileHandler,
            IHostingEnvironment environment,
            IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _articleService = articleService;
            _auditLogService = auditLogService;
            _localizer = localizer;
            _timeZoneHelper = timeZoneHelper;
            _fileHandler = fileHandler;
            _environment = environment;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleView)]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet("new")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleCreate)]
        public ViewResult New()
        {
            return View();
        }

        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleCreate)]
        public async Task<IActionResult> New(ArticleVM model)
        {
            var image=   _fileHandler.UploadImage(model.FeaturedImage,300,300);

            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
  
            var loggedUser = await GetCurrentUserAsync();
            var articleModel = Mapper.Map<ArticleModel>(model);
            articleModel.Author = loggedUser.Email;
            articleModel.CreatedDate = DateTime.UtcNow;
            articleModel.LastUpdated = DateTime.UtcNow;
            articleModel.Slug = "Test slug";
            articleModel.FeaturedImageUrl = image;
            var articleEntity = Mapper.Map<Article>(articleModel);
            _articleService.Add(articleEntity);
            _unitOfWork.Save();
            #region AuditLog
            var newArticle = new Article(); // Storage of this null object shows data before creation = nothing!
            var auditLogModel = new AuditLogModel
            {
                AuditActionType = AuditActionType.Create,
                OldObject = newArticle,
                NewObject = articleEntity,
                KeyField = articleEntity.Id.ToString(),
                ComparisonType = ComparisonType.ObjectCompare,
                LoggedUserEmail = loggedUser.Email
            };
            _auditLogService.Add(auditLogModel);
            _unitOfWork.Save();
            #endregion

            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;

            return RedirectToAction("Index");
        }

        [HttpGet("edit/{articleId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleUpdate)]
        public IActionResult Edit(int articleId)
        {
            var article = _articleService.GetSingle(x => x.Id == articleId);
            var articleVm = Mapper.Map<ArticleVM>(article);
            articleVm.FeaturedImageUrl = $"{Request.Scheme}://{Request.Host}{_urlHelper.Content("~/")}uploadedimages/{articleVm.FeaturedImageUrl}";
            return View(articleVm);
        }

        [HttpPost("edit/{articleId:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleUpdate)]
        public async Task<IActionResult> Edit(ArticleVM model)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            var article = _articleService.GetSingle(x => x.Id == model.Id);
            if (article == null) return RedirectToAction("Index");

            var loggedUser = await GetCurrentUserAsync();
            // Add previous data in old record object for comparison
            var oldRecord = new Article
            {
                Id = article.Id,
                Author = article.Author,
                Title = article.Title,
                Content = article.Content,
                Slug = article.Slug
            };
            // update in database
            article.Title = model.Title;
            article.Content = model.Content;
            _articleService.Update(article);
            // For audit log
            #region AuditLog
            var auditLogModel = new AuditLogModel
            {
                AuditActionType = AuditActionType.Update,
                OldObject = oldRecord,
                NewObject = article,
                KeyField = oldRecord.Id.ToString(),
                ComparisonType = ComparisonType.ObjectCompare,
                LoggedUserEmail = loggedUser.Email
            };
            _auditLogService.Add(auditLogModel);
            _unitOfWork.Save();
            #endregion

            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleDelete)]
        public IActionResult Delete(int id)
        {
            var article = _articleService.GetSingle(x => x.Id == id);
            if (article == null) return RedirectToAction("Index");
            {

                // var sampleObject = new Article(); // Storage of this null object shows data after delete = nothing!
                _articleService.DeleteWhere(x => x.Id == id);
                #region AuditLog
                // _auditLogService.Add(AuditActionType.Delete, id.ToString(), article, sampleObject);
                _unitOfWork.Save();
                #endregion
            }
            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.DeletedSuccessfully].Value;
            return RedirectToAction("Index");
        }

        [HttpPost("DataHandler")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleView)]
        public async Task<JsonResult> DataHandler([FromBody] BlogDTParameters parameters)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                var articles = _articleService.GetAll().Select(art => new ArticleModel
                {
                    Id = art.Id,
                    Title = art.Title,
                    Author = art.Author,
                    CreatedDate=art.CreatedDate
                });
                List<ArticleModel> filteredAtricle;
                int totalArticle;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<ArticleModel, bool>> search =
                        x => (x.Author ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Title ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());
                    totalArticle = articles.Count(search);
                    articles = articles.Where(search);
                    articles = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), articles);
                    filteredAtricle = articles.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                else
                {
                    totalArticle = articles.Count();
                    articles = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), articles);
                    filteredAtricle = articles.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                var counter = parameters.Start + 1;
                var articleModels = filteredAtricle.Select(x => {
                    x.SN = counter++;
                    x.FormatedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, user.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var article = new DTResult<ArticleModel>
                {
                    Draw = parameters.Draw,
                    Data = articleModels,
                    RecordsFiltered = totalArticle,
                    RecordsTotal = totalArticle
                };
                return Json(article);
            }
            catch (Exception ex)
            {
                return Json(new {error = ex.Message});
            }
        }


        #region Helpers
        private static IQueryable<ArticleModel> SortByColumnWithOrder(int order, string orderDirection, IQueryable<ArticleModel> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Title) : data.OrderBy(p => p.Title);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Author) : data.OrderBy(p => p.Author);
                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.CreatedDate) : data.OrderBy(p => p.CreatedDate);
                    default:
                        return data.OrderByDescending(p => p.CreatedDate);
                }
            }
            catch (Exception)
            {
                return data;
            }
        }
        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}