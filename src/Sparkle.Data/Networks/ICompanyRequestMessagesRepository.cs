
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ICompanyRequestMessagesRepository : IBaseNetworkRepository<CompanyRequestMessage, int>
    {
    }
}