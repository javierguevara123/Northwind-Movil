using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Northwind.Sales.WebApi.Extensions;
using NorthWind.Membership.Backend.AspNetIdentity.Options;
using NorthWind.Membership.Backend.AspNetIdentity.Services;
using NorthWind.Membership.Backend.Core.Middleware;
using NorthWind.Membership.Backend.Core.Options;
using NorthWind.Sales.Backend.DataContexts.EFCore.Options;
using NorthWind.Sales.Backend.IoC;
using NorthWind.Sales.Backend.SmtpGateways.Options;
using NorthWind.Sales.WebApi;
using System.Text;

namespace Northwind.Sales.WebApi;

// Esto expone 2 metodos de extension para configurar los servicios web
// y agregar los middlewares y endpoints de la Web API

internal static class Startup
{
    //  Agregar soporte para documentación Swagger.
    public static WebApplication CreateWebApplication(this WebApplicationBuilder builder)
    {

        // Esto registra los servicios necesarios para generar la documentación automática Swagger de la API.
        // Configurar APIExplorer para descubrir y exponer los metadatos de los endpoints de la aplicación.    
        builder.Services.AddEndpointsApiExplorer();

        //  Habilita la documentación de la API.
        builder.Services.AddSwaggerGenBearer();

        //  Registrar servicios con Inyección de Dependencias.
        //  Registrar los servicios de la aplicación.
        //  Esto utiliza el contenedor de IoC (DependencyContainer) para registrar todas las dependencias
        //  del dominio NorthWind Sales, incluyendo:
        //  Use Cases, Repositories, Data Contexts, Presenters.
        //  Aquí "DBOptions" representa un objeto que contiene el "ConnectionString" y se carga
        //  desde "appsettings.json".
        builder.Services.AddNorthWindSalesServices(dbObtions =>
            builder.Configuration.GetSection(DBOptions.SectionKey).Bind(dbObtions),
            smtpOptions => builder.Configuration.GetSection(SmtpOptions.SectionKey).Bind(smtpOptions),
            membershipDBOptions => builder.Configuration.GetSection(MembershipDBOptions.SectionKey).Bind(membershipDBOptions),
            jwtOptions => builder.Configuration.GetSection(JwtOptions.SectionKey).Bind(jwtOptions)
        );

        //  Configurar CORS.
        //  Esto permite que cualquier cliente (como un frontend en Angular, React o Blazor WebAssembly)
        //  pueda consumir la API sin restricciones de origen, método o cabecera.
        //  Habilita el acceso desde otros dominios (útil para frontend).
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(config =>
            {
                config.AllowAnyMethod();
                config.AllowAnyHeader();
                config.AllowAnyOrigin();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
       {
     // Establecer la configuración del Token.
     builder.Configuration.GetSection(JwtOptions.SectionKey)
     .Bind(options.TokenValidationParameters);
     // Establecer la llave para validación de la firma.
     string SecurityKey = builder.Configuration
     .GetSection(JwtOptions.SectionKey)[nameof(JwtOptions.SecurityKey)];
     byte[] SecurityKeyBytes = Encoding.UTF8.GetBytes(SecurityKey);
     options.TokenValidationParameters.IssuerSigningKey =
     new SymmetricSecurityKey(SecurityKeyBytes);
 });

        builder.Services.AddAuthorization();


        //  Construye la instancia "WebApplication" con todos los servicios configurados.
        return builder.Build();
    }

    //  Este método se encarga de:
    //  -Habilitar Swagger solo en desarrollo
    //  -Mapear los endpoints de la aplicación
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.UseExceptionHandler(builder => { });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Inicializar roles y SuperUser
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                RoleSeeder.SeedRolesAndSuperUser(services).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error al inicializar roles y SuperUser");
            }
        }

        app.MapNorthWindSalesEndpoints();
        app.UseCors();
        app.UseAuthentication();
        app.UseMiddleware<TokenBlacklistMiddleware>();
        app.UseAuthorization();

        return app;
    }
}
