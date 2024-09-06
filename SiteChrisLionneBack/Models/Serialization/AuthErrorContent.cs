namespace SiteChrisLionneBack.Models.Serialization
{
    public class AuthErrorContent
    {
        public AuthError error { get; set; }
    }

    public class AuthError
    {
        public int code { get; set; }
        public string message { get; set; }
        public Error[] errors { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public string domain { get; set; }
        public string reason { get; set; }
    }
}
