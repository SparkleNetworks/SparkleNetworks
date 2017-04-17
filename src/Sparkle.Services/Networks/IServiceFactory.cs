
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    partial interface IServiceFactory : IDisposable
    {
        INetworksTransaction CurrentTransaction { get; set; }
        ICacheService Cache { get; }

        int NetworkId { get; set; }
        NetworkModel Network { get; }
        NetworksServiceContext Context { get; set; }
        CultureInfo[] SupportedCultures { get; }
        CultureInfo DefaultCulture { get; }
        TimeZoneInfo DefaultTimezone { get; }

        IMembershipService MembershipService { get; }

        void Initialize();
        void Verify();
        IServiceFactory CreateNewFactory();
        INetworksTransaction NewTransaction();
        void Parallelize(Action<IServiceFactory> action);

        string GetUrl(string path, IDictionary<string, string> query = null, string fragment = null);
        string GetLocalUrl(string path, IDictionary<string, string> query = null, string fragment = null);
    }
}
