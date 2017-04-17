
namespace Sparkle.Services.Networks.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SrkToolkit.Domain;

    public class ChangeUserCultureResult : BaseResult<ChangeUserCultureRequest, ChangeUserCultureError>
    {
        public ChangeUserCultureResult(ChangeUserCultureRequest request)
            : base(request)
        {
        }
    }

    public enum ChangeUserCultureError
    {
        MustBeAuthenticated,
        NoSuchUser

    }
}
