
using Microsoft.EntityFrameworkCore;
using PlayzoneApi.Middlewares;
using PlayzoneApi.Models;
using PlayzoneApi.Repositories;
using PlayzoneApi.Repositories.Interfaces;

namespace PlayzoneApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configuración de DbContext con SQL Server
            builder.Services.AddDbContext<PlayZoneContext>(o =>
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Registro de Repositorios (Inyección de Dependencias)
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
            builder.Services.AddScoped<IVentaRepository, VentaRepository>();
            builder.Services.AddScoped<ISoporteRepository, SoporteRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 3. Registrar Middleware Global de Excepciones (debe ir antes de los controladores)
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
