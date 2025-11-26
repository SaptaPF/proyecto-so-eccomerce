using DotNetEnv;
using Ecommerce.Mapping;
using Ecommerce.Models;
using Ecommerce.Persistence; // <-- El using BUENO
using Ecommerce.Repository.Interfaces;
using Ecommerce.Repository.Repositories;
using Ecommerce.Services.Implementation;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//variables de entorno dentro del docker
Env.Load("/app/myapp.env");
// --- 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DI) ---

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
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
// (Usa el DbContext que acabamos de registrar)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Esto deshabilita la necesidad de confirmar el email
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// 3. REGISTRA OTROS SERVICIOS DE FRAMEWORK
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. REGISTRA AutoMapper (Tu método manual, que funciona)
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

// 5. REGISTRA TUS PROPIOS SERVICIOS
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// --- 2. CONSTRUCCIÓN DE LA APP ---
var app = builder.Build(); // <-- Esta línea ya no dará error

// --- 5. SEMBRAR DATOS (Roles y Admin) ---
// Esto se ejecuta una vez al arrancar la app.
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

app.Run();