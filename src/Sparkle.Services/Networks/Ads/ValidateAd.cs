
namespace Sparkle.Services.Networks.Ads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class ValidateAdRequest : BaseRequest
    {
        public ValidateAdRequest()
        {
        }

        public int ActingUserId { get; set; }

        public int Id { get; set; }

        public bool Accept { get; set; }

        public DateTime PendingEditDate { get; set; }
    }

    public class ValidateAdResult : BaseResult<ValidateAdRequest, ValidateAdError>
    {
        public ValidateAdResult(ValidateAdRequest request)
            : base(request)
        {
        }

        public AdModel Item { get; set; }
    }

    public enum ValidateAdError
    {
        Invalid,
        NoSuchActingUser,
        NotAuthorized,
        NoSuchItem,
        AlreadyValidated,
        CannotValidateWhenClosed,
        CannotAcceptWhenUserNotActive,
        ConcurrencyOnPendingEditDate,
    }
}
