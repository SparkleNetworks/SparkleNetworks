
namespace Sparkle.Services.Networks.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;

    public class DeleteEventRequest : BaseRequest
    {
        public WallItemDeleteReason Reason { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }
    }

    public class DeleteEventResult : BaseResult<DeleteEventRequest, DeleteEventError>
    {
        public DeleteEventResult(DeleteEventRequest request)
            : base(request)
        {
        }
    }

    public enum DeleteEventError
    {
        NoSuchItem,
        NoSuchUserOrInactive,
        NotAuthorized
    }
}
