using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using System.Text.RegularExpressions;
using Google.Api.Gax.ResourceNames;
using SiteChrisLionneBack.Models.Project;

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
                    item.description = queryResult.GetValue<string>("description");
                    item.thumb_image = queryResult.GetValue<string>("thumb_image");
                    item.paragraphs = queryResult.GetValue<List<string>>("paragraphs");
                    item.images = queryResult.GetValue<List<string>>("images");
                    item.banner_image = queryResult.GetValue<string>("banner_image");
                    item.is_front_page = queryResult.GetValue<bool>("is_front_page");
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
                projectDTO.description = project.description;
                projectDTO.paragraphs = project.paragraphs;
                projectDTO.is_front_page = project.is_front_page;

                var collection = Database.db.Collection(Config.projectsCollection);
                var docRef = collection.Document();

                projectDTO.id = docRef.Id;

                await ImageStore.deleteFolder(Config.projectImagesFolder, docRef.Id);
                
                projectDTO.thumb_image = await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.thumb_image);
                projectDTO.banner_image = await ImageStore.uploadImage(Config.projectImagesFolder, docRef.Id, project.banner_image);
                projectDTO.images = await ImageStore.uploadImages(Config.projectImagesFolder, docRef.Id, project.images);

                Dictionary<string, object> document = new Dictionary<string, object>
                {
                    { "id", projectDTO.id },
                    { "title", projectDTO.title },
                    { "description", projectDTO.description },
                    { "thumb_image", projectDTO.thumb_image },
                    { "paragraphs", projectDTO.paragraphs },
                    { "images", projectDTO.images },
                    { "banner_image", projectDTO.banner_image },
                    { "is_front_page", projectDTO.is_front_page },
                };
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
        public async void EditProject([FromQuery] string id, [FromForm] ProjectToEditDTO project)
        {
            try
            {
                CollectionReference collection = Database.db.Collection(Config.projectsCollection);
                var docRef = collection.Document(id);

                if (project.title != null)
                    await docRef.UpdateAsync("title", project.title);
                if(project.description != null)
                    await docRef.UpdateAsync("description", project.description);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpDelete]
        [Route("delete", Name = "DeleteProject")]
        public async void DeleteProject([FromQuery] string id)
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