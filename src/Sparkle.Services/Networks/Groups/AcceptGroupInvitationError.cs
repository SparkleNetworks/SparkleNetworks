
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class AcceptGroupInvitationResult : BasicResult<AcceptGroupInvitationError>
    {
        public AcceptGroupInvitationResult()
            : base()
        {
        }

        public int MembershipId { get; set; }
        public int NewTimelineItemId { get; set; }
        public bool IsPendingGroupValidation { get; set; }
        public bool IsJoined { get; set; }

        public bool IsInviterNotified { get; set; }
    }

    public enum AcceptGroupInvitationError
    {
        NoSuchGroup,
        NoSuchUser,
        NoSuchMembership,
        AlreadyAcceptedMember,
        KickedFromGroup,
        DeletedGroup,
    }
}
