
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class UserEmailChangeRequest : IEntityInt32Id, INetworkEntity
    {
        public UserEmailChangeRequestStatus StatusValue
        {
            get { return (UserEmailChangeRequestStatus)this.Status; }
            set { this.Status = (int)value; }
        }

        public bool IsPreviousEmailForbidden
        {
            get { return this.PreviousEmailForbidden == 0 ? false : true; }
            set { this.PreviousEmailForbidden = value == false ? 0 : 1; }
        }

        public override string ToString()
        {
            return "Requests from user " + this.UserId + " to change from " + this.PreviousEmailAccountPart + this.PreviousEmailTagPart ?? "" + this.PreviousEmailDomainPart + " to " + this.NewEmailAccountPart + this.NewEmailTagPart ?? "" + this.NewEmailDomainPart + " done by user " + this.CreatedByUserId;
        }
    }

    public enum UserEmailChangeRequestOptions
    {
        None            = 0x0000,
        User            = 0x0002,
        CreatedByUser   = 0x0004,
    }

    public enum UserEmailChangeRequestStatus
    {
        Pending,
        Canceled,
        Succeed,
    }
}
