﻿using SiteChrisLionneBack.Models.Image;

namespace SiteChrisLionneBack.Models.Prestation
{
    public class PrestationDTO
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public ImageDTO image { get; set; }
    }
}
