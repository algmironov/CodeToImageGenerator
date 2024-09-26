namespace CodeToImageGenerator.Web.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Создает изображение с полученным фрагментом кода и языком программирования
        /// </summary>
        /// <param name="language"></param>
        /// <param name="code"></param>
        /// <returns><see href="Stream"> с изображением</returns>
        Task<Stream> GenerateImageFromCodeAsync(string language, string code);

        /// <summary>
        /// Генерирует случайное имя файла, содержащее буквы английского алфавита и цифры
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Возвращает строку вида "{язык программирования}_{уникальное имя файла}.png"</returns>
        string GenerateFileName(string language);
    }
}
