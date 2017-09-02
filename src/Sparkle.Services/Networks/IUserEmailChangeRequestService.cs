
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IUserEmailChangeRequestService
    {
        IList<UserEmailChangeRequest> SelectByUserId(int userId);

        UserEmailChangeRequest SelectPendingRequestFromUserId(int userId);

        void CancelPendingRequest(UserEmailChangeRequest item);

        AdminProceduresRequest GetAdminProceduresRequestFromLogin(string login);

        AdminProceduresResult AddUserEmailChangeRequest(AdminProceduresRequest request, int currentUserId);

        bool IsEmailAvailable(EmailAddress emailAddress, int userId);

        bool IsEmailForbiddenOrPending(EmailAddress emailAddress, int userId);

        UserEmailChangeRequest SelectById(int id);

        void ValidatePendingRequest(UserEmailChangeRequest pending);

        IList<UserEmailChangeRequest> GetPendingRequests();

        UserEmailChangeRequestModel GetById(int id);

        IList<UserEmailChangeRequestModel> GetAll();
    }
}
