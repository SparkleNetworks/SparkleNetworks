
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class ConfirmApplyRequestEmailAddressRequest : BaseRequest
    {
        public ConfirmApplyRequestEmailAddressRequest()
        {
        }

        public Guid Key { get; set; }

        public string Secret { get; set; }
    }

    public class ConfirmApplyRequestEmailAddressResult : BaseResult<ConfirmApplyRequestEmailAddressRequest, ConfirmApplyRequestEmailAddressError>
    {
        public ConfirmApplyRequestEmailAddressResult(ConfirmApplyRequestEmailAddressRequest request)
            : base(request)
        {
        }

        public bool EmailWasAlreadyConfirmed { get; set; }

        public ApplyRequestModel Model { get; set; }

        public bool EmailDomainMatch { get; set; }
    }

    public enum ConfirmApplyRequestEmailAddressError
    {
        NoSuchApplyRequest,
        InvalidSecret,
        Accepted,
        Refused,
        UnexpectedState,
        NotSubmitted,
        AcceptOnEmailDomainMatch,
    }
}
