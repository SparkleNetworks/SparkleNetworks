
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnsubscribeFromNotificationRequest : BaseRequest
    {
        public int UserId { get; set; }

        public string Type { get; set; }
    }

    public class UnsubscribeFromNotificationResult : BaseResult<UnsubscribeFromNotificationRequest, UnsubscribeFromNotificationError>
    {
        public UnsubscribeFromNotificationResult(UnsubscribeFromNotificationRequest request)
            : base(request)
        {
        }
    }

    public enum UnsubscribeFromNotificationError
    {
        NoSuchNotification,
        NoSuchUser
    }
}
