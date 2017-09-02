
namespace Sparkle.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Sparkle.Entities.Networks;
    using Sparkle.WebBase;

    /// <summary>
    /// Allows access based on network access level.
    /// If you specify multiple values, the user is authorized in case (s)he has at least one of the specified roles.
    /// </summary>
    public class AuthorizeByNetworkAccessAttribute : AuthorizeUserAttribute
    {
        public AuthorizeByNetworkAccessAttribute(NetworkAccessLevel Level)
        {
            this.Level = Level;
        }

        public NetworkAccessLevel Level { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var sessionService = new NetworkSessionService(httpContext.Session);
            var user = sessionService.User;

            if (user == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var flags = Enum.GetValues(typeof(NetworkAccessLevel))
                .Cast<NetworkAccessLevel>()
                .Where(v => ((int)v) != 0 && (v & this.Level) == v)
                .ToArray();

            if (user.NetworkAccess.HasAnyFlag(flags))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}