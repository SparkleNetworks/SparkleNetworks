
namespace Sparkle.NetworksStatus.Domain
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Mail;

    partial class ServicesConfiguration
    {
        public ServicesConfiguration()
        {
        }

        public string DatabaseConnectionString { get; set; }

        public string AppName { get; set; }

        public string AppUrl { get; set; }

        public string AppCompanyName { get; set; }

        public string AppCompanyLegalName { get; set; }

        public string SupportEmail { get; set; }

        public string EmailSenderDisplayName { get; set; }

        public string EmailSenderEmail { get; set; }

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpSsl { get; set; }

        public static ServicesConfiguration FromAppConfig(string nameSpace)
        {
            if (!string.IsNullOrEmpty(nameSpace))
                nameSpace += ".";

            var obj = new ServicesConfiguration();
            obj.DatabaseConnectionString = ConfigurationManager.AppSettings[nameSpace + "DatabaseConnectionString"];

            return obj;
        }

        public static ServicesConfiguration FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ServicesConfiguration>(json);
        }

        internal MailAddress GetSenderMailAddress()
        {
            return new MailAddress(this.EmailSenderEmail, this.EmailSenderDisplayName);
        }

        internal Uri GetSiteUrl(string pathAndQuery)
        {
            if (pathAndQuery.Length > 0 && pathAndQuery[0] == '/')
                pathAndQuery = pathAndQuery.Substring(1);

            var root = this.AppUrl;
            if (root.Length > 0 && root[root.Length - 1] != '/')
                root += "/";

            var uri = new Uri(root + pathAndQuery, UriKind.Absolute);
            return uri;
        }
    }
}
