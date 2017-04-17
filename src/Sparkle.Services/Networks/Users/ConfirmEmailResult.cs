
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Entities.Networks;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ConfirmEmailResult : BasicResult<ConfirmEmailError>
    {
        public int ActionId { get; set; }

        public string Secret { get; set; }

        public UserActionKey Action { get; set; }

        public User User { get; set; }
    }

    public enum ConfirmEmailError
    {
        NoSuchActionKey,
        EmailActivationActionExpired,
        InvalidSecret,
        NoSuchEmail,
        EmailAlreadyConfirmed,
        UserIsDisabled,
        AccountPendingValidation,
    }
}
