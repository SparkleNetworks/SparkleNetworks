
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IProfileFieldsRepository : IBaseNetworkRepository<ProfileField, int>
    {
        int Count();

        IList<ProfileField> GetUserFields();

        IList<ProfileField> GetAll();

        IList<ProfileField> GetCompanyFields();
    }
}
