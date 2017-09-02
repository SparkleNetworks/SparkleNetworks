
namespace Sparkle.WebStatus.App_Start
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;

    public static class LocalConfig
    {
        public static string SparkleLangDirectory
        {
            get { return ConfigurationManager.AppSettings["SparkleLangDirectory"]; }
        }

        public static string EmailsInboundDirectory
        {
            get { return ConfigurationManager.AppSettings["EmailsInboundLogDirectory"]; }
        }

        public static bool HandleInboundEmails
        {
            get
            {
                bool value;
                var config = ConfigurationManager.AppSettings["HandleInboundEmails"];
                if (config == null || !bool.TryParse(config, out value))
                    value = false;
                return value;
            }
        }
    }
}