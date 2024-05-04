namespace SiteChrisLionneBack.Models.Project
{
    public class ProjectDTO
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string thumb_image { get; set; }
        public List<string> paragraphs { get; set; }
        public List<string> images { get; set; }
        public string banner_image { get; set; }
        public bool is_front_page { get; set; }
    }
}