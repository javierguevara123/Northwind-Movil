using Microsoft.EntityFrameworkCore; // Necesario para métodos asíncronos de EF Core
using NorthWind.Sales.Backend.BusinessObjects.ValueObjects;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrderById;
using NorthWind.Sales.Entities.Dtos.Orders.GetOrders;
using NorthWind.Sales.Entities.Dtos.Products.GetProducts;

namespace NorthWind.Sales.Backend.Repositories.Repositories
{
    internal class QueriesRepository(INorthWindSalesQueriesDataContext context) : IQueriesRepository
    {
        // ========== PRODUCTS ==========

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var queryable = context.Products
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ImageUrl // <--- NUEVO: Mapeo de la imagen
                ));

            return await context.ToListAsync(queryable);
        }

        public async Task<bool> ProductExists(int productId)
        {
            var queryable = context.Products.Where(p => p.Id == productId);
            return await context.AnyAsync(queryable);
        }

        public async Task<short> GetCommittedUnits(int productId)
        {
            var queryable = context.OrderDetails
                .Where(od => od.ProductId == productId)
                .Select(od => (int)od.Quantity);

            var committedUnits = await context.SumAsync(queryable);
            return (short)committedUnits;
        }

        public async Task<ProductDto?> GetProductById(int productId)
        {
            var queryable = context.Products
                .Where(p => p.Id == productId)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ImageUrl // <--- NUEVO: Mapeo de la imagen
                ));

            return await context.FirstOrDefaultAync(queryable);
        }

        // ========== PRODUCTS CON PAGINACIÓN ==========

        public async Task<PagedResultDto<ProductDto>> GetProductsPaged(GetProductsQueryDto query)
        {
            // 1. Crear query base
            var queryable = context.Products.AsQueryable();

            // 2. Aplicar filtros
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                queryable = queryable.Where(p => p.Name.ToLower().Contains(searchTerm));
            }

            if (query.MinPrice.HasValue)
                queryable = queryable.Where(p => p.UnitPrice >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                queryable = queryable.Where(p => p.UnitPrice <= query.MaxPrice.Value);

            // Filtro de stock bajo (opcional, ajustado a lógica B2C si se desea)
            if (query.IsLowStock.HasValue && query.IsLowStock.Value)
                queryable = queryable.Where(p => p.UnitsInStock < 10);

            // 3. Obtener total de registros
            var totalCount = await context.CountAsync(queryable);

            // 4. Aplicar ordenamiento
            queryable = ApplyOrdering(queryable, query.OrderBy, query.OrderDescending);

            // 5. Aplicar paginación y proyección
            var pagedQuery = queryable
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductDto(
                    p.Id,
                    p.Name,
                    p.UnitsInStock,
                    p.UnitPrice,
                    p.ImageUrl // <--- NUEVO: Mapeo de la imagen en lista paginada
                ));

            // 6. Ejecutar query
            var items = await context.ToListAsync(pagedQuery);

            // 7. Retornar resultado paginado
            return new PagedResultDto<ProductDto>
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        private IQueryable<Backend.Repositories.Entities.Product> ApplyOrdering(
            IQueryable<Backend.Repositories.Entities.Product> queryable,
            string? orderBy,
            bool descending)
        {
            return orderBy?.ToLower() switch
            {
                "price" => descending
                    ? queryable.OrderByDescending(p => p.UnitPrice)
                    : queryable.OrderBy(p => p.UnitPrice),

                "stock" => descending
                    ? queryable.OrderByDescending(p => p.UnitsInStock)
                    : queryable.OrderBy(p => p.UnitsInStock),

                "name" or _ => descending
                    ? queryable.OrderByDescending(p => p.Name)
                    : queryable.OrderBy(p => p.Name)
            };
        }

        public async Task<bool> ProductNameExists(string name)
        {
            var queryable = context.Products
                .Where(p => p.Name.ToLower() == name.ToLower());

            return await context.AnyAsync(queryable);
        }

        public async Task<bool> ProductNameExists(string name, int excludeProductId)
        {
            var queryable = context.Products
                .Where(p => p.Name.ToLower() == name.ToLower() && p.Id != excludeProductId);

            return await context.AnyAsync(queryable);
        }

        // [ELIMINADO] GetCustomerCurrentBalance se borra porque Customer ya no existe.

        public async Task<IEnumerable<ProductUnitsInStock>> GetProductsUnitsInStock(IEnumerable<int> productIds)
        {
            var Queryable = context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductUnitsInStock(
            p.Id, p.UnitsInStock));
            return await context.ToListAsync(Queryable);
        }

        // ========== ORDERS ==========

        public async Task<OrderWithDetailsDto?> GetOrderById(int orderId)
        {
            // 1. Query principal - obtener datos de la orden
            // NOTA: Se eliminó el JOIN con Customer.
            var queryable =
                from order in context.Orders
                where order.Id == orderId
                select new
                {
                    order.Id,
                    order.UserId, // <--- CAMBIO: Usamos UserId
                    // CustomerName ya no existe en la tabla Customers.
                    // Podrías devolver el email del usuario si lo tienes, o dejarlo vacío
                    // y que el frontend muestre "Mi Pedido".
                    CustomerName = "Usuario Registrado",
                    order.OrderDate,
                    order.ShipAddress,
                    order.ShipCity,
                    order.ShipCountry,
                    order.ShipPostalCode
                };

            var orderData = await context.FirstOrDefaultAync(queryable);

            if (orderData == null)
                return null;

            // 2. Obtener los detalles
            var detailsQuery =
                from od in context.OrderDetails
                where od.OrderId == orderId
                join product in context.Products on od.ProductId equals product.Id
                select new OrderDetailItemDto(
                    od.ProductId,
                    product.Name,
                    od.Quantity,
                    od.UnitPrice,
                    od.Quantity * od.UnitPrice
                );

            var details = await context.ToListAsync(detailsQuery);

            // 3. Calcular totales
            decimal totalAmount = details.Sum(d => d.Subtotal);
            int itemCount = details.Count();

            // 4. Construir DTO final
            return new OrderWithDetailsDto(
                orderData.Id,
                orderData.UserId, // <--- UserId
                orderData.CustomerName,
                orderData.OrderDate,
                orderData.ShipAddress,
                orderData.ShipCity,
                orderData.ShipCountry,
                orderData.ShipPostalCode,
                totalAmount,
                itemCount,
                details
            );
        }

        public async Task<bool> OrderExists(int orderId)
        {
            var queryable = context.Orders.Where(o => o.Id == orderId);
            return await context.AnyAsync(queryable);
        }

        public async Task<OrderPagedResultDto> GetOrdersPaged(GetOrdersQueryDto query)
        {
            // 1. Query base con totales calculados
            // NOTA: Se eliminó el JOIN con Customer.
            var baseQuery =
                from order in context.Orders
                    // join customer ... [ELIMINADO]
                let totalAmount = context.OrderDetails
                    .Where(od => od.OrderId == order.Id)
                    .Sum(od => od.Quantity * od.UnitPrice)
                let itemCount = context.OrderDetails
                    .Where(od => od.OrderId == order.Id)
                    .Count()
                select new
                {
                    order.Id,
                    order.UserId, // <--- Usamos UserId
                    CustomerName = "Usuario", // Valor por defecto ya que no hay tabla Customers
                    order.OrderDate,
                    order.ShipCity,
                    order.ShipCountry,
                    TotalAmount = totalAmount,
                    ItemCount = itemCount
                };

            // 2. Aplicar filtros

            // IMPORTANTE: Filtrar por el Usuario Logueado (UserId)
            // Asumimos que el DTO GetOrdersQueryDto ahora tiene una propiedad UserId
            // o reutilizamos CustomerId para pasar el GUID del usuario.
            if (!string.IsNullOrWhiteSpace(query.CustomerId))
            {
                // Aquí query.CustomerId en realidad contiene el UserId del token
                baseQuery = baseQuery.Where(x => x.UserId == query.CustomerId);
            }

            if (query.FromDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.OrderDate >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.OrderDate <= query.ToDate.Value);
            }

            // 3. Contar total
            var totalCount = await context.CountAsync(baseQuery);

            // 4. Aplicar ordenamiento
            baseQuery = query.OrderBy?.ToLower() switch
            {
                // Ordenar por cliente ya no tiene mucho sentido en B2C para el usuario final,
                // pero si el admin lo usa, sería por UserId.
                "customer" => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.UserId)
                    : baseQuery.OrderBy(x => x.UserId),
                "amount" => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.TotalAmount)
                    : baseQuery.OrderBy(x => x.TotalAmount),
                "date" or _ => query.OrderDescending
                    ? baseQuery.OrderByDescending(x => x.OrderDate)
                    : baseQuery.OrderBy(x => x.OrderDate)
            };

            // 5. Aplicar paginación
            var pagedQuery = baseQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            // 6. Ejecutar y mapear
            var ordersData = await context.ToListAsync(pagedQuery);

            var items = ordersData.Select(o => new OrderListItemDto(
                o.Id,
                o.UserId, // <--- UserId
                o.CustomerName,
                o.OrderDate,
                o.ShipCity,
                o.ShipCountry,
                o.TotalAmount,
                o.ItemCount
            ));

            return new OrderPagedResultDto
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }
    }
}