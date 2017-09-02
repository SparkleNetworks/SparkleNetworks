
namespace Sparkle.Services.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;

    public class NewsletterSubscriber
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Company { get; set; }

        public bool Accepted { get; set; }

        public bool? Status { get; set; }

        public string Error { get; set; }

        public override string ToString()
        {
            return this.Email;
        }

        public class NewsletterSubscriberComparer : IEqualityComparer<NewsletterSubscriber>
        {
            public bool Equals(NewsletterSubscriber x, NewsletterSubscriber y)
            {
                return x.Email == y.Email;
            }

            public int GetHashCode(NewsletterSubscriber obj)
            {
                return obj.GetHashCode();
            }
        }

    }
}
