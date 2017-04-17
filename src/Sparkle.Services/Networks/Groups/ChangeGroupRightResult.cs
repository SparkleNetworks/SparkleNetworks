
namespace Sparkle.Services.Networks.Groups
{
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ChangeGroupRightResult : BasicResult<ChangeGroupRightError>
    {
        public int MembershipId { get; set; }

        public GroupMemberModel Membership { get; set; }

        public bool IsUserNotified { get; set; }
    }

    public enum ChangeGroupRightError
    {
        NoSuchGroup,
        DeletedGroup,
        NoSuchUser,
        NoSuchMembership,
        NotAuthorized,
        NotGroupMember,
    }
}
