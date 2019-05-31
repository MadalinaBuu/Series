﻿using SeriesApp.App_Start;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SeriesApp
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
