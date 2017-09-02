
namespace Sparkle.WebStatus
{
    using Sparkle.WebBase;
    using Sparkle.WebStatus.Controllers;
    using SrkToolkit.Web.HttpErrors;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcApplication : BaseMvcApplication
    {
        private DateTime? appStartTimeUtc = DateTime.UtcNow;
#if DEBUG
        bool debug = true;
#else
        bool debug = false;
#endif

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ErrorControllerHandler.Register<ErrorController>(this, debug);
        }

        protected void Application_Error()
        {
            var errorController = new ErrorController();
            var context = new HttpContextWrapper(this.Context);
            errorController.Init(new RequestContext(context, new RouteData()));
            var assembly = typeof(MvcApplication).Assembly;

            Exception exception = null;
            try
            {
                exception = ErrorControllerHandler.Handle(this.Context, errorController, debug);
            }
            catch (Exception ex)
            {
                if (exception == null)
                    exception = Server.GetLastError();

                HttpErrorReport.Do(this.Context, false, new Exception[] { exception, ex, }, appStartTimeUtc.Value, assembly, default(string));
                BasicHttpErrorResponse.Execute(this.Context, exception, extraError: ex, message: "Warning: The error page execution failed on top of another exception.");
            }

            if (exception != null)
            {
                Trace.TraceError(DateTime.UtcNow.ToString("o") + " Application_Error: " + exception.ToString());
            }

            if (!debug && exception != null && !this.IsHttpErrorToBeIgnored(this.Context, exception))
            {
                HttpErrorReport.Do(context, false, appStartTimeUtc.Value, assembly);
            }
        }
    }
}