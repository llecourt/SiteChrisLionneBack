using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using SiteChrisLionneBack.Models.Serialization;
using System.Text.Json;

namespace SiteChrisLionneBack
{
    public class Database
    {
        private static Database instance;
        public static FirestoreDb db { get; private set; }

        public static void init()
        {
            if (instance != null) return;
            instance = new Database();
            string jsonString = System.IO.File.ReadAllText(Config.googleCredentialsFileName);
            FirebaseDbSettings dbSettings = JsonSerializer.Deserialize<FirebaseDbSettings>(jsonString)!;
            FirestoreDbBuilder dbBuilder = new FirestoreDbBuilder
            {
                ProjectId = dbSettings.project_id,
                JsonCredentials = jsonString
            };
            db = dbBuilder.Build();
            Console.WriteLine("Database initialized");
        }
    }
}
