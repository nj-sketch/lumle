using System;
using System.Collections.Generic;
using System.Net;
using Lumle.Module.Blog.Services;
using Lumle.Module.Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Module.Blog.Helpers;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Lumle.Core.Localizer;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Lumle.Core.Services.Abstracts;
using Lumle.Core.Models;
using System.Security.Claims;
using Lumle.Infrastructure.Constants.ActionConstants;

namespace Lumle.Module.Blog.Controllers
{
    [Route("blog/[controller]")]
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly IArticleService _articleService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;
        private readonly IHostingEnvironment _environment;
        private readonly IUrlHelper _urlHelper;
        private string _imageUrl;

        public ArticleController(
            IBaseRoleClaimService baseRoleClaimService,
            IArticleService articleService,
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer,
            IHostingEnvironment environment,
            IUrlHelper urlHelper
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _articleService = articleService;
            _userManager = userManager;
            _localizer = localizer;
            _environment = environment;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleView)]
        public async Task<ViewResult> Index()
        {
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.CreateAction, new Claim("permission", Permissions.BlogArticleCreate) },
                { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.BlogArticleUpdate) },
                { OperationActionConstant.DeleteAction, new Claim("permission", Permissions.BlogArticleDelete) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
            var actionModel = new ActionOperation
            {
                CreateAction = actionClaimResult[OperationActionConstant.CreateAction],
                UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction],
                DeleteAction = actionClaimResult[OperationActionConstant.DeleteAction]
            };

            return View(actionModel);
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
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
  
            var loggedUser = await GetCurrentUserAsync();
            await _articleService.Create(model, loggedUser);

            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;

            return RedirectToAction("Index");
        }

        [HttpGet("edit/{articleId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleUpdate)]
        public async Task<IActionResult> Edit(int articleId)
        {
            var article = await _articleService.GetSingleAsync(x => x.Id == articleId);
            var articleVm = Mapper.Map<ArticleVM>(article);
            _imageUrl = !string.IsNullOrEmpty(articleVm.FeaturedImageUrl) ? $"{Request.Scheme}://{Request.Host}{_urlHelper.Content("~/")}uploadedimages/{articleVm.FeaturedImageUrl}"
              : string.Empty;
            articleVm.FeaturedImageUrl = _imageUrl;

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

            var loggedUser = await GetCurrentUserAsync();

            await _articleService.Update(model, loggedUser);

            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.BlogArticleDelete)]
        public IActionResult Delete(int id)
        {
            _articleService.DeleteWhere(x => x.Id == id);
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

                // Get Datatable result
                var result = _articleService.GetDataTableResult(user, parameters);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new {error = ex.Message});
            }
        }

        #region Helpers
        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}