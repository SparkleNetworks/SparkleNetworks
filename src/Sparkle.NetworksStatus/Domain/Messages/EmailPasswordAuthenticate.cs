
namespace Sparkle.NetworksStatus.Domain.Messages
{
    using Sparkle.NetworksStatus.Domain.Lang;
    using Sparkle.NetworksStatus.Domain.Models;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EmailPasswordAuthenticateRequest : BaseRequest
    {
        public EmailPasswordAuthenticateRequest()
        {
        }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        [EmailAddressEx]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        [StringLength(9999, ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "StringLength")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(ValidationStrings), Name = "Label_Password")]
        public string Password { get; set; }

        public string RemoteAddress { get; set; }

        public string UserAgent { get; set; }
    }

    public class EmailPasswordAuthenticateResult : BaseResult<EmailPasswordAuthenticateRequest, EmailPasswordAuthenticateError>
    {
        public EmailPasswordAuthenticateResult(EmailPasswordAuthenticateRequest request)
            : base(request)
        {
        }

        public UserModel User { get; set; }

        public EmailAddressModel EmailAddress { get; set; }

        public DateTime? LastLoginDateUtc { get; set; }
    }

    public enum EmailPasswordAuthenticateError : byte
    {
        // DO NOT INSERT/REMOVE/EDIT ELEMENTS IN THE MIDDLE OF THE LIST
        // THE BACKING VALUE IS SIGNIFICANT AND IS STORED IN DATABASE
        // INSERT AT THE END OF THE LIST
        NoSuchEmail = 1,
        InvalidEmailAddress,
        UnconfirmedEmailAddress,
        NoPassword,
        PasswordIsLockedOut,
        IsNotAuthorized,
        WrongPassword,
        NoUserForEmailAddress,
        ClosedEmailAddress,

        // ALWAYS INSERT AT THE BOTTOM OF THE LIST
        InvalidRemoteAddress,
    }
}
