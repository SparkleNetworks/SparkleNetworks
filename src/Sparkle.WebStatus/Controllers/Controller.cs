
namespace Sparkle.WebStatus.Controllers
{
    using Sparkle.NetworksStatus.Domain.Cache;
    using Sparkle.WebStatus.Domain;
    using Sparkle.WebStatus.Services;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Base controller for the status website.
    /// </summary>
    public class Controller : System.Web.Mvc.Controller
    {
        private ServiceFactory services;
        private ServiceConfiguration servicesConfiguration;

        private WebStatus.Services.ResultService resultService;
        private NetworksStatus.Domain.ServicesConfiguration domainConfiguration;
        private NetworksStatus.Domain.ServiceFactory domain;

        public Controller()
        {
            this.domainConfiguration = NetworksStatus.Domain.ServicesConfiguration.FromAppConfig("WebStatus");
        }

        /// <summary>
        /// Get the entry point for the STATUS DOMAIN LAYER.
        /// </summary>
        public ServiceFactory Services
        {
            get
            {
                if (this.services == null)
                {
                    this.services = this.HttpContext.GetStatusLocalServices(false);
                }

                return this.services;
            }
        }

        public ResultService ResultService
        {
            get { return this.resultService = (this.resultService = new ResultService(this.HttpContext)); }
        }

        public NetworksStatus.Domain.IServiceFactory Domain
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.domain ?? (this.domain = new NetworksStatus.Domain.ServiceFactory(this.domainConfiguration, new AspnetDomainCacheProvider(this.HttpContext.Cache))); }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            this.servicesConfiguration = new ServiceConfiguration()
            {
                DataDirectory = this.Server.MapPath("~/App_Data/Services"),
                SmtpConfiguration = ConfigurationManager.AppSettings["SmtpConfiguration"],
            };

            if (!Directory.Exists(this.servicesConfiguration.DataDirectory))
            {
                Directory.CreateDirectory(this.servicesConfiguration.DataDirectory);
            }
        }

        protected override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            if (this.User != null && this.User.Identity != null && this.User.Identity.Name != null && this.User.Identity.Name.Length > 0)
            {
                this.ViewData["IsAuthenticated"] = true;
            }
            else
            {
                this.ViewData["IsAuthenticated"] = false;
            }

            base.OnResultExecuting(filterContext);
        }
    }
}
