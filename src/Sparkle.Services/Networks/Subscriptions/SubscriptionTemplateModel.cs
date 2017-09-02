
namespace Sparkle.Services.Networks.Subscriptions
{
    using Sparkle.Common;
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class SubscriptionTemplateModel
    {
        public SubscriptionTemplateModel()
        {
        }

        public SubscriptionTemplateModel(Entities.Networks.SubscriptionTemplate item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.Name = item.Name;
            this.AllowAutoRenew = item.AllowAutoRenew;
            this.DurationKind = (DurationUnit)item.DurationKind;
            this.DurationValue = item.DurationValue;
            this.IsDefaultOnAccountCreate = item.IsDefaultOnAccountCreate;
            this.IsForAllCompanyUsers = item.IsForAllCompanyUsers;
            this.IsUserSubscribable = item.IsUserSubscribable;
            this.PriceEurWithoutVat = item.PriceEurWithoutVat;
            this.PriceEurWithVat = item.PriceEurWithVat;
            this.PriceUsdWithoutVat = item.PriceUsdWithoutVat;
            this.PriceUsdWithVat = item.PriceUsdWithVat;
            this.IsSubscriptionEnabled = item.IsSubscriptionEnabled;

            this.Texts = new List<TextInfo>();
            this.Texts.Add(new TextInfo("Confirm", item.ConfirmEmailTextId));
            this.Texts.Add(new TextInfo("Renew", item.RenewEmailTextId));
            this.Texts.Add(new TextInfo("Expire", item.ExpireEmailTextId));
        }

        public int Id { get; set; }

        public int NetworkId { get; set; }

        public string Name { get; set; }

        [Display(Name = "AllowAutoRenew", ResourceType = typeof(NetworksLabels))]
        public bool AllowAutoRenew { get; set; }

        public DurationUnit DurationKind { get; set; }

        [Display(Name = "Duration", ResourceType = typeof(NetworksLabels))]
        public int DurationValue { get; set; }

        [Display(Name = "IsDefaultOnAccountCreate", ResourceType = typeof(NetworksLabels))]
        public bool IsDefaultOnAccountCreate { get; set; }

        [Display(Name = "IsForAllCompanyUsers", ResourceType = typeof(NetworksLabels))]
        public bool IsForAllCompanyUsers { get; set; }

        [Display(Name = "IsUserSubscribable", ResourceType = typeof(NetworksLabels))]
        public bool IsUserSubscribable { get; set; }

        [Display(Name = "PriceWithoutVat", ResourceType = typeof(NetworksLabels))]
        public decimal? PriceEurWithoutVat { get; set; }

        [Display(Name = "PriceWithVat", ResourceType = typeof(NetworksLabels))]
        public decimal? PriceEurWithVat { get; set; }

        [Display(Name = "PriceWithoutVat", ResourceType = typeof(NetworksLabels))]
        public decimal? PriceUsdWithoutVat { get; set; }

        [Display(Name = "PriceWithVat", ResourceType = typeof(NetworksLabels))]
        public decimal? PriceUsdWithVat { get; set; }

        [Display(Name = "IsValid", ResourceType = typeof(NetworksLabels))]
        public bool IsValid
        {
            get
            {
                if ((this.PriceEurWithoutVat.HasValue || this.PriceEurWithVat.HasValue) && (this.PriceUsdWithoutVat.HasValue || this.PriceUsdWithVat.HasValue))
                    return false;
                return true;
            }
        }

        public decimal? PriceToPay
        {
            get { return this.PriceEurWithVat ?? this.PriceEurWithoutVat ?? this.PriceUsdWithVat ?? this.PriceUsdWithoutVat ?? null; }
        }

        public string PriceToPayTitle
        {
            get
            {
                if (this.PriceEurWithVat != null)
                    return this.PriceEurWithVat.Value.ToAmount(Common.StringTransformer.AmountCurrency.EUR);
                if (this.PriceEurWithoutVat != null)
                    return this.PriceEurWithoutVat.Value.ToAmount(Common.StringTransformer.AmountCurrency.EUR);
                if (this.PriceUsdWithVat != null)
                    return this.PriceUsdWithVat.Value.ToAmount(Common.StringTransformer.AmountCurrency.USD);
                if (this.PriceUsdWithoutVat != null)
                    return this.PriceUsdWithoutVat.Value.ToAmount(Common.StringTransformer.AmountCurrency.USD);
                else
                    return null;
            }
        }

        public IList<TextInfo> Texts { get; set; }

        public TextInfo ConfirmText
        {
            get { return this.Texts.SingleOrDefault(o => o.Name == "Confirm"); }
        }

        public TextInfo RenewText
        {
            get { return this.Texts.SingleOrDefault(o => o.Name == "Renew"); }
        }

        public TextInfo ExpireText
        {
            get { return this.Texts.SingleOrDefault(o => o.Name == "Expire"); }
        }

        [Display(Name = "IsSubscriptionEnabled", ResourceType = typeof(NetworksLabels))]
        public bool IsSubscriptionEnabled { get; set; }

        public override string ToString()
        {
            return "SubscriptionTemplateModel " + this.Id + " \"" + this.Name + "\"";
        }
    }

    public class TextInfo
    {
        public string Name { get; set; }

        public int? Id { get; set; }

        public TextInfo(string name, int? id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}
