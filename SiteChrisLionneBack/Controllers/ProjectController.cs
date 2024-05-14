using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.RegularExpressions;
using Google.Api.Gax.ResourceNames;
using SiteChrisLionneBack.Models.Project;
using SiteChrisLionneBack.Models.Image;
using Newtonsoft.Json;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Collections.Generic;

namespace SiteChrisLionneBack.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController : ControllerBase
    {
        [HttpGet(Name = "GetAllProjects")]
        public async Task<List<ProjectDTO>> GetAll([FromQuery] bool frontPageOnly = false)
        {
            List<ProjectDTO> projects = new List<ProjectDTO>();
            try
            {
                CollectionReference collection = Database.db.Collection(Config.projectsCollection);
                Query query = collection;
                if (frontPageOnly)
                {
                    query = collection.WhereEqualTo("is_front_page", frontPageOnly);
                }

                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot queryResult in querySnapshot.Documents)
                {
                    ProjectDTO item = new ProjectDTO();
                    item.id = queryResult.GetValue<string>("id");
                    item.title = queryResult.GetValue<string>("title");
                    item.thumb_image = queryResult.GetValue<ImageDTO>("thumb_image");
                    item.paragraphs = queryResult.GetValue<List<string>>("paragraphs");
                    item.images = queryResult.GetValue<List<ImageDTO>>("images");
                    item.banner_image = queryResult.GetValue<ImageDTO>("banner_image");
                    item.is_front_page = queryResult.GetValue<bool>("is_front_page");
                    item.tags = queryResult.GetValue<List<string>>("tags");
                    projects.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return projects;
        }

        [HttpPut]
        [Route("create", Name = "CreateProject")]
        public async Task<ProjectDTO> CreateProject([FromForm] ProjectToCreateDTO project)
        {
            ProjectDTO projectDTO = new ProjectDTO();
            try
            {
                projectDTO.title = project.title;
                projectDTO.paragraphs = project.paragraphs;
                projectDTO.is_front_page = project.is_front_page;
                projectDTO.tags = project.tags;

                var collection = Database.db.Collection(Config.projectsCollection);
                var docRef = collection.Document();

                projectDTO.id = docRef.Id;

                await ImageStore.deleteFolder(Config.projectImagesFolder, docRef.Id);
                
                projectDTO.thumb_image = await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.thumb_image);
                projectDTO.banner_image = await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.banner_image);
                projectDTO.images = await ImageStore.uploadImages(Config.projectImagesFolder, docRef.Id, project.images);

                var document = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(projectDTO));
                await docRef.SetAsync(document);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return projectDTO;
        }

        [HttpPost]
        [Route("edit", Name = "EditProject")]
        public async Task EditProject([FromQuery] string id, [FromForm] ProjectToEditDTO project)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.projectsCollection);
                var docRef = collection.Document(id);

                if (project.title != null)
                    await docRef.UpdateAsync("title", project.title);
                if (project.paragraphs != null)
                    await docRef.UpdateAsync("paragraphs", project.paragraphs);
                if (project.is_front_page != null)
                    await docRef.UpdateAsync("is_front_page", project.is_front_page);
                if (project.thumb_image != null)
                    await docRef.UpdateAsync("thumb_image", await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.thumb_image));
                if (project.banner_image != null)
                    await docRef.UpdateAsync("banner_image", await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.banner_image));
                if (project.images != null)
                    await docRef.UpdateAsync("images", await ImageStore.uploadImages(Config.projectImagesFolder, docRef.Id, project.images));
                if(project.tags != null)
                    await docRef.UpdateAsync("tags", project.tags);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete", Name = "DeleteProject")]
        public async Task DeleteProject([FromQuery] string id)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.projectsCollection);
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