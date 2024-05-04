using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using SiteChrisLionneBack.Models.Prestation;
using SiteChrisLionneBack.Models.Project;
using System.Text.RegularExpressions;

namespace SiteChrisLionneBack.Controllers
{
    [ApiController]
    [Route("prestations")]
    public class PrestationController
    {
        [HttpGet(Name = "GetAllPrestations")]
        public async Task<List<PrestationDTO>> GetAll()
        {
            List<PrestationDTO> prestationsDTO = new List<PrestationDTO>();
            try
            {
                CollectionReference collection = Database.db.Collection(Config.prestationsCollection);
                QuerySnapshot querySnapshot = await collection.GetSnapshotAsync();
                foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
                {
                    PrestationDTO item = new PrestationDTO();
                    item.id = queryResult.GetValue<string>("id");
                    item.title = queryResult.GetValue<string>("title");
                    item.description = queryResult.GetValue<string>("description");
                    item.image = queryResult.GetValue<string>("image");
                    prestationsDTO.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return prestationsDTO;
        }

        [HttpPut]
        [Route("create", Name = "CreatePrestation")]
        public async Task<PrestationDTO> CreatePrestation([FromForm] PrestationToCreateDTO prestation)
        {
            PrestationDTO prestationDTO = new PrestationDTO();
            try 
            {
                prestationDTO.title = prestation.title;
                prestationDTO.description = prestation.description;

                var collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document();

                prestationDTO.id = docRef.Id;

                await ImageStore.deleteFolder(Config.prestationsImagesFolder, docRef.Id);

                prestationDTO.image = await ImageStore.uploadImage(Config.prestationsImagesFolder, docRef.Id, prestation.image);
                
                Dictionary<string, object> document = new Dictionary<string, object>
                {
                    { "id",  prestationDTO.id },
                    { "title", prestationDTO.title },
                    { "description", prestationDTO.description },
                    { "image", prestationDTO.image },
                };
                await docRef.SetAsync(document);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message); 
            }
            return prestationDTO;
        }

        [HttpPost]
        [Route("edit", Name = "EditPrestation")]
        public async void EditPrestation([FromQuery] string id, [FromForm] PrestationToEditDTO prestation)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document(id);

                if (prestation.title != null)
                    await docRef.UpdateAsync("title", prestation.title);
                if (prestation.description != null)
                    await docRef.UpdateAsync("description", prestation.description);
                if (prestation.image != null)
                    await docRef.UpdateAsync("paragraphs", await ImageStore.uploadImage(Config.prestationsImagesFolder, docRef.Id, prestation.image));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete", Name = "DeletePrestation")]
        public async void DeletePrestation([FromQuery] string id)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document(id);
                await docRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
