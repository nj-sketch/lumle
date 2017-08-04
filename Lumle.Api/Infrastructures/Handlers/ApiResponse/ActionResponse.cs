using System.Collections.Generic;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;

namespace Lumle.Api.Infrastructures.Handlers.ApiResponse
{
    public class ActionResponse : IActionResponse
    {
        public object Data { get; set; }

        public IEnumerable<Message> Errors { get; set; }

        public IEnumerable<Message> Messages { get; set; }

        public IEnumerable<object> Meta { get; set; }

        public Pagination Pagination { get; set; }

        public ActionResponse GetResponse(object data = null, IEnumerable<Message> error = null, IEnumerable<Message> message = null, IEnumerable<object> meta = null,
            Pagination pagination = null)
        {
            return new ActionResponse { Data = data, Errors = error, Messages = message, Meta = meta, Pagination = pagination };
        }
    }
}
