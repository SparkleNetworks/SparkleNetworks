
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Domain.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class LinkedInRedirectionsService : ILinkedInRedirectionsService
    {
        public void Insert(Guid redirectId, int userId, int scope, string apiKey, string state, string returnUrl)
        {
            var obj = new LinkedInRedirection
            {
                Id = redirectId,
                UserId = userId,
                Scope = scope,
                ApiKey = apiKey,
                State = state,
                ReturnUrl = returnUrl,
                DateCreatedUtc = DateTime.UtcNow,
            };

            this.Repos.LinkedInRedirections.Insert(obj);
        }

        public LinkedInRedirectionModel GetById(Guid redirectId)
        {
            var item = this.Services.Repositories.LinkedInRedirections.GetById(redirectId);
            if (item == null)
                return null;

            return new LinkedInRedirectionModel(item);
        }

        public LinkedInRedirectionModel GetByState(string state)
        {
            var item = this.Services.Repositories.LinkedInRedirections.GetByState(state);
            if (item == null)
                return null;

            return new LinkedInRedirectionModel(item);
        }
    }
}
