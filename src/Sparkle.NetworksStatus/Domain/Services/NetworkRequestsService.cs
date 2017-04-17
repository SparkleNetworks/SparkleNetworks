
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Domain.Lang;
    using Sparkle.NetworksStatus.Domain.Models;
    using Sparkle.NetworksStatus.Internals;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;

    partial class NetworkRequestsService : INetworkRequestsService
    {
        private static readonly string[] forbiddenSubdomainNames = new string[]
        {
            "root", "bin", "daemon", "adm", "lp", "sync", "shutdown", "halt", "news", "uucp", "games", "operator", "gopher", "ftp", "rpm", "vcsa", "nscd", "sshd", "rpc", "rpcuser", "nfsnobody", "mailnull", "smmsp", "pcap", "xfs", "ntp", "desktop", "apache", "webalizer", "squid", "postfix", "named", "netdump", "mysql", "admin", "cpanel", "mailman", "mail", "httpd", "nobody", "administrator",
            "support", "help", "forum", "api",
            "sparkle",
        };
        private static readonly MailAddress[] replyTo = new MailAddress[]
        {
            new MailAddress("antoine@sparklenetworks.com", "Antoine Sottiau"),
            new MailAddress("philippe@sparklenetworks.com", "Philippe Lebas"),
            new MailAddress("kevin@sparklenetworks.com", "Kevin Alexandre"),
        };

        public NetworkRequestModel GetCreateRequest(NetworkRequestModel model)
        {
            if (model == null)
            {
                model = new NetworkRequestModel();
            }
            else
            {
                if (model.ContactPhoneNumber != null)
                {
                    model.ContactPhoneNumber = Validate.PhoneNumber(model.ContactPhoneNumber) ?? model.ContactPhoneNumber;
                }
            }

            if (model.AvailableCountries == null)
            {
                model.AvailableCountries = this.GetCountriesList(false);
            }

            return model;
        }

        public BasicResult<CreateNetworkRequestError, NetworkRequestModel> Create(NetworkRequestModel request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new BasicResult<CreateNetworkRequestError, NetworkRequestModel>();

            if (string.IsNullOrEmpty(request.NetworkSubdomain))
            {
                result.Errors.Add(CreateNetworkRequestError.VerifyNetworkSubdomain, DomainStrings.ResourceManager);

                if (!string.IsNullOrEmpty(request.NetworkName))
                {
                    request.NetworkSubdomain = request.NetworkName.MakeUrlFriendly(false).Trim();
                }
            }
            else
            {
                var oldValue = request.NetworkSubdomain;
                request.NetworkSubdomain = request.NetworkSubdomain.MakeUrlFriendly(false).Trim();
                if (oldValue != request.NetworkSubdomain)
                {
                    result.Errors.Add(CreateNetworkRequestError.VerifyNetworkSubdomain, DomainStrings.ResourceManager);
                }

                if (forbiddenSubdomainNames.Contains(request.NetworkSubdomain))
                {
                    result.Errors.Add(CreateNetworkRequestError.ReservedSubdomainName, DomainStrings.ResourceManager);
                }
            }

            SrkToolkit.Common.Validation.EmailAddress email = null;
            if ((email = SrkToolkit.Common.Validation.EmailAddress.TryCreate(request.ContactEmailString)) == null)
            {
                result.Errors.Add(CreateNetworkRequestError.InvalidEmailAddress, DomainStrings.ResourceManager);
            }

            CultureInfo culture = null;
            try
            {
                culture = new CultureInfo(request.Culture);
            }
            catch (CultureNotFoundException)
            {
                result.Errors.Add(CreateNetworkRequestError.InvalidCulture, DomainStrings.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var entity = new NetworkRequest
            {
                AdminCode = Guid.NewGuid(),
                ContactEmailAccount = email.AccountPart,
                ContactEmailDomain = email.DomainPart,
                ContactEmailTag = email.TagPart,
                ContactFirstname = request.ContactFirstname,
                ContactPhoneNumber = request.ContactPhoneNumber,
                ContactLastname = request.ContactLastname,
                Culture = request.Culture,
                DateCreatedUtc = DateTime.UtcNow,
                NetworkCity = request.NetworkCity,
                NetworkCountry = request.NetworkCountry,
                NetworkName = request.NetworkName,
                NetworkSize = request.NetworkSize,
                NetworkSubdomain = request.NetworkSubdomain,
                RemoteAddress = request.RemoteAddress,
                WebId = Guid.NewGuid(),
            };
            this.Repos.NetworkRequests.Insert(entity);

            var model = new NetworkRequestModel(entity);
            result.Data = model;

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("notifications@sparklenetworks.net", "SparkleNetworks.com");
                message.To.Add(new MailAddress("hello@sparklenetworks.com", "Hello Sparkle Networks"));
                ////message.ReplyToList.Add(new MailAddress(model.ContactEmailString, model.ContactFirstname + " " + model.ContactLastname));
                message.ReplyToList.AddRange(replyTo);
                message.IsBodyHtml = false;
                message.Subject = "Apply request from SparkleNetworks.com - " + model.NetworkName;
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.None;
                var template = @"This is a apply request from {0}.

Date:      {1}
From:      {2} {3} {4} <{9}>
Company:   {5} ({6})
Of size:   {7} startups
Phone:     {8}
Email:     {9}
Location:  {10}, {11}
Culture:   {12}
URL:       {13}
Admin URL: {14}

";
                message.Body = string.Format(template, "sparklenetworks.com", model.DateCreatedUtc.ToString("o"), string.Empty, model.ContactFirstname, model.ContactLastname, model.NetworkName, string.Empty, model.NetworkSize, model.ContactPhoneNumber, model.ContactEmailString, model.NetworkCity, model.NetworkCountry, culture.EnglishName, this.Services.Configuration.GetSiteUrl(model.Culture + "/Apply/Status/" + model.WebId), this.Services.Configuration.GetSiteUrl(model.Culture + "/Apply/Status/" + model.WebId + "?AdminCode=" + model.AdminCode));

                this.Services.EmailService.SendMessage(message);

                result.Succeed = true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("NetworkRequestsService.Create failed to send email" + Environment.NewLine + ex.ToString());
            }

            return result;
        }

        public NetworkRequestModel GetByWebId(Guid webId)
        {
            var item = this.Repos.NetworkRequests.GetByWebId(webId);
            if (item != null)
                return new NetworkRequestModel(item);
            return null;
        }

        public int Count()
        {
            return this.Repos.NetworkRequests.Count();
        }

        private IList<RegionInfo> GetCountriesList(bool includeEmptyCountry)
        {
            List<RegionInfo> list;
            var theBests = new System.Collections.ArrayList
            {
                "USA",
                "GBR",
                "FRA",
                "CAN",
            };
            list = SrkToolkit.Globalization.CultureInfoHelper.GetCountries()
                .OrderBy(o => o.NativeName)
                .OrderBy(o =>
                {
                    return -theBests.IndexOf(o.ThreeLetterISORegionName);
                })
                .ToList();

            if (includeEmptyCountry)
            {
                list.Insert(0, null);
            }

            return list;
        }
    }
}
