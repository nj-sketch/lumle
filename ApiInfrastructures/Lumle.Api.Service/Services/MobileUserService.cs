using JsonApiDotNetCore.Services;
using Lumle.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using JsonApiDotNetCore.Data;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lumle.Api.Service.Services
{
    public class MobileUserService : EntityResourceService<MobileUser>
    {

        public MobileUserService(IJsonApiContext jsonApiContext, IEntityRepository<MobileUser> entityRepository, ILoggerFactory loggerFactory) : base(jsonApiContext, entityRepository, loggerFactory)
        {

        }

        public string SayHello()
        {
            return "hello";
        }

    }
}
