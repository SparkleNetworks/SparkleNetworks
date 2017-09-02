
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseService
    {
        private readonly ServiceFactory factory;
        private readonly string serviceName;

        protected BaseService(ServiceFactory factory, string serviceName)
        {
            this.factory = factory;
            this.serviceName = serviceName;
        }

        protected ServiceFactory Services
        {
            get { return this.factory; }
        }
    }
}
