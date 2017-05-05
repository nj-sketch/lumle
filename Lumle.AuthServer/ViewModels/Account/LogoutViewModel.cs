using Lumle.AuthServer.Models.Account;

namespace Lumle.AuthServer.ViewModels.Account
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}
