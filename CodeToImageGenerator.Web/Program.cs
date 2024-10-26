using System.Net; // используется в Release сборке, не удалять!
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;

using CodeToImageGenerator.Web.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Используется в Release сборке, для отладки не нужно
#if !DEBUG
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 80);
    serverOptions.Listen(IPAddress.Any, 443);
});
#endif

var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
var webAppAddress = Environment.GetEnvironmentVariable("WEB_APP_URL");
var webHookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddSingleton<TelegramBotService>(
        sp => new TelegramBotService(
        sp.GetRequiredService<ILogger<TelegramBotService>>(),
        sp.GetRequiredService<IImageService>(),
        botToken,
        webAppAddress,
        webHookUrl
    ));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "TelegramWebApp",
    pattern: "{controller=Home}/{action=Index}/{initData}");

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request received: {context.Request.Path}");
    Console.WriteLine($"Request method: {context.Request.Method}");
    Console.WriteLine($"Content-Type: {context.Request.ContentType}");

    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    Console.WriteLine($"Request body: {body}");

    context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

    await next();

    Console.WriteLine($"Response status code: {context.Response.StatusCode}");
});

app.MapControllers();

var bot = app.Services.GetRequiredService<TelegramBotService>() as TelegramBotService;
await bot.StartAsync(new CancellationToken());

app.Run();


