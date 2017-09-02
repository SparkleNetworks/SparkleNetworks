
namespace Sparkle.Services.Networks.Subscriptions
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class ManualSubscribeRequest : BaseRequest
    {
        private string timezone = "Romance Standard Time";

        public ManualSubscribeRequest()
        {
        }

        public int TemplateId { get; set; }

        [Display(Name = "Timezone", ResourceType = typeof(NetworksLabels))]
        [Timezone]
        public string Timezone
        {
            get { return this.timezone; }
            set { this.timezone = value; }
        }

        [Display(Name = "BeginDate", ResourceType = typeof(NetworksLabels))]
        [DataType(DataType.DateTime)]
        public DateTime DateStartLocal { get; set; }

        public int ActingUserId { get; set; }

        public int ApplyUserId { get; set; }

        [Display(Name = "PaidByCashEur", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F2}")]
        public decimal? PaidByCashEur { get; set; }

        [Display(Name = "PaidByCashUsd", ResourceType = typeof(NetworksLabels))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:F2}")]
        public decimal? PaidByCashUsd { get; set; }

        [Display(Name = "SendUserNotify", ResourceType = typeof(NetworksLabels))]
        public bool SendEmail { get; set; }

        protected override void ValidateFields()
        {
            if (this.PaidByCashEur != null)
            {
                if (this.PaidByCashUsd != null)
                    this.AddValidationError("PaidByCashEur", NetworksLabels.OnlyOneCurrencyIsSupported);
            }

            if (this.PaidByCashUsd != null)
            {
                if (this.PaidByCashEur != null)
                    this.AddValidationError("PaidByCashUsd", NetworksLabels.OnlyOneCurrencyIsSupported);
            }
        }
    }

    public class ManualSubscribeResult : BaseResult<ManualSubscribeRequest, ManualSubscribeError>
    {
        public ManualSubscribeResult(ManualSubscribeRequest request)
            : base(request)
        {
        }

        public SubscriptionModel NewSubscription { get; set; }
    }

    public enum ManualSubscribeError
    {
        NoSuchUser,
        InvalidTemplate,
        NotAuthorized,
    }
}
