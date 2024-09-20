namespace CodeToImageGenerator.Web.Services
{
    public class ImageService : IImageService
    {
        public async Task<Stream> GenerateImageFromCodeAsync(string language, string code)
        {
            var imageStream = await Common.ImageGenerator.GenerateStream(language, code);

            return imageStream;
        }
    }
}
