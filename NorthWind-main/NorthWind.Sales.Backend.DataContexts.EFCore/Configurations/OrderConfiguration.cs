

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // CAMBIO: Reemplazamos CustomerId por UserId
        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(40); // Longitud suficiente para un GUID de Identity

        /* ELIMINADO: Ya no usamos CustomerId ni la relación con Customer
        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasMaxLength(5)
            .IsFixedLength();
        
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId);
        */

        builder.Property(o => o.ShipAddress)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(o => o.ShipCity)
            .HasMaxLength(15);

        builder.Property(o => o.ShipCountry)
            .HasMaxLength(15);

        builder.Property(o => o.ShipPostalCode)
            .HasMaxLength(10);
    }
}