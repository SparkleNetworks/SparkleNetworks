
namespace Sparkle.Services.Networks.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;

    public class InviteToEventRequest : BaseRequest
    {
        public List<int> UserIds { get; set; }

        public int EventId { get; set; }

        public int FromUserId { get; set; }
    }

    public class InviteToEventResult : BaseResult<InviteToEventRequest, InviteToEventCode>
    {
        public InviteToEventResult(InviteToEventRequest request)
            : base(request)
        {
            this.Results = new List<UserResult>();
        }

        public IList<UserResult> Results { get; set; }

        public class UserResult
        {
            public UserModel User { get; set; }
            public bool Succeed { get; set; }
            public EventMemberState PreviousState { get; set; }
            public SrkToolkit.Domain.ResultError<UserCode> Error { get; set; }
        }

        public enum UserCode
        {
            NoSuchUser,
            AlreadyMember,
            InviteeIsNotAuthorized,
            Error,
        }
    }

    public enum InviteToEventCode
    {
        EventIsInPast,
        NoSuchEvent,
        NoSuchInviter,
        UnknownEventVisibility,
        InviterIsNotAuthorized,
        NobodyToInvite,
        MultipleErrors,
    }
}
