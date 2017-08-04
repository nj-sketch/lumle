using System.Collections.Generic;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;

namespace Lumle.Api.Infrastructures.Helpers
{
    public static class ActionResponseHelper
    {

        public static IEnumerable<Message> GetAllErrors(IEnumerable<ShortMessage> msgObjs)
        {
            var msgs = AutoMapper.Mapper.Map<IEnumerable<Message>>(msgObjs);
            return msgs;
        }

    }
}
