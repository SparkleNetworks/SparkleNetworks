
namespace Sparkle.Services.Networks.Subscriptions
{
    using Sparkle.Common;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class SubscriptionModel
    {
        public SubscriptionModel()
        {
        }

        public SubscriptionModel(Entities.Networks.Neutral.SubscriptionPoco item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.TemplateId = item.TemplateId;

            this.AppliesToCompanyId = item.AppliesToCompanyId;
            if (item.AppliesToCompanyId != null)
            {
                this.AppliesToCompany = item.AppliesToCompany != null 
                    ? new CompanyModel(item.AppliesToCompany) 
                    : new CompanyModel(item.AppliesToCompanyId.Value, null, null);
            }

            this.AppliesToUserId = item.AppliesToUserId;
            if (item.AppliesToUserId != null)
            {
                this.AppliesToUser = item.AppliesToUser != null 
                    ? new UserModel(item.AppliesToUser)
                    : new UserModel(item.AppliesToUserId.Value);
            }

            this.AutoRenew = item.AutoRenew;
            this.DateBeginUtc = item.DateBeginUtc.AsUtc();
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateEndUtc = item.DateEndUtc.AsUtc();
            this.DurationKind = (DurationUnit)item.DurationKind;
            this.DurationValue = item.DurationValue;
            this.IsForAllCompanyUsers = item.IsForAllCompanyUsers;
            this.IsPaid = item.IsPaid;
            this.Name = item.Name;

            this.OwnerCompanyId = item.OwnerCompanyId;
            if (item.OwnerCompanyId != null)
            {
                this.OwnerCompany = item.OwnerCompany != null 
                    ? new CompanyModel(item.OwnerCompany) 
                    : new CompanyModel(item.OwnerCompanyId.Value, null, null);
            }

            this.OwnerUserId = item.OwnerUserId;
            if (item.OwnerUserId != null)
            {
                this.OwnerUser = item.OwnerUser != null
                    ? new UserModel(item.OwnerUser)
                    : new UserModel(item.OwnerUserId.Value);
            }

            this.PriceEurWithoutVat = item.PriceEurWithoutVat;
            this.PriceEurWithVat = item.PriceEurWithVat;
            this.PriceUsdWithoutVat = item.PriceUsdWithoutVat;
            this.PriceUsdWithVat = item.PriceUsdWithVat;
            this.PaymentMethod = item.PaymentMethodValue;
        }

        public SubscriptionModel(Entities.Networks.Subscription item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.TemplateId = item.TemplateId;

            this.AppliesToCompanyId = item.AppliesToCompanyId;
            if (item.AppliesToCompanyId != null)
            {
                this.AppliesToCompany = item.AppliesToCompanyReference.IsLoaded
                    ? new CompanyModel(item.AppliesToCompany) 
                    : new CompanyModel(item.AppliesToCompanyId.Value, null, null);
            }

            this.AppliesToUserId = item.AppliesToUserId;
            if (item.AppliesToUserId != null)
            {
                this.AppliesToUser = item.AppliesToUserReference.IsLoaded
                    ? new UserModel(item.AppliesToUser)
                    : new UserModel(item.AppliesToUserId.Value);
            }

            this.AutoRenew = item.AutoRenew;
            this.DateBeginUtc = item.DateBeginUtc.AsUtc();
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateEndUtc = item.DateEndUtc.AsUtc();
            this.DurationKind = (DurationUnit)item.DurationKind;
            this.DurationValue = item.DurationValue;
            this.IsForAllCompanyUsers = item.IsForAllCompanyUsers;
            this.IsPaid = item.IsPaid;
            this.Name = item.Name;

            this.OwnerCompanyId = item.OwnerCompanyId;
            if (item.OwnerCompanyId != null)
            {
                this.OwnerCompany = item.OwnerCompanyReference.IsLoaded
                    ? new CompanyModel(item.OwnerCompany) 
                    : new CompanyModel(item.OwnerCompanyId.Value, null, null);
            }

            this.OwnerUserId = item.OwnerUserId;
            if (item.OwnerUserId != null)
            {
                this.OwnerUser = item.OwnerUserReference.IsLoaded
                    ? new UserModel(item.OwnerUser)
                    : new UserModel(item.OwnerUserId.Value);
            }

            this.PriceEurWithoutVat = item.PriceEurWithoutVat;
            this.PriceEurWithVat = item.PriceEurWithVat;
            this.PriceUsdWithoutVat = item.PriceUsdWithoutVat;
            this.PriceUsdWithVat = item.PriceUsdWithVat;
            this.PaymentMethod = item.PaymentMethodValue;
        }

        public bool IsActive
        {
            get
            {
                this.Now = this.Now ?? DateTime.UtcNow;

                return this.IsPaid
                    && this.DateBeginUtc != null && this.DateBeginUtc < this.Now.Value
                    && (this.DateEndUtc == null || this.Now.Value < this.DateEndUtc);
            }
        }

        public bool IsPast
        {
            get
            {
                this.Now = this.Now ?? DateTime.UtcNow;

                return this.DateEndUtc != null && this.DateEndUtc < this.Now.Value;
            }
        }

        public int Id { get; set; }

        public int TemplateId { get; set; }

        public int NetworkId { get; set; }

        public int? AppliesToCompanyId { get; set; }

        public CompanyModel AppliesToCompany { get; set; }

        public int? AppliesToUserId { get; set; }

        public UserModel AppliesToUser { get; set; }

        public bool AutoRenew { get; set; }

        public DateTime? DateBeginUtc { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime? DateEndUtc { get; set; }

        public DurationUnit DurationKind { get; set; }

        public int DurationValue { get; set; }

        public bool IsForAllCompanyUsers { get; set; }

        public bool IsPaid { get; set; }

        public string Name { get; set; }

        public int? OwnerCompanyId { get; set; }

        public CompanyModel OwnerCompany { get; set; }

        public int? OwnerUserId { get; set; }

        public UserModel OwnerUser { get; set; }

        public decimal? PriceEurWithoutVat { get; set; }

        public decimal? PriceEurWithVat { get; set; }

        public decimal? PriceUsdWithoutVat { get; set; }

        public decimal? PriceUsdWithVat { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public string PaymentMethodTitle
        {
            get { return SrkToolkit.EnumTools.GetDescription(this.PaymentMethod, NetworksEnumMessages.ResourceManager); }
        }

        [IgnoreDataMember]
        public IList<Payments.TransactionModel> Transactions { get; set; }

        public string Title
        {
            get
            {
                if (this.DateBeginUtc != null && this.DateEndUtc != null)
                    return string.Format(NetworksLabels.SubscriptionTitle2Dates, this.DateBeginUtc.Value.ToString("d"), this.DateEndUtc.Value.ToString("d"));

                if (this.DateBeginUtc != null)
                    return string.Format(NetworksLabels.SubscriptionTitleBeginDate, this.DateBeginUtc.Value.ToString("d"));

                if (this.DateEndUtc != null)
                    return string.Format(NetworksLabels.SubscriptionTitleEndDate, this.DateEndUtc.Value.ToString("d"));

                return string.Format(NetworksLabels.SubscriptionId, this.Id);
            }
        }

        // used in subscription notifiations
        public int RemainingDays
        {
            get { return this.DateEndUtc.HasValue && this.Now.HasValue ? (int)((this.DateEndUtc.Value - this.Now.Value).TotalDays + 1) : 0; }
        }

        public DateTime? Now { get; set; }
    }
}
