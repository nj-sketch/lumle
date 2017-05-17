using IdentityServer4.EntityFramework.Options;

namespace Lumle.AuthServer.Infrastructures.Options
{
    public class UserStoreOptions
    {
        public TableConfiguration DefaultSchema { get; set; } = null;

        public  TableConfiguration CustomUser { get; set; } = new TableConfiguration("PublicUser_MobileUser");
    }
}
