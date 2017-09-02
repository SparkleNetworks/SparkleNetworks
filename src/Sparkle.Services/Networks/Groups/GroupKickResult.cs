
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class GroupKickResult : BasicResult<GroupKickError>
    {
        public int MembershipId { get; set; }

        public Entities.Networks.GroupMember Membership { get; set; }

        public bool IsUserNotified { get; set; }
    }

    public enum GroupKickError
    {
        NoSuchGroup,
        DeletedGroup,
        NoSuchUser,
        NoSuchMembership,
        NotGroupMember,
        NotAuthorized,
    }
}
