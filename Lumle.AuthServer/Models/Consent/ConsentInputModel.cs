using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.AuthServer.Models.Consent
{
    public class ConsentInputModel
    {
        public string Button { get; set; }
        public IEnumerable<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
    }
}
