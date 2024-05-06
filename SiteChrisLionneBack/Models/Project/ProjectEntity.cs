using SiteChrisLionneBack.Models.Image;
using System.Buffers.Text;

namespace SiteChrisLionneBack.Models.Project
{
    public class ProjectEntity
    {
        public string id { get; set; }
        public string title { get; set; }
        public ImageEntity thumb_image { get; set; }
        public List<string> paragraphs { get; set; }
        public List<ImageEntity> images { get; set; }
        public ImageEntity banner_image { get; set; }
        public bool is_front_page { get; set; }
        public List<string> tags { get; set; }
    }
}
