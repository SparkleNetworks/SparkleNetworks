
namespace Sparkle.Services.Networks.Groups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class InviteToGroupResult : BasicResult<InviteToGroupError>
    {
    }

    public enum InviteToGroupError
    {
        NoSuchGroup,
        NoSuchUser,
        NoSuchMembership,
        AlreadyAcceptedMember,
        KickedFromGroup,
        ActingUserIsNotGroupMember,
        AlreadyJoinRequest,
        AlreadyMember,
        AlreadyInvited,
        AlreadyRejected,
        UnexpectedMembershipStatis,
        DeletedGroup,
        Left,
    }
}
