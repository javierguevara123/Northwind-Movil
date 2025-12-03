using Microsoft.EntityFrameworkCore.Design;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;
internal class NorthWindDomainLogsContextFactory : IDesignTimeDbContextFactory<NorthWindDomainLogsContext>
{
    public NorthWindDomainLogsContext CreateDbContext(string[] args)
    {
        IOptions<DBOptions> DbOptions =
        Microsoft.Extensions.Options.Options.Create(
        new DBOptions
        {
            DomainLogsConnectionString =
        "Data Source=JAVIER;Initial Catalog=NorthWindLogsDB;Integrated Security=True;Trust Server Certificate=True"
        });
        return new NorthWindDomainLogsContext(DbOptions);
    }
}