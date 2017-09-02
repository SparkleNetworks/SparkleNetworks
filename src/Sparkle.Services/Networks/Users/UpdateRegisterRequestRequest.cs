
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UpdateRegisterRequestRequest : BaseRequest
    {
        public int Id { get; set; }

        public Entities.Networks.RegisterRequestStatus NewStatus { get; set; }
    }

    public class UpdateRegisterRequestResult : BaseResult<UpdateRegisterRequestRequest, UpdateRegisterRequestCode>
    {
        public UpdateRegisterRequestResult(UpdateRegisterRequestRequest request)
            : base(request)
        {
        }

        public RegisterRequestModel ItemBefore { get; set; }

        public InvitePersonResult InviteResult { get; set; }

        public RegisterRequestModel ItemAfter { get; set; }
    }

    public enum UpdateRegisterRequestCode
    {
        NoSuchRequest,
        RequestAlreadyHandled,
        InviteError,
        InvalidNewStatus,
        Unauthorized,
    }
}
