
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EmailContact
    {
        public EmailContact()
        {
        }

        public EmailContact(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public EmailContact(string emailAddress, string displayName)
        {
            this.EmailAddress = emailAddress;
            this.DisplayName = EnsureDisplayName(displayName);
        }

        public string DisplayName { get; set; }

        public string EmailAddress { get; set; }

        public static EmailContact Create(User user)
        {
            return new EmailContact
            {
                DisplayName = EnsureDisplayName(user.FirstName + " " + user.LastName),
                EmailAddress = user.Email,
            };
        }

        public static EmailContact Create(Sparkle.Entities.Networks.Neutral.UserPoco user)
        {
            return new EmailContact
            {
                DisplayName = EnsureDisplayName(user.FirstName + " " + user.LastName),
                EmailAddress = user.Email,
            };
        }

        public static EmailContact Create(Sparkle.Entities.Networks.Neutral.Person user)
        {
            return new EmailContact
            {
                DisplayName = EnsureDisplayName(user.FirstName + " " + user.LastName),
                EmailAddress = user.Email,
            };
        }

        public static EmailContact Create(Invited user)
        {
            return new EmailContact
            {
                EmailAddress = user.Email,
            };
        }

        private static string EnsureDisplayName(string displayName)
        {
            return string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        }
    }
}
