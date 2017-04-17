
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserInvitationModel
    {
        public UserInvitationModel(Entities.Networks.Invited item)
        {
            this.Set(item);
        }

        private void Set(Entities.Networks.Invited item)
        {
            if (item == null)
                return;

            this.Id = item.Id;
            this.Code = item.Code;
            this.CompanyId = item.CompanyId;
            this.CompanyAccessLevel = item.CompanyAccessLevel != null ? (CompanyAccessLevel)item.CompanyAccessLevel.Value : default(CompanyAccessLevel?);
            this.DateInvitedUtc = item.Date.ToUniversalTime();
            this.Email = new EmailAddress(item.Email);
            this.InvitedByUserId = item.InvitedByUserId;
            this.Unregistred = item.Unregistred;
            this.UserId = item.UserId;
        }

        public int Id { get; set; }

        public Guid Code { get; set; }

        public int CompanyId { get; set; }

        public CompanyAccessLevel? CompanyAccessLevel { get; set; }

        public DateTime DateInvitedUtc { get; set; }

        public EmailAddress Email { get; set; }

        public int InvitedByUserId { get; set; }

        public bool Unregistred { get; set; }

        public int? UserId { get; set; }

        public RegisterRequestModel RegisterRequest { get; set; }
    }
}
