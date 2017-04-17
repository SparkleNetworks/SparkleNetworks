
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class DeleteInvitationRequest : BaseRequest
    {
        public DeleteInvitationRequest()
        {
        }

        public int InvitationId { get; set; }

        public int ActingUserId { get; set; }
    }

    public class DeleteInvitationResult : BaseResult<DeleteInvitationRequest, DeleteInvitationError>
    {
        public DeleteInvitationResult(DeleteInvitationRequest request)
            : base(request)
        {
        }
    }

    public enum DeleteInvitationError
    {
        NoSuchInvitation,
        NoSuchUser,
        Unauthorized,
        InvitationAlreadyUsed,
    }
}
