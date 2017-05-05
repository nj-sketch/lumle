using System.Linq;
using System.Reflection;

namespace Lumle.Infrastructure
{
    public class ModuleInfo
    {
        public string Name { get; set; }

        public Assembly Assembly { get; set; }

        public string ShortName => Name.Split('.').Last();

        public string Path { get; set; }
    }
}
