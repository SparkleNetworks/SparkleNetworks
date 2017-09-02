
namespace Sparkle.Services.Networks.Models.Timelines
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AddTimelineTagResult : BaseResult<AddTimelineTagRequest, AddTimelineTagError>
    {
        public AddTimelineTagResult(AddTimelineTagRequest request)
            : base(request)
        {
        }

        public int TagId { get; set; }

        public Tags.TagModel Tag { get; set; }
    }

    public enum AddTimelineTagError
    {
        NoSuchTag,
        NoSuchTimelineItem,
        NoSuchTagInTimelineItem,
        AlreadyAdded,
        AlreadyDeleted,
        CannotAddDeletedTag,
        CannotAddDeletedTagDefault,
        NotAuthorized,
    }

    public enum RemoveTimelineTagError
    {
        NoSuchRelation,
        AlreadyDeleted,
        NotAuthorized,
    }
}
