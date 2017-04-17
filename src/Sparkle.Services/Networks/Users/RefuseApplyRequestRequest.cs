
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class RefuseApplyRequestRequest : BaseRequest
    {
        private bool isSpam = false;

        public RefuseApplyRequestRequest()
        {
        }

        public RefuseApplyRequestRequest(int itemId, int userId)
        {
            this.Id = itemId;
            this.UserId = userId;
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        [Display(Name = "IsSpam", ResourceType = typeof(NetworksLabels))]
        public bool IsSpam
        {
            get { return this.isSpam; }
            set { this.isSpam = value; }
        }

        [Display(Name = "Remark", ResourceType = typeof(NetworksLabels))]
        public string Remark { get; set; }

        public bool ToDelete { get; set; }
    }

    public class RefuseApplyRequestResult : BaseResult<RefuseApplyRequestRequest, RefuseApplyRequestError>
    {
        public RefuseApplyRequestResult(RefuseApplyRequestRequest request)
            : base(request)
        {
        }
    }

    public enum RefuseApplyRequestError
    {
        NoSuchApplyRequest,
        PendingEmailConfirmation,
        AlreadyAccepted,
        AlreadyRefused,
    }
}
