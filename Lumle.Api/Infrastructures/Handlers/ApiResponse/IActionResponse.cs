using System.Collections.Generic;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;

namespace Lumle.Api.Infrastructures.Handlers.ApiResponse
{
    public interface IActionResponse
    {

        ActionResponse GetResponse(object data = null, IEnumerable<Message> error = null, IEnumerable<Message> message = null, IEnumerable<object> meta = null, Pagination pagination = null);

    }
}
