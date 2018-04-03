using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

           /*routes.MapRoute("event",
                "Community/{communityId}/{controller}/{eventId}",
                new { controller = "Event", action = "Get" }
                );

            routes.MapRoute("CommunityPost",
                "Community/Create",
                new { controller = "Community", action = "Create" }
            );

            routes.MapRoute("Community",
                 "Community/{id}", // URL with parameters
                 new { controller = "Community", action = "Get" }
                 
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           */
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
        );
           

            routes.MapRoute(name: "signin-google", url: "signin-google", defaults: new { controller = "Account", action = "LoginCallback" }); 
                           
            
                
        }
    }
}
