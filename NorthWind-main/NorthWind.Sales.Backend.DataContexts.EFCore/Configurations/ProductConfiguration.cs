using NorthWind.Sales.Backend.Repositories.Entities;

namespace NorthWind.Sales.Backend.DataContexts.EFCore.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(p => p.UnitPrice)
                .HasPrecision(8, 2);

            // NUEVO: Configuración para la URL de la imagen
            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);

            // DATOS SEMILLA ACTUALIZADOS
            builder.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Chai",
                    UnitPrice = 35,
                    UnitsInStock = 20,
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/ce/Masala_Chai.jpg/640px-Masala_Chai.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Chang",
                    UnitPrice = 55,
                    UnitsInStock = 0,
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/a/a7/Chang_beer_logo.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Aniseed Syrup",
                    UnitPrice = 65,
                    UnitsInStock = 20,
                    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR_u2J8i7qW_XzZ1KqY1w&s"
                },
                new Product
                {
                    Id = 4,
                    Name = "Chef Anton's Cajun Seasoning",
                    UnitPrice = 75,
                    UnitsInStock = 40,
                    ImageUrl = "https://m.media-amazon.com/images/I/81+M+D1+L._AC_SL1500_.jpg"
                },
                new Product
                {
                    Id = 5,
                    Name = "Chef Anton's Gumbo Mix",
                    UnitPrice = 50,
                    UnitsInStock = 20,
                    ImageUrl = "https://www.cajungrocer.com/media/catalog/product/cache/1/image/9df78eab33525d08d6e5fb8d27136e95/c/h/chef-antons-gumbo-mix.jpg"
                }
                // Agrega más productos si lo deseas
            );
        }
    }
}