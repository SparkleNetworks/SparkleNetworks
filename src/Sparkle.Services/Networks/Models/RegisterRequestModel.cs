
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RegisterRequestModel
    {
        public RegisterRequestModel()
        {
        }

        public RegisterRequestModel(RegisterRequest item)
        {
            this.Set(item);
        }

        private void Set(RegisterRequest item)
        {
            this.Id = item.Id;

            this.CompanyId = item.CompanyId;
            this.CompanyName = item.CompanyName;
            if (item.CompanyId != null)
            {
                this.Company = item.Company != null ? new CompanyModel(item.Company) : new CompanyModel(item.CompanyId.Value, null, null);
            }

            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateUpdatedUtc = item.DateUpdatedUtc.AsUtc();
            this.EmailAddress = item.EmailAddress;
            this.NetworkId = item.NetworkId;

            this.RepliedBy = item.RepliedBy != null ? new UserModel(item.RepliedBy) : null;
            this.ReplyUserId = item.ReplyUserId;

            this.StatusCode = item.StatusCode;

            this.ValidatedBy = item.ValidatedBy != null ? new UserModel(item.ValidatedBy) : null;
            this.ValidatedByUserId = item.ValidatedByUserId;

            this.Code = item.Code;

            this.AcceptedInvitationId = item.AcceptedInvitationId;
            if (item.AcceptedInvitationId != null)
            {
                if (item.AcceptedInvitation != null)
                {
                    this.AcceptedInvitation = new RegisterInvitationModel(item.AcceptedInvitation);
                }
                else
                {
                    this.AcceptedInvitation = new RegisterInvitationModel(item.AcceptedInvitationId.Value);
                }
            }
        }

        public int Id { get; set; }

        public int? CompanyId { get; set; }
        public CompanyModel Company { get; set; }

        public string CompanyName { get; set; }

        public string DisplayCompanyName
        {
            get { return this.CompanyName != null ? this.CompanyName : this.Company.Name; }
        }

        public DateTime DateCreatedUtc { get; set; }
        public DateTime? DateUpdatedUtc { get; set; }

        public string EmailAddress { get; set; }

        public int NetworkId { get; set; }

        public UserModel RepliedBy { get; set; }
        public int? ReplyUserId { get; set; }

        public RegisterRequestStatus StatusCode { get; set; }

        public UserModel ValidatedBy { get; set; }
        public int? ValidatedByUserId { get; set; }

        public Guid? Code { get; set; }

        public int? AcceptedInvitationId { get; set; }
        public RegisterInvitationModel AcceptedInvitation { get; set; }

        public string RepliedByProfileUrl { get; set; }

        public string ValidatedByProfileUrl { get; set; }
    }
}
