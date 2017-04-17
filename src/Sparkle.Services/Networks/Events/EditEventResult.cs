
namespace Sparkle.Services.Networks.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SrkToolkit.Domain;

    public class EditEventResult : BaseResult<EditEventRequest, EditEventError>
    {
        public EditEventResult(EditEventRequest request)
            : base(request)
        {
        }

        public Entities.Networks.EventMember EventMemberEntity { get; set; }

        public Entities.Networks.Event EventEntity { get; set; }

        public Models.EventModel Event { get; set; }

        public Models.EventMemberModel EventMember { get; set; }
    }

    public enum EditEventError
    {
        UserIsNotGroupMember,
        NoSuchUser,
        NoSuchCategory,
        NoSuchVisibility,
        NoSuchPlace,
        NoSuchEvent,
        CannotAdministrateEvent,
    }
}
