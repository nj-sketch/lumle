namespace Lumle.Infrastructure.Constants.AuthorizeRules
{
    public static class Permissions
    {
        #region Dashboard Module Permission

        public const string DashboardView = "dashboard.read";

        #endregion

        #region Blog Module Permissions

        public const string BlogArticleCreate = "blog.article.create";
        public const string BlogArticleView = "blog.article.read";
        public const string BlogArticleUpdate = "blog.article.update";
        public const string BlogArticleDelete = "blog.article.delete";

        #endregion

        #region Authorization Module Permission

        public const string AuthorizationPermissionCreate = "authorization.permission.create";
        public const string AuthorizationPermissionView = "authorization.permission.read";
        public const string AuthorizationPermissionUpdate = "authorization.permission.update";
        public const string AuthorizationPermissionDelete = "authorization.permission.delete";

        public const string AuthorizationRoleCreate = "authorization.role.create";
        public const string AuthorizationRoleView = "authorization.role.read";
        public const string AuthorizationRoleUpdate = "authorization.role.update";
        public const string AuthorizationRoleDelete = "authorization.role.delete";


        public const string AuthorizationUserCreate = "authorization.user.create";
        public const string AuthorizationUserView = "authorization.user.read";
        public const string AuthorizationUserUpdate = "authorization.user.update";
        public const string AuthorizationUserDelete = "authorization.user.delete";

        #endregion

        #region Localization Module Permissions

        public const string LocalizationCultureView = "localization.culture.read";
        public const string LocalizationCultureUpdate = "localization.culture.update";
        public const string LocalizationCultureCreate= "localization.culture.create";

        #endregion

        #region Audit Module Permissions

        public const string AuditLogView = "audit.auditlog.read";
        public const string CustomLogView = "audit.customlog.read";

        #endregion

        #region Calendar Module Permissions

        public const string CalendarView = "calendar.calendar.read";
        public const string CalendarCreate = "calendar.calendar.create";
        public const string CalendarUpdate = "calendar.calendar.update";
        public const string CalendarDelete = "calendar.calendar.delete";

        #endregion

        #region Admin Config
        public const string AdminConfigEmailTemplateView = "adminconfig.emailtemplate.read";
        public const string AdminConfigEmailTemplateUpdate = "adminconfig.emailtemplate.update";

        public const string AdminConfigCredentialView = "adminconfig.credential.read";
        public const string AdminConfigCredentialUpdate = "adminconfig.credential.update";
        
        public const string AdminConfigSystemSettingView = "adminconfig.systemsetting.read";
        public const string AdminConfigSystemSettingUpdate = "adminconfig.systemsetting.update";
        #endregion

        #region Public User Module Permissions
        public const string PublicUserView = "publicuser.manageuser.read";
        public const string PublicUserUpdate = "publicuser.manageuser.update";
        #endregion

    }
}
