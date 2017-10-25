using Lumle.Core.Models;
using Lumle.Data.Data;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.Localization;
using System;
using System.Collections.Generic;
using System.Reflection;

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
                new Role { Id="1", Name="superadmin", NormalizedName="SUPERADMIN", ConcurrencyStamp="bd3bee0b-5f1d-482d-b890-ffdc01915da3", Priority=1, IsBlocked=false },
                new Role { Id="2", Name="admin", NormalizedName="ADMIN", ConcurrencyStamp="6fc2a194-3tt8-43c1-85ee-9c7ced966628", Priority=2, IsBlocked=false },
                new Role { Id="3", Name="manager", NormalizedName="MANAGER", ConcurrencyStamp="6fc2a194-3e45-43c1-85ee-9c7ced966628", Priority=3, IsBlocked=false },
                new Role { Id="4", Name="guest", NormalizedName="GUEST", ConcurrencyStamp="6fc2a194-3tt5-43c1-85ee-9c7ced966628", Priority=4, IsBlocked=false }
            };

            _baseContext.Set<Role>().AddRange(roles);
            _baseContext.SaveChanges();
            #endregion

            #region Seed Role Claim Of Admin for Base Role Claim
            var baseRoleClaims = new BaseRoleClaim[]
            {
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.role.create", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.role.read", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.role.update", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.role.delete", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },

                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.user.create", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.user.read", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.user.update", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
                new BaseRoleClaim{ ClaimType="permission", ClaimValue="authorization.user.delete", RoleId="1", CreatedDate= DateTime.UtcNow, LastUpdated = DateTime.UtcNow }
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

            var resources = new List<Resource>();
            foreach (FieldInfo field in typeof(LabelConstants).GetFields())
            {
                var label = field.GetRawConstantValue().ToString();
                resources.Add(new Resource { CultureId = 1, ResourceCategoryId = 1, Key = label, Value = label, CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow });
            }

            foreach (FieldInfo field in typeof(ActionMessageConstants).GetFields())
            {
                var message = field.GetRawConstantValue().ToString();
                resources.Add(new Resource { CultureId = 1, ResourceCategoryId = 2, Key = message, Value = message, CreatedDate = DateTime.UtcNow, LastUpdated = DateTime.UtcNow });
            }

            _baseContext.Set<Resource>().AddRange(resources);
            _baseContext.SaveChanges();

            #endregion

            #region Seed Default User
            var user = new User
            {
                Id = "a2764928-3832-4d8d-a004-9fa7fa46c39c",
                AccessFailedCount = 0,
                AccountStatus = 2,
                ConcurrencyStamp = "e99af957-0e57-47d0-8f0b-28edd28566a4",
                CreatedBy = "demo",
                CreatedDate = DateTime.UtcNow,
                Culture = "en-US",
                Email = "demo@ekbana.com",
                EmailConfirmed = true,
                LockoutEnabled = true,
                NormalizedEmail = "DEMO@EKBANA.COM",
                NormalizedUserName = "DEMO",
                PasswordHash = "AQAAAAEAACcQAAAAEBtXs+EqRMTJF9toqV18C2NtPgR3XhjhnSTwZhtPtLSOYIhIImebLdwQ2g0qlZtNqQ==",
                PhoneNumberConfirmed = false,
                SecurityStamp = "93f976e6-d0c1-4618-9a13-ba3abba0cc86",
                TimeZone = "Asia/Kathmandu",
                TwoFactorEnabled = false,
                UserName = "demo"

            };
            _baseContext.Set<User>().Add(user);
            _baseContext.SaveChanges();

            var userRole = new UserRole { RoleId = "1", UserId = "a2764928-3832-4d8d-a004-9fa7fa46c39c" };
            _baseContext.Set<UserRole>().Add(userRole);
            _baseContext.SaveChanges();

            var userProfile = new UserProfile()
            {
                Email = "demo@ekbana.com",
                UserName = "demo",
                UserId = "a2764928-3832-4d8d-a004-9fa7fa46c39c",
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsDeleted = false,
                City = "lalitpur",
                State = 1,
                StreetAddress = "kupondole",
                Country = 155,
                PostalCode = "99999"
            };
            _baseContext.Set<UserProfile>().Add(userProfile);
            _baseContext.SaveChanges();
            #endregion
        }
    }
}
