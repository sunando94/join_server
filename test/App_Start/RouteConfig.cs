using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace test
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                
            );
           
            //routes.MapRoute(
            //    name: "help_ui_shortcut",
            //    routeTemplate: "help",
            //    defaults: null,
            //    constraints: null,
            //    handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "help/ui/index")

            //    );
        }
    }
}
