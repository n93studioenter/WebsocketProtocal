using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebsocketProtocal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "WebSocket",
                url: "WebSocket/{action}/{id}",
                defaults: new { controller = "WebSocket", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ClientManagement",
                url: "ClientManagement/{action}/{id}",
                defaults: new { controller = "ClientManagement", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
