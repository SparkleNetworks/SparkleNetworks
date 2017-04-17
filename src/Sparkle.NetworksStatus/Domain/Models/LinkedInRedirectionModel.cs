
namespace Sparkle.NetworksStatus.Domain.Models
{
    using Sparkle.NetworksStatus.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LinkedInRedirectionModel
    {
        public LinkedInRedirectionModel(LinkedInRedirection item)
        {
            this.Set(item);
        }

        private void Set(LinkedInRedirection item)
        {
            this.Id = item.Id;
            this.UserId = item.UserId;
            this.Scope = item.Scope;
            this.ApiKey = item.ApiKey;
            this.State = item.State;
            this.ReturnUrl = item.ReturnUrl;
        }

        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int Scope { get; set; }

        public string ApiKey { get; set; }

        public string State { get; set; }

        public string ReturnUrl { get; set; }
    }
}
