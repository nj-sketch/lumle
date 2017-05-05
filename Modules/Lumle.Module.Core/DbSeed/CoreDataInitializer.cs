using Lumle.Core.Models;
using Lumle.Data.Data;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using System;

namespace Lumle.Module.Core.DbSeed
{
    public class CoreDataInitializer : IDataInitializer
    {
        public CoreDataInitializer()
        {

        }
        public void Initialize(BaseContext _baseContext)
        {

            #region Seed System Role
            var roles = new Role[]
            {
                new Role {Id="1", Name="superadmin", NormalizedName="SUPERADMIN", ConcurrencyStamp="bd3bee0b-5f1d-482d-b890-ffdc01915da3", Priority=1, IsBlocked=false },
                new Role {Id="2", Name="admin", NormalizedName="ADMIN", ConcurrencyStamp="6fc2a194-3tt8-43c1-85ee-9c7ced966628", Priority=2, IsBlocked=false },
                new Role {Id="3", Name="manager", NormalizedName="MANAGER", ConcurrencyStamp="6fc2a194-3e45-43c1-85ee-9c7ced966628", Priority=3, IsBlocked=false },
                new Role {Id="4", Name="guest", NormalizedName="GUEST", ConcurrencyStamp="6fc2a194-3tt5-43c1-85ee-9c7ced966628", Priority=4, IsBlocked=false }
            };

            _baseContext.Set<Role>().AddRange(roles);
            _baseContext.SaveChanges();
            #endregion

            #region Seed Role Claim Of Admin for Base Role Claim
            var baseRoleClaims = new BaseRoleClaim[]
            {
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.permission.create", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.permission.read", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.permission.update", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.permission.delete", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.role.create", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.role.read", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.role.update", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.role.delete", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.user.create", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.user.read", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.user.update", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new BaseRoleClaim{ClaimType="permission", ClaimValue="authorization.user.delete", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow}
            };

            _baseContext.Set<BaseRoleClaim>().AddRange(baseRoleClaims);
            _baseContext.SaveChanges();
            #endregion

            #region Seed System Language
            var cultures = new Culture[]
            {
                new Culture { Name="en-US", DisplayName="English (United State)", IsEnable=true, IsActive=true, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="fr-FR", DisplayName="French (France)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="ne", DisplayName="Nepali (Nepal)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="de", DisplayName="German (Germany)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="es-ES", DisplayName="Spanish (Spain)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="ja", DisplayName="Japanese", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="ko", DisplayName="Korean", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="zh-Hans", DisplayName="Chinese (Simplified)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new Culture { Name="it-it", DisplayName="Italian (Italy)", IsEnable=false, IsActive=false, CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow }
            };

            _baseContext.Set<Culture>().AddRange(cultures);
            _baseContext.SaveChanges();
            #endregion

            #region Seed Resource Category
            var resourceCategories = new ResourceCategory[]
            {
                new ResourceCategory { Name="Label", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
                new ResourceCategory { Name="Message", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow },
            };

            _baseContext.Set<ResourceCategory>().AddRange(resourceCategories);
            _baseContext.SaveChanges();
            #endregion

            #region Seed Resource
            var resources = new Resource[]
            {
                //Message
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Added successfully", Value="Added successfully", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Updated successfully", Value="Updated successfully", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Deleted successfully", Value="Deleted successfully", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Success", Value="Success", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Email sent successfully", Value="Email sent successfully", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Error occured", Value="Error occured", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Please select valid item", Value="Please select valid item", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Resource you are looking no longer exits", Value="Resource you are looking no longer exits", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Something went wrong. Please try again later", Value="Something went wrong. Please try again later", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Please fill all the required field", Value="Please fill all the required field", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to send activation email. Please try again", Value="Unable to send activation email. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Role not found', N'Role not found", Value="Role not found', N'Role not found", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to create role. Role already exist", Value="Unable to create role. Role already exist", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to assign role to user", Value="Unable to assign role to user", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to manage user role", Value="Unable to manage user role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to delete. Please try again", Value="Unable to delete. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to add. Please try again", Value="Unable to add. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to update. Please try again", Value="Unable to update. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Success!!! Your account has been created please check your email and confirm to login", Value="Success!!! Your account has been created please check your email and confirm to login", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your email has been confirmed. Please reset password to login", Value="Your email has been confirmed. Please reset password to login", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your password has been reset. Please enter credential to login", Value="Your password has been reset. Please enter credential to login", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your security code is:", Value="Your security code is:", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your email has been confirmed", Value="Your email has been confirmed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Invalid login attempt. Please contact your adminstration", Value="Invalid login attempt. Please contact your adminstration", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to create User. Please try again", Value="Unable to create User. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Error from external provider:", Value="Error from external provider:", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to sign in. Please contact your admin", Value="Unable to sign in. Please contact your admin", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Email already confirmed", Value="Email already confirmed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="This link is invalid", Value="This link is invalid", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="This link has been expired", Value="This link has been expired", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Somthing went wrong, Please contact your admin", Value="Somthing went wrong, Please contact your admin", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Cannot reset your password due to internal server error. Please try again", Value="Cannot reset your password due to internal server error. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Invalid code", Value="Invalid code", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your password has been changed", Value="Your password has been changed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your phone number was added", Value="Your phone number was added", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your phone number was removed", Value="Your phone number was removed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your password has been set", Value="Your password has been set", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="The external login was added", Value="The external login was added", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Your two-factor authentication provider has been set", Value="Your two-factor authentication provider has been set", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Unable to change password", Value="Unable to change password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="The external login was removed", Value="The external login was removed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Failed to verify phone number", Value="Failed to verify phone number", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Event not found. Please try again", Value="Event not found. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=2,  Key="Holiday not found. Please try again", Value="Holiday not found. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Data inserted successfully", Value="Data inserted successfully", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Unable to insert data. Please try again", Value="Unable to insert data. Please try again", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Unable to insert data due to Invalid excel format", Value="Unable to insert data due to Invalid excel format", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Buttons
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add New", Value="Add New", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add", Value="Add", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Manage", Value="Manage", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit", Value="Edit", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Save", Value="Save", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Submit", Value="Submit", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Delete", Value="Delete", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Cancel", Value="Cancel", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Back", Value="Back", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Reset", Value="Reset", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Sign Up", Value="Sign Up", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Log In", Value="Log In", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Register", Value="Register", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Menu and Submenu
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Dashboard", Value="Dashboard", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Blog", Value="Blog", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Article", Value="Article", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Authorization", Value="Authorization", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Permission", Value="Permission", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Role", Value="Role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="User", Value="User", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Localization", Value="Localization", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Culture", Value="Culture", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Audit", Value="Audit", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Audit Log", Value="Audit Log", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Custom Log", Value="Custom Log", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Calendar", Value="Calendar", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Configuration", Value="Configuration", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Email Template", Value="Email Template", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Credential", Value="Credential", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="System Setting", Value="System Setting", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="System Health", Value="System Health", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //General
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="S.N.", Value="S.N.", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Action", Value="Action", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Please select", Value="Please select", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Enter your password", Value="Enter your password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Download", Value="Download", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Are you sure you want to delete", Value="Are you sure you want to delete", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="of", Value="of", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Admin Configuration
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Credential Category", Value="Credential Category", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Category", Value="Category", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Manage Credential", Value="Manage Credential", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Manage Email Template", Value="Manage Email Template", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Value", Value="Value", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Title", Value="Title", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Subject", Value="Subject", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Load Last Template", Value="Load Last Template", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Load Default Template", Value="Load Default Template", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Email Subject", Value="Email Subject", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Email Body", Value="Email Body", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="System Health Report", Value="System Health Report", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Last checked on", Value="Last checked on", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Service", Value="Service", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Status", Value="Status", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Response", Value="Response", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="System Maintance Mode", Value="System Maintance Mode", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="ON", Value="ON", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="OFF", Value="OFF", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Select All", Value="Select All", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Select roles that can access system on maintenance mode", Value="Select roles that can access system on maintenance mode", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Update System Maintenance Mode", Value="Update System Maintenance Mode", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Enter password to update system maintenance mode", Value="Enter password to update system maintenance mode", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Localization
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Select language", Value="Select language", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Language", Value="Language", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="All", Value="All", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Label", Value="Label", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Message", Value="Message", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Original Word", Value="Original Word", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Translated Word", Value="Translated Word", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Filter by", Value="Filter by", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Please choose excel files", Value="Please choose excel files", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Resource type", Value="Resource type", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Import data from Excel", Value="Import data from Excel", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Import Excel", Value="Import Excel", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Excel sheet must only contain SN, Key and Value column in given order", Value="Excel sheet must only contain SN, Key and Value column in given order", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Authorization
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Name", Value="Name", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Username", Value="Username", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Priority", Value="Priority", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Description", Value="Description", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Delete Role", Value="Delete Role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit Role", Value="Edit Role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Role Name", Value="Role Name", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Role Priority", Value="Role Priority", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add Role", Value="Add Role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Role Credential Management", Value="Role Credential Management", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Export Excel", Value="Export Excel", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Export Pdf", Value="Export Pdf", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Email", Value="Email", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Email Confirmed", Value="Email Confirmed", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Profile", Value="Profile", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Personal Information", Value="Personal Information", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="First Name", Value="First Name", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Last Name", Value="Last Name", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Phone", Value="Phone", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Website", Value="Website", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="About Me", Value="About Me", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Time Zone", Value="Time Zone", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit Profile", Value="Edit Profile", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit User", Value="Edit User", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Account Status", Value="Account Status", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add User", Value="Add User", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Change Photo", Value="Change Photo", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Change Password", Value="Change Password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Logout", Value="Logout", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Core
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Reset password", Value="Reset password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Confirm password", Value="Confirm password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Forgot password", Value="Forgot password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Forgot password confirmation", Value="Forgot password confirmation", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Locked out", Value="Locked out", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Login failure", Value="Login failure", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="User Sign Up", Value="User Sign Up", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="User Login", Value="User Login", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Enter password", Value="Enter password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Forgot your password?", Value="Forgot your password?", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Remember me", Value="Remember me", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Verify", Value="Verify", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="New Password", Value="New Password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Enter your email", Value="Enter your email", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Send verification code", Value="Send verification code", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Select two-factor authentication provider:", Value="Select two-factor authentication provider:", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Reset password confirmation", Value="Reset password confirmation", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Your password has been reset", Value="Your password has been reset", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Enter your email to reset your password", Value="Enter your email to reset your password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Reset password email sent", Value="Reset password email sent", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Please check your email to reset your password', N'Please check your email to reset your password", Value="Please check your email to reset your password', N'Please check your email to reset your password", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="This account has been locked out, please try again later", Value="This account has been locked out, please try again later", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Unsuccessful login with service", Value="Unsuccessful login with service", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="You have successfully authenticated with", Value="You have successfully authenticated with", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Please enter an email address for this site below and click the Register button to finish logging in", Value="Please enter an email address for this site below and click the Register button to finish logging in", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Calendar
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Manage Holiday", Value="Manage Holiday", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Manage Event", Value="Manage Event", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Holiday", Value="Holiday", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Event", Value="Event", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Date", Value="Date", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Delete Holiday", Value="Delete Holiday", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Delete Event", Value="Delete Event", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Start", Value="Start", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="End", Value="End", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit Holiday", Value="Edit Holiday", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit Event", Value="Edit Event", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Remarks", Value="Remarks", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Event Time", Value="Event Time", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add Holiday", Value="Add Holiday", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add Event", Value="Add Event", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Audit Filter Attributes
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="User Profile", Value="User Profile", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="User Role", Value="User Role", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Module", Value="Module", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Audit By", Value="Audit By", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Audit Action", Value="Audit Action", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Audit Date", Value="Audit Date", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Summary", Value="Summary", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Tool", Value="Tool", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //System Log Attribute
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Remote Address", Value="Remote Address", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Log Level", Value="Log Level", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Logged Date", Value="Logged Date", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Browser", Value="Browser", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Blog Module
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Add Article", Value="Add Article", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Content", Value="Content", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Publish", Value="Publish", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Author", Value="Author", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Created Date", Value="Created Date", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Edit Article", Value="Edit Article", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},

                //Error Module
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Error", Value="Error", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Bad request. Please check your request", Value="Bad request. Please check your request", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="For more information please contact your admin", Value="For more information please contact your admin", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Forbidden request. Make sure you got permission to access resources", Value="Forbidden request. Make sure you got permission to access resources", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Page not found", Value="Page not found", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="The page you are looking for has been moved or no longer exists", Value="The page you are looking for has been moved or no longer exists", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Take Me Home", Value="Take Me Home", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="System under maintenance mode", Value="System under maintenance mode", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow},
                new Resource{CultureId=1, ResourceCategoryId=1,  Key="Please try again in a few minutes or contact the website administrator", Value="Please try again in a few minutes or contact the website administrator", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow}
            };

            _baseContext.Set<Resource>().AddRange(resources);
            _baseContext.SaveChanges();

            #endregion
        }
    }
}
