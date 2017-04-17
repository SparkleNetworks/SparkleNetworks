using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ICompanyAdminService
    {
        int Insert(CompanyAdmin item);
        void Delete(CompanyAdmin item);
    }
}
