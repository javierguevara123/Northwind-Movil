using NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts
{
    internal class NorthWindDomainLogsContext : DbContext
    {
        public NorthWindDomainLogsContext(DbContextOptions<NorthWindDomainLogsContext> options) : base(options) { }

        public DbSet<DomainLog> DomainLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANTE: No usar ApplyConfigurationsFromAssembly aquí,
            // porque cargaría las configuraciones de Order, Product, Customer, etc.

            // 1. Aplicar configuración específica para ErrorLogs
            modelBuilder.ApplyConfiguration(new ErrorLogConfiguration());

            // 2. La tabla DomainLogs se mapeará por convención (Entity Framework default)
            // ya que no tienes un archivo DomainLogConfiguration.cs específico.
            // Si quisieras personalizarla, lo harías así:
            /*
            modelBuilder.Entity<DomainLog>(entity => 
            {
                entity.HasKey(e => e.Id);
                // otras reglas...
            });
            */
        }
    }

}
