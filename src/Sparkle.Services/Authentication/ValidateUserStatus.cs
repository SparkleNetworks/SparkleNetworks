
namespace Sparkle.Services.Authentication
{
    public enum ValidateUserStatus
    {
        Ok,
        InvalidUsername,
        FailedPasswordAttemptCountExceeded,
        UnrecognizedPasswordFormat,
        InvalidPassword,
        NotActivated,
        NotAllowed,
        LockedOut,
        UnknownReason,
        MustChangePassword,
    }
}
