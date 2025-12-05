
namespace Microsoft.AspNetCore.Builder;
public static class EndpointsContainer
{
    public static WebApplication MapNorthWindSalesEndpoints(
   this WebApplication app)
    {
        app.UseOrdersController();
        app.UseMembershipEndpoints();
        app.UseProductsController();

        return app;
    }
}
