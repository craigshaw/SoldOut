using System.Web.Mvc;
using System.Web.Routing;

namespace SoldOutWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Search",
                url: "{action}/{id}",
                defaults: new { controller = "Search", action = "All", id = UrlParameter.Optional }
            );
        }
    }
}
