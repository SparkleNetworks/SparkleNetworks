
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class JoinGroupResult : BasicResult<JoinGroupError>
    {
        public int MembershipId { get; set; }

        public bool IsJoined { get; set; }

        public int NewTimelineItemId { get; set; }

        public bool IsPendingGroupValidation { get; set; }
    }

    public enum JoinGroupError
    {
        NoSuchGroup,
        DeletedGroup,
        NoSuchUser,
        UnexpectedMembership,
        AlreadyAcceptedMember
    }
}
