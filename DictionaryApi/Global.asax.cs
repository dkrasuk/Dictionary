using DictionaryApi.Configuration;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Mvc;
using DictionaryApi.App_Start;

namespace DictionaryApi
{
    /// <summary>
    /// WebApiApplication
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application_Start
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutofacConfig.AutofacRegister();

            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
