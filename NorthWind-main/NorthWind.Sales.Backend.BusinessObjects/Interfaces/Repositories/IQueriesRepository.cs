using NorthWind.Sales.Backend.BusinessObjects.ValueObjects;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.BusinessObjects.Interfaces.Repositories
{
    public interface IQueriesRepository
    {
        // ========== PRODUCTS ==========
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProductById(int productId);
        Task<bool> ProductExists(int productId);
        Task<PagedResultDto<ProductDto>> GetProductsPaged(GetProductsQueryDto query);
        Task<short> GetCommittedUnits(int productId);
        Task<bool> ProductNameExists(string name, int excludeProductId);
        Task<bool> ProductNameExists(string name);

        // ========== HELPERS ==========
        Task<IEnumerable<ProductUnitsInStock>> GetProductsUnitsInStock(IEnumerable<int> productIds);

        // ========== ORDERS ==========

        // Obtiene las órdenes paginadas (filtradas por UserId dentro del query DTO)
        Task<OrderPagedResultDto> GetOrdersPaged(GetOrdersQueryDto query);

        /// <summary>
        /// Obtiene una orden por ID con todos sus detalles.
        /// </summary>
        Task<OrderWithDetailsDto?> GetOrderById(int orderId);

        /// <summary>
        /// Verifica si una orden existe por su ID.
        /// </summary>
        Task<bool> OrderExists(int orderId);

        // ELIMINADO: Task<decimal?> GetCustomerCurrentBalance(string customerId);
    }
}