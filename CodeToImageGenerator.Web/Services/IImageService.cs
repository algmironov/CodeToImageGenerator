namespace CodeToImageGenerator.Web.Services
{
    public interface IImageService
    {
        Task<Stream> GenerateImageFromCodeAsync(string language, string code);
        string GenerateFileName(string language);
    }
}
