
namespace Sparkle.Services.Networks.Events
{
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ValidateUserCalendarTokenRequest : LocalBaseRequest
    {
        public ValidateUserCalendarTokenRequest()
        {
        }

        public string Token { get; set; }
    }

    public class ValidateUserCalendarTokenResult : BaseResult<ValidateUserCalendarTokenRequest, ValidateUserCalendarTokenError>
    {
        public ValidateUserCalendarTokenResult(ValidateUserCalendarTokenRequest request)
            : base(request)
        {
        }
    }

    public enum ValidateUserCalendarTokenError
    {
        NoSuchUser,
        NotAuthorized,
        InvalidToken,
        WrongToken,
    }
}
