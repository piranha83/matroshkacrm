using System.Linq;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Web.Configurations
{
    public class DynamicConfiguration
    {
        public class Order { public int Id { get; set; } }
        public static void AddOrderRoute(IRouteBuilder config)
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Order>("Orders1");

            var route = config.Routes.First(r => r is Microsoft.AspNet.OData.Routing.ODataRoute);
            //var odataRoute = route as System.Web.OData.Routing.ODataRoute;
            var odataRoute = route as Microsoft.AspNet.OData.Routing.ODataRoute;

            config.MapODataServiceRoute(
                "odata2",
                "v{version:apiVersion}",
                builder.GetEdmModel()
                );//,
                //null, odataRoute.PathRouteConstraint);
                //pathHandler: odataRoute.PathRouteConstraint.PathHandler,
                //routingConventions:  odataRoute.PathRouteConstraint.RoutingConventions);
            
        }
    }
}