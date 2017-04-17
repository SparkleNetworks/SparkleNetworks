
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class ValidateInvitationCodeResult : BasicResult<ValidateInvitationCodeError>
    {
        public Models.UserInvitationModel Invitation { get; set; }

        public bool IsValid { get; set; }
    }

    public enum ValidateInvitationCodeError
    {
        AlreadyUsed,
        NoSuchCode
    }
}
