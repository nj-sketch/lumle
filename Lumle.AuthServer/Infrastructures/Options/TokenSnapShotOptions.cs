using IdentityServer4.EntityFramework.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.AuthServer.Infrastructures.Options
{
    public class TokenSnapShotOptions
    {
        public TableConfiguration DefaultSchema { get; set; } = null;

        public TableConfiguration TokenSnapShot { get; set; } = new TableConfiguration("Auth_TokenSnapShot");
    }
}
