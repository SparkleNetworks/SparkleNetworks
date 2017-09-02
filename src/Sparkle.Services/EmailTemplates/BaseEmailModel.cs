
namespace Sparkle.Services.EmailTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks;
    using Sparkle.UI;
    using SrkToolkit;
    using Sparkle.Services.Networks.Lang;

    public class BaseEmailModel<T> : BaseEmailModel
    {
        public BaseEmailModel(SimpleContact recipient, string networkAccentColor, Strings lang, T data)
            : base(recipient, networkAccentColor, lang)
        {
            this.Model = data;
        }

        public T Model { get; set; }
    }
    
    public class BaseEmailModel
    {
        public BaseEmailModel(string recipientEmailAddress, string networkAccentColor, Strings lang)
        {
            this.SetRecipient(recipientEmailAddress);

            this.Data = new Dictionary<string, object>();
            this.Registered = true;
            this.RecipientInvitedId = 0;

            this.Styles = new EmailModelStyles(networkAccentColor);
            this.Lang = lang;
        }

        public BaseEmailModel(SimpleContact recipient, string networkAccentColor, Strings lang)
        {
            this.SetRecipient(recipient);

            this.Data = new Dictionary<string, object>();
            this.Registered = true;
            this.RecipientInvitedId = 0;

            this.Styles = new EmailModelStyles(networkAccentColor);
            this.Lang = lang;
        }

        public void SetRecipient(string recipientEmailAddress) 
        {
            this.RecipientEmailAddress = recipientEmailAddress;
            this.RecipientContact = new SimpleContact
            {
                EmailAddress = recipientEmailAddress,
            };
        }

        public void SetRecipient(SimpleContact recipient)
        {
            this.RecipientEmailAddress = recipient.EmailAddress;
            this.RecipientContact = recipient;
        }

        public bool Registered { get; set; }

        public User Recipient { get; set; }
        public User Sender { get; set; }

        public string RecipientEmailAddress { get; set; }

        public int RecipientInvitedId { get; set; }

        public string RecipientInvitedCode { get; set; }

        public IDictionary<string, object> Data { get; set; }

        public string Title { get; set; }
        public string Subtitle { get; set; }

        /// <summary>
        /// The click tracking URL parameter.
        /// </summary>
        public string TrackerFollow { get; set; }

        /// <summary>
        /// The display tracking URL parameter.
        /// </summary>
        public string TrackerDisplay { get; set; }

        public virtual void Initialize(IServiceFactory services)
        {
            this.AppConfiguration = services.AppConfiguration;
        }

        public SimpleContact RecipientContact { get; set; }

        public Infrastructure.AppConfiguration AppConfiguration { get; set; }

        public EmailModelStyles Styles { get; set; }

        public Strings Lang { get; set; }

        public CultureInfo Culture { get; set; }

        public TimeZoneInfo Timezone { get; set; }

        public NotificationType? NotificationType { get; set; }

        public string NotificationTypeTitle
        {
            get { return EnumTools.GetDescription(this.NotificationType.Value, NetworksEnumMessages.ResourceManager); }
        }

        public string NotificationUrl { get; set; }

        public string UnsubscribeNotificationUrl { get; set; }
    }
}
