
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IServiceFactoryEx : IServiceFactory
    {
        IRepositoryFactory Repositories { get; }
        IRepositoryFactory NewTransaction { get; }
        IEmailService EmailService { get; }
        ServicesConfiguration Configuration { get; }
    }
}
