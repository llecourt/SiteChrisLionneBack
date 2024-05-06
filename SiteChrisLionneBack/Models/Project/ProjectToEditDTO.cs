namespace SiteChrisLionneBack.Models.Project
{
    public class ProjectToEditDTO
    {
        public string? title { get; set; }
        public IFormFile? thumb_image { get; set; }
        public List<string>? paragraphs { get; set; }
        public List<IFormFile>? images { get; set; }
        public IFormFile? banner_image { get; set; }
        public bool? is_front_page { get; set; }
        public List<string>? tags { get; set; }
    }
}
