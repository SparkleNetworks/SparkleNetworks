
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ValidateApplyRequestRequest : BaseRequest
    {
        public ValidateApplyRequestRequest()
        {
        }
    }

    public class ValidateApplyRequestResult : BaseResult<ValidateApplyRequestRequest, ValidateApplyRequestError>
    {
        public ValidateApplyRequestResult(ValidateApplyRequestRequest request)
            : base(request)
        {
        }

        public string GoUrl { get; set; }
    }

    public enum ValidateApplyRequestError
    {
        NoSuchItem,
        InvalidSecret,
        Refused,
        PendingAccept,
        UnknownState,
    }
}
