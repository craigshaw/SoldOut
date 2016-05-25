using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace SoldOutWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "api/{controller}/{id}"
            );
            
        }
    }
}
