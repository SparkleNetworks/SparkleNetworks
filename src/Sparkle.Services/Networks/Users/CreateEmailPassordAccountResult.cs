
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class CreateEmailPassordAccountResult : BaseResult<CreateEmailPassordAccountRequest, CreateEmailPassordAccountError>
    {
        public CreateEmailPassordAccountResult(CreateEmailPassordAccountRequest request)
            : base(request)
        {
        }

        public UserModel User { get; set; }

        public UserActionKey ConfirmEmailActionKey { get; set; }

        public bool EmailSent { get; set; }

        public ValidateInvitationCodeResult CodeValidation { get; set; }
    }

    public enum CreateEmailPassordAccountError
    {
        UserEmailAlreadyExists,
        InvitedEmailAlreadyExists,
        InvitationOnly,
        RegisterInCompanyValueIsInvalid,
        InternalError1,
        InternalError2,
        InternalError3,
        InvalidInvitation,
        CompanyIsInactive,
        RegisterRequestEmailMismatch,
        JoinCompanyIdIsInvalid,
        NoCompanyFound,
    }
}
