using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SocialNetwork.Core.Dependency;
using System.Web;
using SocialNetwork.Web.Controllers;

namespace SocialNetwork.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            logger.Info(DateTime.Now + ": старт приложения.");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(NinjectBindings.Instance);

        }

        protected void Application_Error()
        {
            Exception exc = Server.GetLastError();
            logger.Error(DateTime.Now + ": Ошибка! ");

            if (exc is HttpException)
            {
                logger.Info(": FF ");
                RedirectToErrorPage((HttpException)exc);
            }

            Server.ClearError();
        }

        private void RedirectToErrorPage(HttpException ex)
        {
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);
            switch (ex.GetHttpCode())
            {
                case 401:
                    routeData.Values.Add("action", "ErrorCode401");
                    break;
                case 402:
                    routeData.Values.Add("action", "ErrorCode402");
                    break;
                case 403:
                    routeData.Values.Add("action", "ErrorCode403");
                    break;
                case 404:
                    logger.Info(": Ошибка404! ");
                    routeData.Values.Add("action", "ErrorCode404");
                    break;
                case 500:
                    routeData.Values.Add("action", "ErrorCode500");
                    break;
                default:
                    routeData.Values.Add("action", "ErrorCode500");
                    break;
            }

            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        protected void Application_End()
        {
            logger.Info(DateTime.Now + ": завершение работы приложения.");
        }

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    }
}