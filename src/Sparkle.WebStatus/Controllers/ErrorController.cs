
namespace Sparkle.WebStatus.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// This controller handles error pages as described at https://github.com/sandrock/SrkToolkit/blob/master/Wiki/SrkToolkit.Web-ResultAndErrors.md
    /// </summary>
    public class ErrorController : SrkToolkit.Web.HttpErrors.BaseErrorController
    {
        public ErrorController()
        {
        }

        public void Init(RequestContext requestContext)
        {
            this.Initialize(requestContext);
        }
    }
}