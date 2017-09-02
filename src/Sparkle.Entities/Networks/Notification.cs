
namespace Sparkle.Entities.Networks
{
    partial class Notification
    {
        public override string ToString()
        {
            return "For user " + this.UserId;
        }
    }

    public enum NotificationType
    {
        ContactRequest,
        Publication,
        Comment,
        EventInvitation,
        PrivateMessage,
        Newsletter,
        DailyNewsletter,
        PrivateGroupJoinRequest,
        MailChimp,
        MainTimelineItems,
        MainTimelineComments,
        CompanyTimelineItems,
        CompanyTimelineComments,
    }
}
