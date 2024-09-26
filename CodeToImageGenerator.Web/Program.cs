using System.Net; // используется в Release сборке, не удалять!

using CodeToImageGenerator.Web.Middleware;
using CodeToImageGenerator.Web.Services;

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

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddSingleton<TelegramBotService>(
        sp => new TelegramBotService(
        sp.GetRequiredService<ILogger<TelegramBotService>>(),
        sp.GetRequiredService<IImageService>(),
        botToken,
        webAppAddress
    ));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

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

app.UseTelegramMiniAppMiddleware();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "TelegramWebApp",
    pattern: "{controller=Home}/{action=Index}/{initData}");

app.MapControllers();

var bot = app.Services.GetRequiredService<TelegramBotService>() as TelegramBotService;
await bot.StartAsync(new CancellationToken());

app.Run();


