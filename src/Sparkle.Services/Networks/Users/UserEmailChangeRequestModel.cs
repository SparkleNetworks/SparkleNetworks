
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Models;
    using Sparkle.Entities.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserEmailChangeRequestModel
    {
        public UserEmailChangeRequestModel()
        {
        }

        public UserEmailChangeRequestModel(UserEmailChangeRequest item)
            : this()
        {
            this.Set(item);
        }

        public int Id { get; set; }

        public int NetworkId { get; set; }

        public string PreviousEmailAccountPart { get; set; }

        public string PreviousEmailTagPart { get; set; }

        public string PreviousEmailDomainPart { get; set; }

        public string NewEmailAccountPart { get; set; }

        public string NewEmailTagPart { get; set; }

        public string NewEmailDomainPart { get; set; }

        public int UserId { get; set; }

        public UserModel User { get; set; }

        public string UserProfileUrl { get; set; }

        public int CreatedByUserId { get; set; }

        public UserModel CreatedByUser { get; set; }

        public string CreatedByUserProfileUrl { get; set; }

        public int Status { get; set; }

        public UserEmailChangeRequestStatus StatusValue { get; set; }

        public bool IsPreviousEmailForbidden { get; set; }

        public string EmailChangeRemark { get; set; }

        public DateTime CreateDateUtc { get; set; }

        public DateTime? ValidateDateUtc { get; set; }

        public EmailAddress PreviousEmail
        {
            get { return new EmailAddress(this.PreviousEmailAccountPart, this.PreviousEmailTagPart, this.PreviousEmailDomainPart); }
        }

        public EmailAddress NewEmail
        {
            get { return new EmailAddress(this.NewEmailAccountPart, this.NewEmailTagPart, this.NewEmailDomainPart); }
        }

        private void Set(UserEmailChangeRequest item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.PreviousEmailAccountPart = item.PreviousEmailAccountPart;
            this.PreviousEmailTagPart = item.PreviousEmailTagPart;
            this.PreviousEmailDomainPart = item.PreviousEmailDomainPart;
            this.NewEmailAccountPart = item.NewEmailAccountPart;
            this.NewEmailTagPart = item.NewEmailTagPart;
            this.NewEmailDomainPart = item.NewEmailDomainPart;
            this.UserId = item.UserId;
            this.CreatedByUserId = item.CreatedByUserId;
            this.Status = item.Status;
            this.StatusValue = item.StatusValue;
            this.IsPreviousEmailForbidden = item.IsPreviousEmailForbidden;
            this.EmailChangeRemark = item.EmailChangeRemark;
            this.CreateDateUtc = item.CreateDateUtc;
            this.ValidateDateUtc = item.ValidateDateUtc;
            this.EmailChangeRemark = item.EmailChangeRemark;

            if (item.User != null)
            {
                this.User = new UserModel(item.User);
            }
            if (item.CreatedByUser != null)
            {
                this.CreatedByUser = new UserModel(item.CreatedByUser);
            }
        }
    }
}
