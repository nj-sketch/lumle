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

namespace Lumle.Module.Authorization.Controllers
{
    [Route("authorization/[controller]")]
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionController(IPermissionService permissionService,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;

        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationPermissionView)]
        public IActionResult Index()
        {
            var permissionEntities = _permissionService.GetAll();
            var permissions = Mapper.Map<IEnumerable<PermissionVM>>(permissionEntities);

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
        public IActionResult Add(PermissionVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = "Please fill the required field.";
                    return View(model);
                }

                var isSlugExist =
                    _permissionService.GetSingle(
                        x => string.Equals(x.Slug, model.Slug.Trim(), StringComparison.CurrentCultureIgnoreCase));

                if (isSlugExist != null)
                {
                    TempData["ErrorMsg"] = "Permission already exist. Try with new slug.";
                    return View(model);
                }

                var permissionModel = SplitSlug(model);
                var permissionEntity = Mapper.Map<Permission>(permissionModel);

                _permissionService.Add(permissionEntity);
                _unitOfWork.Save();

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
        public IActionResult Edit(int id)
        {

            if (id <= 0)
            {
                TempData["ErrorMsg"] = "Permission not found.";
                return RedirectToAction("Index");
            }

            var permissionEntity = _permissionService.GetSingle(x => x.Id == id);
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
        public IActionResult Edit(PermissionVM model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = "Please fill the required field.";
                    return View(model);
                }

                var permission = _permissionService.GetSingle(x => x.Id == model.Id);
                if (permission == null)
                {
                    TempData["ErrorMsg"] = "Permission does not exist.";
                    return View("Edit", model);
                }

                var permissionModel = SplitSlug(model);
                var permissionEntity = Mapper.Map<Permission>(permissionModel);
                permissionEntity.Id = model.Id;

                _permissionService.Update(permissionEntity);
                _unitOfWork.Save();

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
        public IActionResult Delete(int id)
        {

            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMsg"] = "Permission not found.";
                    return RedirectToAction("Index");
                }

                var permission = _permissionService.GetSingle(x => x.Id == id);
                if (permission == null)
                {
                    TempData["ErrorMsg"] = "Permission not found.";
                    return RedirectToAction("Index");
                }

                _permissionService.DeleteWhere(x => x.Id == id);
                _unitOfWork.Save();

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
