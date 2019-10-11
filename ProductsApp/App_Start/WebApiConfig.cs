using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Owin;

namespace ProductsApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "CategoryApi",
            //    routeTemplate: "api/{controller}/category/{category}",
            //    defaults: new { category  = RouteParameter.Optional }
            //);
        }
    }
}

