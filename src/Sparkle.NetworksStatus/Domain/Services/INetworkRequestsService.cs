
namespace Sparkle.NetworksStatus.Domain.Services
{
    using Sparkle.NetworksStatus.Domain.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface INetworkRequestsService
    {
        NetworkRequestModel GetCreateRequest(NetworkRequestModel model);

        BasicResult<CreateNetworkRequestError, NetworkRequestModel> Create(NetworkRequestModel model);

        NetworkRequestModel GetByWebId(Guid webId);

        int Count();
    }

    public enum CreateNetworkRequestError
    {
        ReservedSubdomainName,
        InvalidEmailAddress,
        InvalidCulture,
        VerifyNetworkSubdomain,
    }
}
