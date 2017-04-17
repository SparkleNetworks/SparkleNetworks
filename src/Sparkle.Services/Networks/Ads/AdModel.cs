
namespace Sparkle.Services.Networks.Ads
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdModel
    {
        public AdModel()
        {
        }

        public AdModel(Ad item, AdCategory category)
        {
            this.Set(item);

            if (category != null)
            {
                this.CategoryName = category.Name;
                this.CategoryAlias = category.Alias;
            }
        }

        public AdModel(Ad item, AdCategoryModel category)
        {
            this.Set(item);

            if (category != null)
            {
                this.CategoryName = category.Name;
                this.CategoryAlias = category.Alias;
            }
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int? NetworkId { get; set; }

        public string Alias { get; set; }

        public int CategoryId { get; set; }

        public DateTime Date { get; set; }

        public bool IsOpen { get; set; }

        public DateTime? CloseDateUtc { get; set; }

        public int? CloseUserId { get; set; }

        public bool? IsValidated { get; set; }

        public DateTime? ValidationDateUtc { get; set; }

        public int? ValidationUserId { get; set; }

        public DateTime? UpdateDateUtc { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? PendingEditDate { get; set; }

        public string PendingEditMessage { get; set; }

        public string PendingEditTitle { get; set; }

        public string CategoryName { get; set; }

        public string CategoryAlias { get; set; }

        public Models.UserModel Owner { get; set; }

        public bool? IsNewForUser { get; set; }

        public override string ToString()
        {
            return "Ad " + this.Id + " U:" + this.UserId + " "
                + (this.IsOpen ? "IsOpen" : "IsClose") + " "
                + (this.IsValidated == null ? "IsPendingValidation" : this.IsValidated == true ? "IsValidated" : "IsRefused") + " "
                + (this.PendingEditDate != null ? "IsPendingEdit " : "")
                + "\"" + this.Title + "\"";
        }

        private void Set(Ad item)
        {
            this.Id = item.Id;
            this.UserId = item.UserId;
            this.NetworkId = item.NetworkId;
            this.Alias = item.Alias;
            this.CategoryId = item.CategoryId;
            this.CloseDateUtc = item.CloseDateUtc.AsUtc();
            this.CloseUserId = item.CloseUserId;
            this.Date = item.Date.AsUtc();
            this.IsOpen = item.IsOpen;
            this.IsValidated = item.IsValidated;
            this.ValidationDateUtc = item.ValidationDateUtc.AsUtc();
            this.ValidationUserId = item.ValidationUserId;
            this.Content = item.Message;
            this.PendingEditDate = item.PendingEditDate.AsUtc();
            this.PendingEditMessage = item.PendingEditMessage;
            this.PendingEditTitle = item.PendingEditTitle;
            this.Title = item.Title;
            this.UpdateDateUtc = item.UpdateDateUtc.AsUtc();

            if (item.OwnerReference.IsLoaded)
            {
                this.Owner = new Models.UserModel(item.Owner);
            }
        }
    }
}
