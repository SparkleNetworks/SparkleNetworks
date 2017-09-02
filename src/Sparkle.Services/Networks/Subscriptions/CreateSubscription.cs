
namespace Sparkle.Services.Networks.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class CreateSubscriptionRequest : BaseRequest
    {
        public CreateSubscriptionRequest()
        {
        }

        public int UserId { get; set; }
    }

    public class CreateSubscriptionResult : BaseResult<CreateSubscriptionRequest, CreateSubscriptionError>
    {
        public CreateSubscriptionResult(CreateSubscriptionRequest request)
            : base(request)
        {
        }

        public SubscriptionModel CreatedSubscription { get; set; }
    }

    public enum CreateSubscriptionError
    {
        NoSuchUser,
        InactiveUser,
        NoDefaultTemplate,
        NotEligible,
    }
}
