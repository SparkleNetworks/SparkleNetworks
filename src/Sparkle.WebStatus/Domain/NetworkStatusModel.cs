
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NetworkStatusModel
    {
        public NetworkStatusModel()
        {
        }

        public NetworkStatusModel(NetworkModel x)
        {
            this.Network = x;
        }

        public NetworkModel Network { get; set; }

        public Version AssemblyVersion { get; set; }

        public Version AssemblyFileVersion { get; set; }

        public DateTime? BuildDateUtc { get; set; }

        public string BuildConfiguration { get; set; }

        public Exception StatusException { get; set; }

        public int? OnlineUsers { get; set; }

        public string RawStatus { get; set; }

        public bool? ServicesVerified { get; set; }

        public string[] ServicesVerifyErrors { get; set; }

        public List<string> OnlineUsernames { get; set; }

        public int? ActiveUsers { get; set; }

        public int? ActivitiesH24 { get; set; }
    }
}
