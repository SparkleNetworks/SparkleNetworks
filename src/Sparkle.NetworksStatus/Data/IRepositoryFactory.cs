
namespace Sparkle.NetworksStatus.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial interface IRepositoryFactory : IDisposable
    {
        IRepositoryFactory BeginTransaction();
        IRepositoryFactory CompleteTransaction();
        IRepositoryFactory AbortTransaction();
        void ClearTransaction();
    }
}
