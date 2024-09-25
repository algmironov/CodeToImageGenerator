using System.Web;

using Newtonsoft.Json.Linq;

using PuppeteerSharp;

namespace CodeToImageGenerator.Common
{
    /// <summary>
    /// Класс, в котором преобразуется текстовый фрагмент кода в изображение
    /// </summary>
    public class ImageGenerator
    {
        /// <summary>
        /// Превращает строку с фрагментом кода в скриншот с подсветкой синтаксиса
        /// </summary>
        /// <param name="lang">Язык программирования</param>
        /// <param name="code">Фрагмент кода</param>
        /// <returns>Stream с массивом байтов для отправки или загрузки изображения</returns>
        public async static Task<Stream> GenerateStream(string lang, string code)
        {
            var finalHtml = PrepareHTML(lang, code);

#if DEBUG
            await new BrowserFetcher().DownloadAsync();

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
#else
            var chromiumExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromiumExecutablePath, 
                Args =[
                    "--no-sandbox", 
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage", 
                    "--disable-gpu"]
            });
#endif
            using var page = await browser.NewPageAsync();

            await page.SetContentAsync(finalHtml);

            var dimensions = await page.EvaluateFunctionAsync<string>(@"
        () => {
            const container = document.querySelector('.container');
            const width = container.offsetWidth;
            const height = container.offsetHeight;
            return JSON.stringify({width, height});
        }
    ");

            try
            {
                var dimensionsObj = JObject.Parse(dimensions);
                int width = dimensionsObj["width"].Value<int>();
                int height = dimensionsObj["height"].Value<int>();

                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = width,
                    Height = height
                });

                var screenshotTask = page.ScreenshotStreamAsync(new ScreenshotOptions { FullPage = false, OptimizeForSpeed = true });
                var screenshot = await screenshotTask;

                Console.WriteLine("Изображение с кодом успешно создано");
                
                return screenshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpeced exception during screenshot creation: {ex.Message}");
                return null;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }

        /// <summary>
        /// Превращает строку с фрагментом кода в изображение, сохраняет его в формате .png и возвращает имя файла
        /// </summary>
        /// <param name="lang">Язык программирования</param>
        /// <param name="code">Фрагмент кода</param>
        /// <returns>Имя файла с изображением</returns>
        [Obsolete("Метод сохраняет изображение локально, что в данном приложении не используется")]
        public async static Task<string> GenerateImage(string lang, string code)
        {
            var finalHtml = PrepareHTML(lang, code);

            var chromiumExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromiumExecutablePath, 
                Args =[
                    "--no-sandbox", 
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage", 
                    "--disable-gpu"]
            });

            using var page = await browser.NewPageAsync();

            await page.SetContentAsync(finalHtml);

            var dimensions = await page.EvaluateFunctionAsync<string>(@"
        () => {
            const container = document.querySelector('.container');
            const width = container.offsetWidth;
            const height = container.offsetHeight;
            return JSON.stringify({width, height});
        }
    ");

            try
            {
                var dimensionsObj = JObject.Parse(dimensions);
                int width = dimensionsObj["width"].Value<int>();
                int height = dimensionsObj["height"].Value<int>();

                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = width,
                    Height = height
                });

                var imageName = "code.png";

                await page.ScreenshotAsync(imageName, new ScreenshotOptions { FullPage = false, OptimizeForSpeed = true });

                Console.WriteLine("Изображение с кодом успешно создано");
                return imageName;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }

        private static string PrepareHTML(string language, string code)
        {
            var htmlTemplate = File.ReadAllText("code.html");

            var escapedCode = HttpUtility.HtmlEncode(code);

            var finalHtml = htmlTemplate
                .Replace("{language}", language)
                .Replace("{code}", escapedCode);

            return finalHtml;
        }
    }
}
