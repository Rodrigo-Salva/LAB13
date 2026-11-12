using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio.Mapeos;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VeterinariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VeterinariaDb")));

builder.Services.AddScoped<IDuenoRepositorio, DuenoRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();
builder.Services.AddScoped<IDuenoServicio, DuenoServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "Veterinaria API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
