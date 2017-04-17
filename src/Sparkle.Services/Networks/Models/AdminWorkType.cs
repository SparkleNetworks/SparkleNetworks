
namespace Sparkle.Services.Networks.Models
{
    using System;

    public enum AdminWorkType
    {
        NewApplyRequestToManage,
        NewCompanyRequestToManage,
        UserEmailChangeRequest,
        RegisterRequest,
        CompanyHasNoAdministrator,
        PartnerResourceToApprove,
        NewAdToApprove,
        EditAdToApprove,
    }
}
