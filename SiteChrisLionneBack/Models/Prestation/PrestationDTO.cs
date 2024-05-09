using SiteChrisLionneBack.Models.Image;

namespace SiteChrisLionneBack.Models.Prestation
{
    public class PrestationDTO
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<string> paragraphs { get; set; }
        public ImageDTO image { get; set; }
    }
}
