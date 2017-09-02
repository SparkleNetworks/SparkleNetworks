
namespace Sparkle.NetworksStatus.Domain
{
    using Sparkle.NetworksStatus.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class BaseService : Disposable
    {
        protected IRepositoryFactory Repos
        {
            get { return this.serviceFactory.Repositories; }
        }
    }
}
