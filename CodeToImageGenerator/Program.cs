using CodeToImageGenerator.Common;

namespace CodeToImageGenerator.Demo
{
    /// <summary>
    /// Класс для простой демонстрации возможностей генератора изображений из кода.
    /// 
    /// Просто вставляете фрагмент кода вместо  {place your code snippet here} и язык программирования вместо {csharp} и запускаете приложение.
    /// 
    /// В папке ~/bin/Debug/net8.0 будет сохранен файл с изображением.
    /// </summary>
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            string code = "place your code snippet here";

            string programmingLanguage = "csharp";

            await ImageGenerator.GenerateImage(code, programmingLanguage);
        }
    }
}
