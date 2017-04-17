
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AddGroupTagResult : BaseResult<AddGroupTagRequest, AddGroupTagError>
    {
        public AddGroupTagResult(AddGroupTagRequest request)
            : base(request)
        {
            this.OptionnalMessage = null;
        }

        public int TagId { get; set; }

        public string OptionnalMessage { get; set; }
    }

    public enum AddGroupTagError
    {
        NoSuchTag,
        NoSuchGroup,
        NoSuchTagInGroup,
        AlreadyAdded,
        AlreadyDeleted,
        CannotAddDeletedTag,
        CannotAddDeletedTagDefault,
    }
}
