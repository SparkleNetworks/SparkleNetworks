
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AcceptApplyRequestRequest : BaseRequest
    {
        public Guid ApplyKey { get; set; }

        public int? UserId { get; set; }

        public bool NotifyNewlyCreatedUser { get; set; }

        public IDictionary<int, string> AvailableCompanies { get; set; }

        [Display(Name = "IsWrongCompany", ResourceType = typeof(NetworksLabels))]
        public bool IsWrongCompany { get; set; }

        public int JoinCompanyId { get; set; }

        public Dictionary<int, string> CompaniesNameMatch { get; set; }
    }

    public class AcceptApplyRequestResult : BaseResult<AcceptApplyRequestRequest, AcceptApplyRequestError>
    {
        public AcceptApplyRequestResult(AcceptApplyRequestRequest request)
            : base(request)
        {
        }

        public CreateCompanyResult CreateCompanyResult { get; set; }

        public CreateEmailPassordAccountResult CreateUserResult { get; set; }
    }

    public enum AcceptApplyRequestError
    {
        NoSuchApplyRequest,
        CreateCompanyRequestFailed,
        CreateEmailPasswordAccountFailed,
        OnForwardRequestNoSuchCompany,
        NotInPendingAccept,
        JoinCompanyIsDisabled,
    }
}
