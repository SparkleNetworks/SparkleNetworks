
namespace Sparkle.Services.Networks.Users
{
    using System;
    using SrkToolkit.Domain;

    public class SendPasswordRecoveryEmailResult : BasicResult<SendPasswordRecoveryEmailError>
    {
        public SendPasswordRecoveryEmailResult()
        {
        }

        public Uri PasswordResetLink { get; set; }
    }

    public enum SendPasswordRecoveryEmailError
    {
        UserIsNotAuthorized,
        NoSuchUser,
        EmailInternalError,
        EmailProviderError,
    }
}
