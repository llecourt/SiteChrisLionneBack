namespace SiteChrisLionneBack.Models.Prestation
{
    public class PrestationToEditDTO
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public IFormFile? image { get; set; }
    }
}
