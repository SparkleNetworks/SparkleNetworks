
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class SetProfilePictureResult : BaseResult<SetProfilePictureRequest, SetProfilePictureError>
    {
        public SetProfilePictureResult(SetProfilePictureRequest request)
            : base(request)
        {
        }

        public Entities.Networks.User User { get; set; }
    }

    public enum SetProfilePictureError
    {
        FileIsNotPicture,
        NoSuchPartnerResource

    }
}
