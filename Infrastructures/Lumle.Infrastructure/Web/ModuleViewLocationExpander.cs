using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Lumle.Infrastructure.Web
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {
        private const string ModuleKey = "module";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (!context.Values.ContainsKey(ModuleKey)) return viewLocations;

            var module = context.Values[ModuleKey];
            if (string.IsNullOrWhiteSpace(module)) return viewLocations;

            var moduleViewLocations = new string[]
            {
                "/Modules/Lumle.Module." + module + "/Views/{1}/{0}.cshtml",
                "/Modules/Lumle.Module." + module + "/Views/Shared/{0}.cshtml"
            };

            viewLocations = moduleViewLocations.Concat(viewLocations);
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var controller = context.ActionContext.ActionDescriptor.DisplayName;
            var moduleName = controller.Split('.')[2];
            context.Values[ModuleKey] = moduleName;
        }
    }
}
