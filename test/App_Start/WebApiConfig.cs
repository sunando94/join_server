using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;

namespace test
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();
          
            //config.Routes.MapHttpRoute(name: "DefaultApi",
            //    routeTemplate: "api/{Controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "help_ui_shortcut",
                routeTemplate: "help",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(SwaggerDocsConfig.DefaultRootUrlResolver, "help/ui/index")

                );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
        }
    }
}
