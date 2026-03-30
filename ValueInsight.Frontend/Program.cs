using Microsoft.AspNetCore.Localization;
using System.Globalization;
using ValueInsight.Frontend; // Se till att detta matchar din namespace för SharedResource

var builder = WebApplication.CreateBuilder(args);

// --- 1. LÄGG TILL LOKALISERING-SERVICES ---
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => {
        // Detta gör att dina ViewModels (Login/Register) hittar texterna i SharedResource
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// --- 2. KONFIGURERA SPRÅK-MIDDLEWARE ---
// Detta måste ligga INNAN app.UseRouting() för att fungera bäst
var supportedCultures = new[] {
    new CultureInfo("sv"),
    new CultureInfo("en")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("sv"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Standard-konfiguration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();