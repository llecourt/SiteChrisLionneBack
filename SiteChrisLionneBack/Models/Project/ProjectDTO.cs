﻿using SiteChrisLionneBack.Models.Image;

namespace SiteChrisLionneBack.Models.Project
{
    public class ProjectDTO
    {
        public string id { get; set; }
        public string title { get; set; }
        public ImageDTO thumb_image { get; set; }
        public List<string> paragraphs { get; set; }
        public List<ImageDTO> images { get; set; }
        public ImageDTO banner_image { get; set; }
        public bool is_front_page { get; set; }
        public List<string> tags { get; set; }
    }
}