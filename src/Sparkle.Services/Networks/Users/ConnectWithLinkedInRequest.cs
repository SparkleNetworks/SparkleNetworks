
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.LinkedInNET.OAuth2;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ConnectWithLinkedInRequest : BaseRequest
    {
        public AuthorizationAccessToken AccessToken { get; set; }
    }

    public class ConnectWithLinkedInResult : BaseResult<ConnectWithLinkedInRequest, ConnectWithLinkedInError>
    {
        public ConnectWithLinkedInResult(ConnectWithLinkedInRequest request)
            : base(request)
        {
        }

        public Entities.Networks.User UserMatch { get; set; }

        public bool PartialMatch { get; set; }

        public string LinkedInUserId { get; set; }

        public string LinkedInEmail { get; set; }
    }

    public enum ConnectWithLinkedInError
    {
        LinkedInNotConfigured,
        ApiCallFailed,
        NoSuchUser,
        UserEmailMatch,
        LinkedInLoginIsDisabled,
    }
}
