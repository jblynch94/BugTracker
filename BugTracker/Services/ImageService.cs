using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class ImageService : IImageService
    {

        private readonly string _defaultBugTrackerImage = "/img/DefaultContactImage.png";
        private readonly string _defaultUserImage = "/img/DefaultContactImage.png";
        

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {
            if (fileData == null || fileData.Length == 0) 
            {
                switch (imageType)
                {
                    //BlogUser Image based on the 'defaultImage' enum
                    case 1: return _defaultUserImage;
                    //BlogPost Image based on the 'defaultImage' enum
                    case 2: return _defaultBugTrackerImage;
                    
                }
                
            }
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData!);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
