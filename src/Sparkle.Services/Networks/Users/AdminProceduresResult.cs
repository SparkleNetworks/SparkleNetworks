
namespace Sparkle.Services.Networks.Users
{
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdminProceduresResult : BaseResult<AdminProceduresRequest, AdminProceduresError>
    {
        public AdminProceduresResult(AdminProceduresRequest request)
            : base(request)
        {
        }
    }

    public enum AdminProceduresError
    {
        NoSuchUser,
        AlreadyPending,
        ForbiddenEmail,
        UserIsInactive,
        NoSuchCompany
    }
}
