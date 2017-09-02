
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ApiKeyModel
    {
        private string secret;

        public ApiKeyModel()
        {
        }

        public ApiKeyModel(Entities.Networks.ApiKey item)
        {
            this.Set(item);
        }

        private void Set(Entities.Networks.ApiKey item)
        {
            this.Id = item.Id;
            this.IsEnabled = item.IsEnabled;
            this.Key = item.Key;
            this.Name = item.Name;
            this.Roles = item.Roles;
            this.secret = item.Secret;
            this.DateCreatedUtc = item.DateCreatedUtc;
            this.Description = item.Description;
        }

        public int Id { get; set; }

        public bool IsEnabled { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Roles { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public string Description { get; set; }

        public string GetSecret()
        {
            return this.secret;
        }

        public override string ToString()
        {
            return "ApiKey " + this.Id + " '" + this.Key + "' " + (this.IsEnabled ? "Enabled" : "DISABLED");
        }
    }
}
