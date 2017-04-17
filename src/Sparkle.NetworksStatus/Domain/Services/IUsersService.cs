
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Domain.Messages;
    using Sparkle.NetworksStatus.Domain.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface IUsersService
    {
        IList<UserModel> GetAllSortedById(int id, int pageSize);

        int Count();

        BasicResult<EditUserError, UserModel> Create(UserModel model);

        UserModel GetById(int id, bool withPrimaryEmail = true, bool withEmailAuths = false);

        IList<EmailAddressAuthenticationModel> GetEmailAddressAuthentications(int userId);

        EmailAddressAuthenticationModel GetEmailAddressAuthenticationById(int id);

        BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel> CreateEmailAddressAuthentications(EmailAddressAuthenticationModel model);

        BasicResult<EmailAddressAuthenticationError, EmailAddressAuthenticationModel> EditEmailAddressAuthentications(EmailAddressAuthenticationModel model);

        EmailPasswordAuthenticateResult EmailPasswordAuthenticate(EmailPasswordAuthenticateRequest model);
    }
}
