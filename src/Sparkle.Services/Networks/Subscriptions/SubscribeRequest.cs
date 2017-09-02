
namespace Sparkle.Services.Networks.Subscriptions
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscribeRequest : BaseRequest
    {
        public SubscribeRequest()
        {
        }

        public SubscribeRequest(int userId, int templateId)
        {
            this.UserId = userId;
            this.TemplateId = templateId;
        }

        public int UserId { get; set; }

        public int TemplateId { get; set; }

        public SubscriptionTemplateModel Template { get; set; }

        public SubscriptionProvider SubscriptionProvider { get; set; }

        public StripeConfigModel StripeConfig { get; set; }

        public string SiteName { get; set; }

        public string UserEmail { get; set; }

        public string stripeToken { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class SubscribeResult : BaseResult<SubscribeRequest, SubscribeError>
    {
        public SubscribeResult(SubscribeRequest request)
            : base(request)
        {
        }
    }

    public enum SubscribeError
    {
        NoSuchTemplate,
        CannotCreateCharge,
        CannotCaptureCharge,
        NoSuchUserOrInactive,
        InternalError,
    }

    public enum SubscriptionProvider
    {
        Stripe,
    }
}
