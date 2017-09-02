
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class SetSingleProfileFieldRequest : BaseRequest
    {
        public SetSingleProfileFieldRequest()
        {
        }

        public Guid? ApplyKey { get; set; }

        public int ProfileFieldId { get; set; }

        public string Value { get; set; }

        public string Target { get; set; }

        public string UserRemoteAddress { get; set; }
    }

    public class SetSingleProfileFieldResult : BaseResult<SetSingleProfileFieldRequest, SetSingleProfileFieldError>
    {
        public SetSingleProfileFieldResult(SetSingleProfileFieldRequest request)
            : base(request)
        {
        }
    }

    public enum SetSingleProfileFieldError
    {
        Invalid,
        NoSuchItem,
        RequestIsNotNew,
        NoSuchProfileField,
        InvalidTargetForThisProfileField,
        InvalidTarget
    }
}
