using System.Text;

namespace CodeToImageGenerator.Web.Services
{
    public class ImageService : IImageService
    {
        /// <summary>
        /// Создает изображение с полученным фрагментом кода и языком программирования
        /// </summary>
        /// <param name="language">Язык программирования</param>
        /// <param name="code">Фрагмент кода</param>
        /// <returns><see href="Stream"> с изображением</returns>
        public async Task<Stream> GenerateImageFromCodeAsync(string language, string code)
        {
            var imageStream = await Common.ImageGenerator.GenerateStream(language, code);

            return imageStream;
        }

        /// <summary>
        /// Генерирует случайное имя файла, содержащее буквы английского алфавита и цифры
        /// </summary>
        /// <param name="language">Язык программирования</param>
        /// <returns>Возвращает строку вида "{язык программирования}_{уникальное имя файла}.png"</returns>
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
