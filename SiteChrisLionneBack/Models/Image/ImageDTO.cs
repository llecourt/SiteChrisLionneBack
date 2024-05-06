using Google.Cloud.Firestore;

namespace SiteChrisLionneBack.Models.Image
{
    [FirestoreData]
    public class ImageDTO
    {
        [FirestoreProperty("name")]
        public string name { get; set; }
        [FirestoreProperty("sizes")]
        public List<string> sizes { get; set; }
    }
}
