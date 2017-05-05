using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Infrastructure
{
    public abstract class BaseActionPrevilege
    {
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}
