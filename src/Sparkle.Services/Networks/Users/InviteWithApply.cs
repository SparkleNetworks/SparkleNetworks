
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class InviteWithApplyRequest : BaseRequest
    {
        public InviteWithApplyRequest()
        {
        }

        public int ActingUserId { get; set; }

        public string Email { get; set; }

        public bool SkipApproval { get; set; }

        public short? CompanyCategoryId { get; set; }

        public int? CompanyRelationshipTypeId { get; set; }
    }

    public class InviteWithApplyResult : BaseResult<InviteWithApplyRequest, InviteWithApplyError>
    {
        public InviteWithApplyResult(InviteWithApplyRequest request)
            : base(request)
        {
        }

        public ApplyRequestModel Item { get; set; }
    }

    public enum InviteWithApplyError
    {
        NotAuthorized,
        NoSuchActingUser,
        InvalidEmailAddress,
        InvalidCompanyCategory,
        InvalidCompanyRelationship,
        EmailAddressInUse,
    }
}
