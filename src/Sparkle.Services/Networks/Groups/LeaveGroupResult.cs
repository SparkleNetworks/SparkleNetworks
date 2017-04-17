
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class LeaveGroupResult : BasicResult<LeaveGroupError>
    {
        public int MembershipId { get; set; }
    }

    public enum LeaveGroupError
    {
        NoSuchGroup,
        DeletedGroup,
        NoSuchUser,
        NoSuchMembership,
        NotGroupMember,
        LastAdmin
    }
}
