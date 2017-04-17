
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NotificationModel
    {
        public NotificationModel()
        {
        }

        public bool Comment { get; set; }

        public bool ContactRequest { get; set; }

        public bool PrivateMessage { get; set; }

        public bool EventInvitation { get; set; }

        public bool Publication { get; set; }

        public bool Newsletter { get; set; }

        public bool DailyNewsletter { get; set; }

        public bool MainTimelineItems { get; set; }

        public bool MainTimelineComments { get; set; }

        public bool MailChimp { get; set; }

        public bool PrivateGroupJoinRequest { get; set; }

        public int StartPage { get; set; }

        public bool NotifyOnPersonalEmailAddress { get; set; }

        public string MailChimpStatus { get; set; }

        public bool CompanyTimelineItems { get; set; }

        public bool CompanyTimelineComments { get; set; }

        public IEnumerable<Tuple<NotificationType, string, bool>> Items
        {
            get
            {
                yield return GetTuple(NotificationType.MainTimelineItems, this.MainTimelineItems);
                yield return GetTuple(NotificationType.MainTimelineComments, this.MainTimelineComments);
                yield return GetTuple(NotificationType.DailyNewsletter, this.DailyNewsletter);
                yield return GetTuple(NotificationType.Newsletter, this.Newsletter);
                yield return GetTuple(NotificationType.Comment, this.Comment);
                yield return GetTuple(NotificationType.ContactRequest, this.ContactRequest);
                yield return GetTuple(NotificationType.EventInvitation, this.EventInvitation);
                yield return GetTuple(NotificationType.MailChimp, this.MailChimp);
                yield return GetTuple(NotificationType.PrivateGroupJoinRequest, this.PrivateGroupJoinRequest);
                yield return GetTuple(NotificationType.PrivateMessage, this.PrivateMessage);
                yield return GetTuple(NotificationType.Publication, this.Publication);
                yield return GetTuple(NotificationType.CompanyTimelineComments, this.CompanyTimelineComments);
                yield return GetTuple(NotificationType.CompanyTimelineItems, this.CompanyTimelineItems);
            }
        }

        private static Tuple<NotificationType, string, bool> GetTuple(NotificationType type, bool value)
        {
            return new Tuple<NotificationType, string, bool>(
                type,
                SrkToolkit.EnumTools.GetDescription<NotificationType>(type, NetworksEnumMessages.ResourceManager),
                value);
        }

        public void Apply(Notification result)
        {
            this.Comment = result.Comment;
            this.ContactRequest = result.ContactRequest;
            this.PrivateMessage = result.PrivateMessage;
            this.EventInvitation = result.EventInvitation;
            this.MailChimp = result.MailChimp && (result.MailChimpStatus == null || result.MailChimpStatus == "subscribed");
            this.MailChimpStatus = result.MailChimpStatus;
            this.Publication = result.Publication;
            this.PrivateGroupJoinRequest = result.PrivateGroupJoinRequest;
            this.StartPage = result.StartPage ?? 0;
            if (result.NotifyOnPersonalEmailAddress != null)
                this.NotifyOnPersonalEmailAddress = result.NotifyOnPersonalEmailAddress.Value;
            if (result.Newsletter != null)
                this.Newsletter = result.Newsletter.Value;
            if (result.DailyNewsletter != null)
                this.DailyNewsletter = result.DailyNewsletter.Value;
            if (result.MainTimelineItems != null)
                this.MainTimelineItems = result.MainTimelineItems.Value;
            if (result.MainTimelineComments != null)
                this.MainTimelineComments = result.MainTimelineComments.Value;
            if (result.CompanyTimelineItems != null)
                this.CompanyTimelineItems = result.CompanyTimelineItems.Value;
            if (result.CompanyTimelineComments != null)
                this.CompanyTimelineComments = result.CompanyTimelineComments.Value;
        }
    }
}
