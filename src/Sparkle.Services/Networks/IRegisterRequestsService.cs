
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Common;
    using Sparkle.Entities.Networks;
    using SrkToolkit.Common.Validation;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;

    public interface IRegisterRequestsService
    {
        EmitRegisterRequestResult EmitRegisterRequest(EmailAddress emailAddress, int companyId);

        IList<RegisterRequest> GetPendingRegisterRequests(RegisterRequestOptions options);

        RegisterRequest GetRegisterRequestById(int id);

        bool CanAccept(RegisterRequest request);

        IList<RegisterRequest> GetRegisterRequests(RegisterRequestStatus registerRequestStatus);

        RegisterRequest MarkAccepted(RegisterRequest request, int userId, int? invitedId);

        RegisterRequest MarkInCommunication(RegisterRequest request, int userId);

        RegisterRequest MarkDenied(RegisterRequest request, int userId);

        int CountPending();

        int CountPendingByCompany(int companyId);

        RegisterRequestModel GetByCode(Guid id);
        RegisterRequestModel GetByCode(Guid id, RegisterRequestOptions options);

        IList<RegisterRequestModel> GetAllByCompany(int companyId);

        IList<UpdateRegisterRequestResult> Update(IList<UpdateRegisterRequestRequest> request, int invitingUserId, int companyId);

        int? GetCompanyIdFromRequestsIds(int[] registerRequestsIds);

        RegisterRequestModel GetById(int id);
    }
}
