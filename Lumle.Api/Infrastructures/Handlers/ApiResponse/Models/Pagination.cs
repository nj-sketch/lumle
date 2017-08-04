namespace Lumle.Api.Infrastructures.Handlers.ApiResponse.Models
{
    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int Size { get; set; }
        public int TotalObject { get; set; }
        public string Previous { get; set; }
        public string Next { get; set; }
    }
}
