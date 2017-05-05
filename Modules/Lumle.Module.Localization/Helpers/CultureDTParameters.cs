using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.Localization.Helpers
{
    public class CultureDTParameters : DTParameters
    {
        public string Culture { get; set; }
        public int ResourceCategoryId { get; set; }
    }
}
