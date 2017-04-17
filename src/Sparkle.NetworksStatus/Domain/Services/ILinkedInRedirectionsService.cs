
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Domain.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial interface ILinkedInRedirectionsService : IDisposable
    {
        void Insert(Guid redirectId, int userId, int scope, string apiKey, string state, string returnUrl);

        LinkedInRedirectionModel GetById(Guid redirectId);
        LinkedInRedirectionModel GetByState(string state);
    }
}
