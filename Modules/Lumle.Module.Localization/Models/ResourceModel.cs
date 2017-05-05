namespace Lumle.Module.Localization.Models
{
    public class ResourceModel
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public int CultureId { get; set; }
        public int ResourceCategoryId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
