
namespace Sparkle.WebStatus.Areas.NetworkRootApi
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class NetworkRootApiAreaRegistration : AreaRegistration
    {
        public const string BasePath = "NetworkRootApi";

        public override string AreaName
        {
            get { return "NetworkRootApi"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
        }
    }
}
