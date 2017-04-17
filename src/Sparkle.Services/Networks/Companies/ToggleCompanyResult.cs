
namespace Sparkle.Services.Networks.Companies
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ToggleCompanyResult : BaseResult<ToggleCompanyRequest, ToggleCompanyError>
    {
        public ToggleCompanyResult(ToggleCompanyRequest request)
            : base(request)
        {
        }
    }

    public enum ToggleCompanyError
    {
        NoSuchCompany,
        NoSuchUser,
        AlreadyEnabled,
        AlreadyDisabled,
        UnknownError

    }
}
