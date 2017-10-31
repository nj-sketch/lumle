using System;
using AutoMapper;
using Lumle.Module.Authorization.Entities;
using Lumle.Module.Authorization.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Lumle.Core.Attributes;
using Lumle.Module.Authorization.ViewModels.PermissionViewModels;
using Lumle.Module.Authorization.Models;
using Lumle.Data.Data.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Core.Services.Abstracts;
using System.Security.Claims;
using Lumle.Infrastructure.Constants.ActionConstants;
using System.Threading.Tasks;

namespace Lumle.Module.Authorization.Controllers
{
    [Route("authorization/[controller]")]
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly IPermissionService _permissionService;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionController(
            IBaseRoleClaimService baseRoleClaimService,
            IPermissionService permissionService,
            IUnitOfWork unitOfWork
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _permissionService = permissionService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionView)]
        public async Task<IActionResult> Index()
        {
            #region action-privelege
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.AuthorizationPermissionUpdate) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
            #endregion

            var permissionEntities = _permissionService.GetAll();
            var permissions = Mapper.Map<IList<PermissionVM>>(permissionEntities);

            // Send permission edit previlege in view
            ViewBag.UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction];

            return View(permissions);
        }


        [HttpGet("add")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionCreate)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionCreate)]
        public async Task<IActionResult> Add(PermissionVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = "Please fill the required field.";
                    return View(model);
                }

                var isSlugExist =
                   await _permissionService.GetSingle(
                        x => string.Equals(x.Slug, model.Slug.Trim(), StringComparison.CurrentCultureIgnoreCase));

                if (isSlugExist != null)
                {
                    TempData["ErrorMsg"] = "Permission already exist. Try with new slug.";
                    return View(model);
                }

                var permissionModel = SplitSlug(model);
                var permissionEntity = Mapper.Map<Permission>(permissionModel);

                await _permissionService.Add(permissionEntity);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMsg"] = "Permission added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = "Error Occured. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet("edit/{id:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionUpdate)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMsg"] = "Permission not found.";
                return RedirectToAction("Index");
            }

            var permissionEntity = await _permissionService.GetSingle(x => x.Id == id);
            if (permissionEntity == null)
            {
                TempData["ErrorMsg"] = "Permission not found.";
                return RedirectToAction("Index");
            }

            var permission = Mapper.Map<PermissionVM>(permissionEntity);

            return View(permission);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionUpdate)]
        public async Task<IActionResult> Edit(PermissionVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = "Please fill the required field.";
                    return View(model);
                }

                var permission = await _permissionService.GetSingle(x => x.Id == model.Id);
                if (permission == null)
                {
                    TempData["ErrorMsg"] = "Permission does not exist.";
                    return View("Edit", model);
                }

                var permissionModel = SplitSlug(model);
                var permissionEntity = Mapper.Map<Permission>(permissionModel);
                permissionEntity.Id = model.Id;

                await _permissionService.Update(permissionEntity);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMsg"] = "Permission updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = "Error Occured. Please try again.";
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionDelete)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMsg"] = "Permission not found.";
                    return RedirectToAction("Index");
                }

                var permission = await _permissionService.GetSingle(x => x.Id == id);
                if (permission == null)
                {
                    TempData["ErrorMsg"] = "Permission not found.";
                    return RedirectToAction("Index");
                }

                await _permissionService.DeleteWhere(x => x.Id == id);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMsg"] = "Permission deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = "Error Occured. Please try again.";
                return RedirectToAction("Index");
            }
        }

        #region Helper Class
        private static PermissionModel SplitSlug(PermissionVM model)
        {
            var slugArray = model.Slug.Split('.');
            var permission = new PermissionModel
            {
                Slug = model.Slug.Trim().ToLower(),
                DisplayName = model.DisplayName.Trim(),
                Menu = slugArray[0].Trim().ToLower(),
                SubMenu = slugArray.Length > 1 ? slugArray[1].Trim().ToLower() : string.Empty,
                CreatedDate = DateTime.UtcNow
            };

            return permission;
        }
        #endregion
    }
}
