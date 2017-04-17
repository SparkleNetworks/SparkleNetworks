
namespace Sparkle.Services.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public interface ICreateNetworkRequestsService
    {
        CreateNetworkRequest Create(CreateNetworkRequest request);
        CreateNetworkRequest Insert(CreateNetworkRequest request);
        CreateNetworkRequest Update(CreateNetworkRequest request);
    }
}
