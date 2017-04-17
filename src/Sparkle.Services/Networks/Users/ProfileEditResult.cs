
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class ProfileEditResult : BaseResult<ProfileEditRequest, ProfileEditError>
    {
        public ProfileEditResult(ProfileEditRequest request)
            : base(request)
        {
        }
    }

    public enum ProfileEditError
    {
        NoSuchUser,
        InactiveUser,
        NoSuchIndustry,
        NoSuchCountry

    }
}
