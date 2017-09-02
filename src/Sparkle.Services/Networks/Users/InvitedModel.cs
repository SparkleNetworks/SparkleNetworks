
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InvitedModel
    {
        public InvitedModel(Entities.Networks.Invited item)
        {
            this.Id = item.Id;
            this.Code = item.Code;
            this.CompanyAccessLevel = item.CompanyAccessLevel;
            this.CompanyId = item.CompanyId;
            this.Date = item.Date.AsUtc();
            this.DeletedByUserId = item.DeletedByUserId;
            this.DeletedDateUtc = item.DeletedDateUtc.AsUtc();
            this.Email = item.Email;
            this.InvitedByUserId = item.InvitedByUserId;
            this.Unregistred = item.Unregistred;
            this.UserId = item.UserId;
        }

        public int Id { get; set; }

        public Guid Code { get; set; }

        public int? CompanyAccessLevel { get; set; }

        public int CompanyId { get; set; }

        public DateTime Date { get; set; }

        public int? DeletedByUserId { get; set; }

        public DateTime? DeletedDateUtc { get; set; }

        public string Email { get; set; }

        public int InvitedByUserId { get; set; }

        public bool Unregistred { get; set; }

        public int? UserId { get; set; }

        public Models.UserModel InvitedByUser { get; set; }
    }
}
