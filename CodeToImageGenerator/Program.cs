using System.Web;

using Newtonsoft.Json.Linq;

using PuppeteerSharp;

namespace CodeToImageGenerator
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            string htmlTemplate = File.ReadAllText("code.html");

            string code = @"
public static void Rename(string path)
    {
        int num = 1;
        var files = Directory.GetFiles(path).ToList();
        string newFileName = string.Empty;

        // int num = files.Count;
        try { 
        foreach (var file in files)
        {
            string fileName = Path.GetFileName(file);
            string fileExtension = Path.GetExtension(file);
            if (fileExtension != ""null"" && (fileExtension == "".jpg"" || fileExtension == "".JPG""))
            {
                if (num < 10)
                {
                    newFileName = $""00000{num}{fileExtension}"";
                    string newFilePath = Path.Combine(path, newFileName);
                    File.Move(file, newFilePath);
                }
                else if (num > 9 && num < 100)
                {
                    newFileName = $""0000{num}{fileExtension}"";
                    string newFilePath = Path.Combine(path, newFileName);
                    File.Move(file, newFilePath);
                }
                else if (num > 99 && num < 1000)
                {
                    newFileName = $""000{num}{fileExtension}"";
                    string newFilePath = Path.Combine(path, newFileName);
                    File.Move(file, newFilePath);
                }
                else if (num > 999 && num < 10000)
                {
                    newFileName = $""00{num}{fileExtension}"";
                    string newFilePath = Path.Combine(path, newFileName);
                    File.Move(file, newFilePath);
                }
                num++;
            }

        }
        } catch (Exception ex)
        {
            Console.WriteLine(""Произошла ошибка: "" + ex.Message);
        }

        Console.WriteLine(""Все файлы успешно переименованы."");
    }";

            string programmingLanguage = "csharp";

            string escapedCode = HttpUtility.HtmlEncode(code);

            string finalHtml = htmlTemplate
                .Replace("{language}", programmingLanguage)
                .Replace("{code}", escapedCode);

            File.WriteAllText("output.html", finalHtml);

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

            var dimensionsObj = JObject.Parse(dimensions);

            int width = dimensionsObj["width"].Value<int>();
            int height = dimensionsObj["height"].Value<int>();

            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = width,
                Height = height
            });

            await page.ScreenshotAsync("code_screenshot.png", new ScreenshotOptions { FullPage = false, OptimizeForSpeed = true });

            var screenshot = await page.ScreenshotStreamAsync(new ScreenshotOptions { FullPage = false, OptimizeForSpeed = true });

            Console.WriteLine("Изображение с кодом успешно создано");
            await browser.CloseAsync();
        }
    }
}
