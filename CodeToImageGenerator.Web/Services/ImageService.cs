using System.Text;

namespace CodeToImageGenerator.Web.Services
{
    public class ImageService : IImageService
    {
        public async Task<Stream> GenerateImageFromCodeAsync(string language, string code)
        {
            var imageStream = await Common.ImageGenerator.GenerateStream(language, code);

            return imageStream;
        }

        public string GenerateFileName(string language)
        {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var uniqueNameLength = 10;

            var sb = new StringBuilder();

            for (var i = 0; i < uniqueNameLength; i++)
            {
                sb.Append(letters[random.Next(letters.Length -1)]);
            }

            return $"{language}_{sb}.png";
        }
    }
}
