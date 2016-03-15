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

            routes.MapRoute(
              name: "PriceHistory",
              url: "PriceHistory/{id}/{conditionId}",
              defaults: new { controller = "Search", action = "PriceHistoryByCondition", id = UrlParameter.Optional, conditionId = UrlParameter.Optional }
);
        }
    }
}
