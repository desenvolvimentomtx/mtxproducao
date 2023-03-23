using System.Web.Mvc;
using System.Web.Routing;

namespace MatrizTributaria
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] {"MatrizTributaria.Controllers"}
            );

            routes.MapRoute(
            name:"cliente",
            url:"Cliente",
            defaults: new { controller = "HomeCliente", action = "HomeCliente", id = UrlParameter.Optional }
            
);

        }
    }
}
