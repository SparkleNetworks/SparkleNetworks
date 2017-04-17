
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LightContactsModel
    {
        public IList<LightContactModel> Contacts { get; set; }

        public LightContactsModel()
        {
            this.Contacts = new List<LightContactModel>();
        }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new Newtonsoft.Json.JsonSerializerSettings {  });
        }
    }

    public class LightContactModel
    {
        public Guid UniqueId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string[] Emails { get; set; }

        public string Title { get; set; }

        public string Place { get; set; }

        public string DisplayName
        {
            get { return this.Firstname + " " + this.Lastname; }
        }

        public string Email
        {
            get { return this.Emails.Length > 0 ? "(" + this.Emails.First() + ")" : ""; }
        }

        public string Headline
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Title))
                {
                    if (!string.IsNullOrEmpty(this.Place))
                        return this.Title + " @ " + this.Place;
                    return this.Title;
                }
                return this.Place;
            }
        }

        public LightContactModel()
        {
            this.UniqueId = Guid.NewGuid();
        }

        public bool IsOnNetwork { get; set; }
    }
}
