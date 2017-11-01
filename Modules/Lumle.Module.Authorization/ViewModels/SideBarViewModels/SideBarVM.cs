using System.Collections.Generic;

namespace Lumle.Module.Authorization.ViewModels.SideBarViewModels
{
    public class SideBarVM
    {
        public List<SideBarItemVM> SideBarItems { get; } = new List<SideBarItemVM>
        {
            new SideBarItemVM
            {
                Name = "dashboard",
                DisplayName = "Dashboard",
                Icon = "glyph-icon icon-linecons-tv",
                Sequence = 1,
                MenuLevel = 0
            },
            new SideBarItemVM
            {
                Name = "blog",
                DisplayName = "Blog",
                Icon = "glyph-icon icon-rss",
                Sequence = 2,
                MenuLevel = 0
            },
            new SideBarItemVM { Name = "article", DisplayName = "Article", Icon = "", Sequence = 1, MenuLevel = 1 },
            new SideBarItemVM
            {
                Name = "authorization",
                DisplayName = "Authorization",
                Icon = "glyph-icon icon-unlock",
                Sequence = 3,
                MenuLevel = 0
            },
            new SideBarItemVM { Name = "permission", DisplayName = "Permission", Icon = "", Sequence = 1, MenuLevel = 1 },
            new SideBarItemVM { Name = "role", DisplayName = "Role", Icon = "", Sequence = 2, MenuLevel = 1 },
            new SideBarItemVM { Name = "user", DisplayName = "User", Icon = "", Sequence = 3, MenuLevel = 1 },
            new SideBarItemVM
            {
                Name = "localization",
                DisplayName = "Localization",
                Icon = "glyph-icon icon-language",
                Sequence = 4,
                MenuLevel = 0
            },
            new SideBarItemVM { Name = "culture", DisplayName = "Culture", Icon = "", Sequence = 1, MenuLevel = 1 },
            new SideBarItemVM
            {
                Name = "audit",
                DisplayName = "Audit",
                Icon = "glyph-icon icon-database",
                Sequence = 5,
                MenuLevel = 0
            },
            new SideBarItemVM { Name = "auditlog", DisplayName = "Audit Log", Icon = "", Sequence = 1, MenuLevel = 1 },
            new SideBarItemVM { Name = "customlog", DisplayName = "Custom Log", Icon = "", Sequence = 2, MenuLevel = 1 },
            new SideBarItemVM
            {
                Name = "adminconfig",
                DisplayName = "Configuration",
                Icon = "glyph-icon icon-gear",
                Sequence = 6,
                MenuLevel = 0
            },
            new SideBarItemVM{ Name = "emailtemplate", DisplayName = "Email Template", Icon = "", Sequence = 1, MenuLevel = 1 },
            new SideBarItemVM { Name = "credential", DisplayName = "Credential", Icon = "", Sequence = 2, MenuLevel = 1 },
            new SideBarItemVM { Name = "systemsetting", DisplayName = "System Setting", Icon = "", Sequence = 3, MenuLevel = 1 },
            new SideBarItemVM { Name = "systemhealth", DisplayName = "System Health", Icon = "", Sequence = 4, MenuLevel = 1 },
            new SideBarItemVM
            {
                Name = "publicuser",
                DisplayName = "Public User",
                Icon = "glyph-icon icon-group",
                Sequence = 7,
                MenuLevel = 0
            }
        };
    }
}
