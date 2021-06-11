using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Data_Tier.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //This configures the WebApi to work as we want

            //This binds cutom HTTP attr paths in controllers to their funcs
            //We're not going to need this now but keep in for later
            config.MapHttpAttributeRoutes();

            //This binds API routes in the way we expect. You'll need to go to https:localhost:xxxx/api/whatever to get to the api
            //Note: The /api/ is critical
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}