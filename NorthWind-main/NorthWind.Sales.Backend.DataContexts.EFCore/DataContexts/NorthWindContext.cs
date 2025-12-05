
using NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;
using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.DataContexts;

internal class NorthWindContext : DbContext
{
    protected override void OnConfiguring(
   DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
        "Data Source=JAVIER;Initial Catalog=NorthWindDBM;Integrated Security=True;Trust Server Certificate=True");
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ELIMINAR O COMENTAR ESTA LÍNEA:
        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // ^ Esto es lo que causaba que ErrorLogs apareciera aquí.

        // SOLUCIÓN: Aplicar manualmente solo las configuraciones de negocio.
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}