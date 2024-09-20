using System.Web;

using Newtonsoft.Json.Linq;

using PuppeteerSharp;

namespace CodeToImageGenerator.Common
{
    public class ImageGenerator
    {
        public async static Task<Stream> GenerateStream(string lang, string code)
        {
            var htmlTemplate = File.ReadAllText("code.html");

            var escapedCode = HttpUtility.HtmlEncode(code);

            var finalHtml = htmlTemplate
                .Replace("{language}", lang)
                .Replace("{code}", escapedCode);

            await new BrowserFetcher().DownloadAsync();
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
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
            catch (Exception)
            {
                return null;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }

        public async static Task<string> GenerateImage(string lang, string code)
        {
            var htmlTemplate = File.ReadAllText("code.html");

            var escapedCode = HttpUtility.HtmlEncode(code);

            var finalHtml = htmlTemplate
                .Replace("{language}", lang)
                .Replace("{code}", escapedCode);

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
    }
}
