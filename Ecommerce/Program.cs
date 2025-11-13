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

// --- 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR DI) ---

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ***
// 1. REGISTRA EL DbContext (El BUENO, de Persistence)
// ***
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.MigrationsAssembly("Ecommerce.Persistence");
        }
    );
});

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
});

// 5. REGISTRA TUS PROPIOS SERVICIOS
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICarritoService, CarritoService>();

// --- 2. CONSTRUCCIÓN DE LA APP ---
var app = builder.Build(); // <-- Esta línea ya no dará error

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();