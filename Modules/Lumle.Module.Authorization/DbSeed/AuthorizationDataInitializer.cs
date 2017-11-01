using Lumle.Data.Data;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.Authorization.Entities;
using System;

namespace Lumle.Module.Authorization.DbSeed
{
    public  class AuthorizationDataInitializer : IDataInitializer
    {
        public AuthorizationDataInitializer()
        {

        }
        public  void Initialize(BaseContext _baseContext)
        {
            #region Seed Role Permisssion
            var permissions = new Permission[]
            {
                //dashboard 
                new Permission {Slug="dashboard.read", DisplayName= "View Dashboard", Menu="dashboard", SubMenu="", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

                //Blog >>Article
                new Permission {Slug="blog.article.create", DisplayName= "Create Article", Menu="blog", SubMenu="article", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="blog.article.read", DisplayName= "View Blog", Menu="blog", SubMenu="article", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="blog.article.update", DisplayName= "Update Blog", Menu="blog", SubMenu="article", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="blog.article.delete", DisplayName= "Delete Blog", Menu="blog", SubMenu="article", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               
                //Authorization  >> Permission
                new Permission {Slug="authorization.permission.create", DisplayName= "Create Permission", Menu="authorization", SubMenu="permission", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="authorization.permission.read", DisplayName= "View Permission", Menu="authorization", SubMenu="permission", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="authorization.permission.update", DisplayName= "Update Permission", Menu="authorization", SubMenu="permission", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
                new Permission {Slug="authorization.permission.delete", DisplayName= "Delete Permission", Menu="authorization", SubMenu="permission", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               
               //Authorization  >> Role
               new Permission {Slug="authorization.role.create", DisplayName= "Create Role", Menu="authorization", SubMenu="role", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.role.read", DisplayName= "View Role", Menu="authorization", SubMenu="role", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.role.update", DisplayName= "Update Role", Menu="authorization", SubMenu="role", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.role.delete", DisplayName= "Delete Role", Menu="authorization", SubMenu="role", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //Authorization >> User
               new Permission {Slug="authorization.user.create", DisplayName= "Create User", Menu="authorization", SubMenu="user", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.user.read", DisplayName= "View user", Menu="authorization", SubMenu="user", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.user.update", DisplayName= "Update User", Menu="authorization", SubMenu="user", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="authorization.user.delete", DisplayName= "Delete User", Menu="authorization", SubMenu="user", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //Localization
               new Permission {Slug="localization.culture.create", DisplayName= "Add Culture", Menu="localization", SubMenu="culture", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="localization.culture.read", DisplayName= "View Culture", Menu="localization", SubMenu="culture", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="localization.culture.update", DisplayName= "Edit Culture", Menu="localization", SubMenu="culture", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="localization.culture.delete", DisplayName= "Delete Culture", Menu="localization", SubMenu="culture", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //AuditLog
               new Permission {Slug="audit.auditlog.read", DisplayName= "View Audit Log", Menu="audit", SubMenu="auditlog", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="audit.customlog.read", DisplayName= "View Custom Log", Menu="audit", SubMenu="customlog", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //AdminConfig >> EmailTemplate
               new Permission {Slug="adminconfig.emailtemplate.read", DisplayName= "View Email Template", Menu="adminconfig", SubMenu="emailtemplate", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="adminconfig.emailtemplate.update", DisplayName= "Edit Email Template", Menu="adminconfig", SubMenu="emailtemplate", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //AdminConfig >> Credential
               new Permission {Slug="adminconfig.credential.read", DisplayName= "View Credential", Menu="adminconfig", SubMenu="credential", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="adminconfig.credential.update", DisplayName= "Edit Credential", Menu="adminconfig", SubMenu="credential", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //AdminConfig >> System Setting
               new Permission {Slug="adminconfig.systemsetting.read", DisplayName= "View System Setting", Menu="adminconfig", SubMenu="systemsetting", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="adminconfig.systemsetting.update", DisplayName= "Edit System Setting", Menu="adminconfig", SubMenu="systemsetting", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //PublicUser >> UserManagement
               //AdminConfig >> System Setting
               new Permission {Slug="publicuser.manageuser.read", DisplayName= "View Public User", Menu="publicuser", SubMenu="", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},
               new Permission {Slug="publicuser.manageuser.update", DisplayName= "Edit Public User", Menu="publicuser", SubMenu="", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

               //AdminConfig >> System Health
               new Permission {Slug="adminconfig.systemhealth.read", DisplayName= "View System Health", Menu="adminconfig", SubMenu="systemhealth", CreatedDate=DateTime.UtcNow, LastUpdated=DateTime.UtcNow},

            };
            _baseContext.Set<Permission>().AddRange(permissions);
            
            _baseContext.SaveChanges();
            #endregion
        }
    }
}
