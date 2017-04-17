
namespace Sparkle.Data.Networks
{
    using System;
    using System.Data;
    using System.Data.Common;

    public interface IDataTransaction : IDisposable
    {
        IRepositoryFactory Repositories { get; }
        DbConnection EntityConnection { get; }
        DbConnection StoreConnection { get; }

        IDisposable BeginTransaction(IsolationLevel level = IsolationLevel.Unspecified);
        void CompleteTransaction();
        void AbortTransaction();
        void AbortTransaction(bool throwIfAlreadyClosed);
        void ExecuteChanges();
    }
}
