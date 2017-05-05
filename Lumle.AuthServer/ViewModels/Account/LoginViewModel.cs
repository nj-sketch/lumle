using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lumle.AuthServer.Infrastructures.Providers;
using Lumle.AuthServer.Models.Account;

namespace Lumle.AuthServer.ViewModels.Account
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }
        public bool EnableLocalLogin { get; set; }
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    }
}
