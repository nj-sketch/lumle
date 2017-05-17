using JsonApiDotNetCore.Internal;
using System.Collections.Generic;

namespace Lumle.Api.Infrastructures.CustomModels
{
    public class LumleApiError : Error
    {
        private readonly IEnumerable<string> _messages;

        public LumleApiError(string status, string title, string detail, IEnumerable<string> messages)
        :base(status, title, detail)
        {
                _messages = messages;
        }

        public LumleApiError(string status, string title, string detail)
        {
            _messages = new List<string>();
        }




    }
}
