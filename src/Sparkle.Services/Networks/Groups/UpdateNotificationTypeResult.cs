
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class UpdateNotificationTypeResult : BasicResult<UpdateNotificationError>
    {
        public int MembershipId { get; set; }
    }

    public enum UpdateNotificationError
    {
        NoSuchGroup,
        NoSuchUser,
        DeletedGroup,
        NoSuchMembership,
        NotGroupMember
    }
}
