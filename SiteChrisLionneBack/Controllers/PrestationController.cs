﻿using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SiteChrisLionneBack.Models.Image;
using SiteChrisLionneBack.Models.Prestation;
using SiteChrisLionneBack.Models.Project;
using System.Dynamic;
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
                    item.paragraphs = queryResult.GetValue<List<string>>("paragraphs");
                    item.image = queryResult.GetValue<ImageDTO>("image");
                    prestationsDTO.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return prestationsDTO;
        }

        [Authorize]
        [HttpPut]
        [Route("create", Name = "CreatePrestation")]
        public async Task<PrestationDTO> CreatePrestation([FromForm] PrestationToCreateDTO prestation)
        {
            PrestationDTO prestationDTO = new PrestationDTO();
            try 
            {
                prestationDTO.title = prestation.title;
                prestationDTO.paragraphs = prestation.paragraphs;

                var collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document();

                prestationDTO.id = docRef.Id;

                await ImageStore.deleteFolder(Config.prestationsImagesFolder, docRef.Id);

                prestationDTO.image = await ImageStore.uploadImage(Config.prestationsImagesFolder, docRef.Id, prestation.image);

                var document = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(prestationDTO));
                await docRef.SetAsync(document);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message); 
            }
            return prestationDTO;
        }

        [Authorize]
        [HttpPost]
        [Route("edit", Name = "EditPrestation")]
        public async Task EditPrestation([FromQuery] string id, [FromForm] PrestationToEditDTO prestation)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document(id);

                if (prestation.title != null)
                    await docRef.UpdateAsync("title", prestation.title);
                if (prestation.paragraphs != null)
                    await docRef.UpdateAsync("paragraphs", prestation.paragraphs);
                if (prestation.image != null)
                {
                    await ImageStore.deleteFolder(Config.prestationsImagesFolder, docRef.Id);
                    await docRef.UpdateAsync("image", await ImageStore.uploadImage(Config.prestationsImagesFolder, docRef.Id, prestation.image));
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("delete", Name = "DeletePrestation")]
        public async Task DeletePrestation([FromQuery] string id)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.prestationsCollection);
                var docRef = collection.Document(id);
                await ImageStore.deleteFolder(Config.prestationsImagesFolder, docRef.Id);
                await docRef.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
