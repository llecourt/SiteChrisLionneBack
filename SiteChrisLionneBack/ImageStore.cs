using Google.Cloud.Storage.V1;
using SiteChrisLionneBack.Models.Image;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;

namespace SiteChrisLionneBack
{
    public class ImageStore
    {
        private static ImageStore instance;
        public static StorageClient client { get; private set; }

        public static void init()
        {
            if (instance != null) return;
            instance = new ImageStore();
            string jsonString = System.IO.File.ReadAllText(Config.googleCredentialsFileName);
            StorageClientBuilder builder = new StorageClientBuilder
            {
                JsonCredentials = jsonString
            };
            client = builder.Build();
            Console.WriteLine("Cloud storage initialized");
        }

        public static async Task<List<ImageDTO>> uploadImages(string rootFolderName, string imageFolderName, List<IFormFile> files)
        {
            List<ImageDTO> images = new List<ImageDTO>();
            foreach (IFormFile file in files)
            {
                var imgDTO = await uploadImage(rootFolderName, imageFolderName, file);
                images.Add(imgDTO);
            }
            return images;
        }

        public static async Task<ImageDTO> uploadImage(string rootFolderName, string imageFolderName, IFormFile file)
        {
            var imgDTO = new ImageDTO();
            imgDTO.sizes = new List<string>();

            var img = SixLabors.ImageSharp.Image.Load(file.OpenReadStream());

            imgDTO.name = await uploadConvertedImage(rootFolderName, imageFolderName, img);
            imgDTO.sizes.Add("orig");

            var sizes = new int[] { 1000, 500, 200 };
            var imageH = img.Size.Height;

            foreach (var size in sizes)
            {
                if (imageH > size)
                {
                    img.Mutate(x => x.Resize(0, size));
                    await uploadConvertedImage(rootFolderName, imageFolderName, img, "-" + size);
                    imgDTO.sizes.Add(size.ToString());
                }
            }
         
            return imgDTO;
        }

        private static async Task<string> uploadConvertedImage(string rootFolderName, string imageFolderName, Image file, string quality = "")
        {
            string fileName = file.GetHashCode().ToString() + quality;
            string uploadPath = string.Join("/", new string[] { rootFolderName, imageFolderName, fileName });
            
            var ms = new MemoryStream();
            await file.SaveAsync(ms, new JpegEncoder());

            await client.UploadObjectAsync(Config.bucketName, uploadPath, "image/jpeg", ms);

            return fileName;
        }

        public static async Task deleteFolder(string rootFolderName, string imageFolderName)
        {
            string folder = string.Join("/", new string[] { rootFolderName, imageFolderName });
            var objs = client.ListObjects(Config.bucketName, folder);
            foreach (var obj in objs)
            {
                await client.DeleteObjectAsync(obj);
            }
        }
    }
}
