using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts
{
    internal class NorthWindDomainLogsContextFactory : IDesignTimeDbContextFactory<NorthWindDomainLogsContext>
    {
        public NorthWindDomainLogsContext CreateDbContext(string[] args)
        {
            // 1. Construir la configuración para leer appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Obtener la cadena de conexión
            // Asegúrate de que esta clave coincida con tu appsettings (ej. DBOptions:DomainLogsConnectionString)
            var connectionString = configuration.GetSection("DBOptions:DomainLogsConnectionString").Value;

            // 3. Construir las opciones del DbContext
            var optionsBuilder = new DbContextOptionsBuilder<NorthWindDomainLogsContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 4. Crear el contexto pasando las opciones correctas
            return new NorthWindDomainLogsContext(optionsBuilder.Options);
        }
    }
}