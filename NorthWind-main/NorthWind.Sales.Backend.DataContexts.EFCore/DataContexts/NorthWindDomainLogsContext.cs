using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts
{
    internal class NorthWindDomainLogsContext : DbContext
    {
        // Constructor existente...
        public NorthWindDomainLogsContext(DbContextOptions<NorthWindDomainLogsContext> options) : base(options) { }

        public DbSet<DomainLog> DomainLogs { get; set; }

        // NUEVO DBSET
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Esto aplicará automáticamente la configuración de ErrorLogConfiguration 
            // si está en el mismo ensamblado.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Opcional: Si quieres forzar solo las de logs aquí, puedes instanciarlas manualmente:
            // modelBuilder.ApplyConfiguration(new Configurations.DomainLogConfiguration());
            // modelBuilder.ApplyConfiguration(new Configurations.ErrorLogConfiguration());
        }
    }

}
