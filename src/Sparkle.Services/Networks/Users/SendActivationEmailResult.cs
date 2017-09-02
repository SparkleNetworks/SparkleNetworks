
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SendActivationEmailResult : BasicResult<SendActivationEmailError>
    {
        public string Email { get; set; }
        public User User { get; set; }
        public UserActionKey ActionKey { get; set; }
    }

    public enum SendActivationEmailError
    {
        NoSuchUserByEmail,
        EmailAlreadyConfirmed,
        NoEmailActivationAction,
        EmailActionDoneTooManyTimes,
        EmailActivationActionExpired,
        EmailProviderError,
    }
}
