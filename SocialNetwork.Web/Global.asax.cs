using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SocialNetwork.Core.Dependency;
using System.Web;

namespace SocialNetwork.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            logger.Info("Старт приложения.");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(NinjectBindings.Instance);

        }

        protected void Application_Error()
        {
            var exc = Server.GetLastError();
            var statusCode = exc is HttpException ? (exc as HttpException).GetHttpCode() : 500 /*Internal Server Error*/;
            if (statusCode == 500)
            {
                logger.Error("Ошибка! " + exc);
            }
            Response.Redirect($"/Error/ErrorCode{statusCode}", true);
        }

        protected void Application_End()
        {
            logger.Info("Завершение работы приложения.");
        }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    }
}