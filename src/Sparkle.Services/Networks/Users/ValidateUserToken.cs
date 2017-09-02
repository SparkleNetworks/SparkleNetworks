
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;
    using Sparkle.Services.Networks.Models;

    public class ValidateUserTokenRequest : LocalBaseRequest
    {
        public ValidateUserTokenRequest()
        {
        }

        public string Token { get; set; }
    }

    public class ValidateUserTokenResult : BaseResult<ValidateUserTokenRequest, ValidateUserTokenError>
    {
        public ValidateUserTokenResult(ValidateUserTokenRequest request)
            : base(request)
        {
        }

        public UserModel User { get; set; }
    }

    public enum ValidateUserTokenError
    {
        NoSuchUser,
        NotAuthorized,
        InvalidToken,
    }
}
