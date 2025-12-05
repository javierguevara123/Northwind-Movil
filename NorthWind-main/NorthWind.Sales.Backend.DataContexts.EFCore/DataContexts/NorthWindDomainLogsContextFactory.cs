using Microsoft.EntityFrameworkCore.Design;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts
{
    internal class NorthWindDomainLogsContextFactory : IDesignTimeDbContextFactory<NorthWindDomainLogsContext>
    {
        public NorthWindDomainLogsContext CreateDbContext(string[] args)
        {
            // ESTA CADENA SE USA SOLO PARA CREAR LA MIGRACIÓN (Tiempo de diseño).
            // No afecta a la ejecución real de la API.
            var connectionString = "Data Source=JAVIER;Initial Catalog=NorthWindLogsDBM;Integrated Security=True;Trust Server Certificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<NorthWindDomainLogsContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new NorthWindDomainLogsContext(optionsBuilder.Options);
        }
    }
}