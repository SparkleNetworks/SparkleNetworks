
namespace Sparkle.Services.Networks.Subscriptions
{
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class MassSubscriptionRequest : BaseRequest
    {
        [Display(Name = "BeginDate", ResourceType = typeof(NetworksLabels))]
        [DataType(DataType.DateTime)]
        public DateTime StartDateLocal { get; set; }

        [Display(Name = "Subscription", ResourceType = typeof(NetworksLabels))]
        public int TemplateId { get; set; }
        public IList<SubscriptionTemplateModel> Templates { get; set; }

        public int UserId { get; set; }

        public bool SendConfirmEmail { get; set; }
    }

    public class MassSubscriptionResult : BaseResult<MassSubscriptionRequest, MassSubscriptionError>
    {
        public MassSubscriptionResult(MassSubscriptionRequest request)
            : base(request)
        {
        }
    }

    public enum MassSubscriptionError
    {
        NotAuthorized,
        InvalidTemplate
    }
}
