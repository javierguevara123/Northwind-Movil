using NorthWind.Sales.Backend.Controllers.Logs;

namespace Microsoft.AspNetCore.Builder;
public static class EndpointsContainer
{
    public static WebApplication MapNorthWindSalesEndpoints(
   this WebApplication app)
    {
        app.UseOrdersController();
        app.UseMembershipEndpoints();
        app.UseProductsController();
        app.UseLogsController();
        app.MapControllers();
        return app;
    }
}
