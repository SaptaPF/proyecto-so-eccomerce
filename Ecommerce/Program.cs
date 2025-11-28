using DotNetEnv;
using Ecommerce.Mapping;
using Ecommerce.Models;
using Ecommerce.Persistence; 
using Ecommerce.Repository.Interfaces;
using Ecommerce.Repository.Repositories;
using Ecommerce.Services.Implementation;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//HOLA SOY ABDAL Kappa
var builder = WebApplication.CreateBuilder(args);
//variables de entorno dentro del docker
Env.Load("/app/myapp.env");
// --- 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DI) ---

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "3.143.232.22";
//Aqui se va a editar la conexion con la base de datos, ya no en appsettings.json
var connectionString = $"server={dbHost};database=EcommerceDB;user=WebAdmin;password=admin;";

Console.WriteLine($"[DEBUG] DB_HOST env var = '{dbHost}'");
Console.WriteLine($"[DEBUG] Connection string = '{connectionString}'");


// If you use EF Core:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.MigrationsAssembly("Ecommerce.Persistence");
        }
        ));

// 2. REGISTRA IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ProductoProfile>();
    config.AddProfile<CategoriaProfile>();
    config.AddProfile<ResenaProfile>();
    config.AddProfile<ItemCarritoProfile>();
    config.AddProfile<CarritoProfile>();
    config.AddProfile<DireccionProfile>();
    config.AddProfile<ProductoUpsertProfile>();
    config.AddProfile<CategoriaUpsertProfile>();
    config.AddProfile<PedidoAdminProfile>();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();


var app = builder.Build(); 

// --- 5. SEMBRAR DATOS (Roles y Admin) ---

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Llamamos a nuestro seeder estático
        await RoleSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Opcional: loggear el error si el seeder falla
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante el sembrado de datos (seeding).");
    }
}
// --- 3. CONFIGURACIÓN DEL PIPELINE DE HTTP ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var defaultCulture = new System.Globalization.CultureInfo("en-US"); 
defaultCulture.NumberFormat.CurrencySymbol = "$"; 
defaultCulture.NumberFormat.CurrencyNegativePattern = 1; 

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new List<System.Globalization.CultureInfo> { defaultCulture },
    SupportedUICultures = new List<System.Globalization.CultureInfo> { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);
app.Run();
