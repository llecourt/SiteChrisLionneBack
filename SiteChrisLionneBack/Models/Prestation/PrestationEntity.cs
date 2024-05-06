using SiteChrisLionneBack.Models.Image;

namespace SiteChrisLionneBack.Models.Prestation
{
    public class PrestationEntity
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public ImageEntity image { get; set; }
    }
}
