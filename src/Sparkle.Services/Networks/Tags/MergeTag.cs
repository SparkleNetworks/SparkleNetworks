
namespace Sparkle.Services.Networks.Tags
{
    using Sparkle.Services.Networks.Models.Tags;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MergeTagRequest : BaseRequest
    {
        public TagModelType Type { get; set; }
        public int Id { get; set; }
        public string NewName { get; set; }
        public bool AllNetworks { get; set; }

        public IList<TagModel> AvailableTags { get; set; }
    }

    public class MergeTagResult : BaseResult<MergeTagRequest, MergeTagError>
    {
        public MergeTagResult(MergeTagRequest request)
            : base(request)
        {
        }
    }

    public enum MergeTagError
    {
    }
}
