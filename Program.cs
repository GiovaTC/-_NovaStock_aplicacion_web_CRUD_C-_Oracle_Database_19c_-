
using CrudProductosOracle19c.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios MVC
builder.Services.AddControllersWithViews();

// Registrar el repositorio de productos Oracle
builder.Services.AddScoped<ProductoRepository>();

var app = builder.Build();

// Configuración del manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirigir HTTP a HTTPS
app.UseHttpsRedirection();

// Habilitar archivos estáticos: CSS, JavaScript e imágenes
app.UseStaticFiles();

// Habilitar el enrutamiento
app.UseRouting();

// Configurar la ruta principal de NovaStock
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

// Iniciar la aplicación
app.Run();
    