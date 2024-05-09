namespace SiteChrisLionneBack.Models.Prestation
{
    public class PrestationToCreateDTO
    {
        public string title { get; set; }
        public List<string> paragraphs { get; set; }
        public IFormFile image { get; set; }
    }
}
