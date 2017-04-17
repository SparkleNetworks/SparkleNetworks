
namespace Sparkle.Services.Networks.Payments
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TransactionModel
    {
        public TransactionModel()
        {
        }
        
        public TransactionModel(Entities.Networks.StripeTransaction stripe)
        {
            this.Type = PaymentMethod.Stripe;

            this.AmountEur = stripe.AmountEur;
            this.AmountUsd = stripe.AmountUsd;
            ////this.CardId = stripe.CardId;
            this.ChargeId = stripe.ChargeId;
            ////this.CustomerId = stripe.CustomerId;
            this.DateCreatedUtc = stripe.DateCreatedUtc.AsUtc();
            ////this.DateUpdatedUtc = stripe.DateUpdatedUtc.AsUtc();
            this.Id = stripe.Id;
            this.IsChargeCaptured = stripe.IsChargeCaptured;
            this.IsChargeCreated = stripe.IsChargeCreated;
            ////this.TokenId = stripe.TokenId;
            this.UserId = stripe.UserId;
        }

        public decimal? AmountEur { get; set; }

        public decimal? AmountUsd { get; set; }

        public string ChargeId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public int Id { get; set; }

        public bool IsChargeCreated { get; set; }

        public bool IsChargeCaptured { get; set; }

        public int UserId { get; set; }

        public PaymentMethod Type { get; set; }

        public string TypeName
        {
            get { return EnumTools.GetDescription(this.Type, NetworksEnumMessages.ResourceManager); }
        }

        public Models.UserModel User { get; set; }
    }
}
