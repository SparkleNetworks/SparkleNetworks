
namespace Sparkle.Data.Networks
{
    using System.Data;

    partial interface IRepositoryFactory
    {
        IDataTransaction NewTransaction();
        IDataTransaction NewTransaction(IDataTransaction parentTransaction);

        IRepositoryFactory Clone();
        void BeginTransaction(IsolationLevel isolation);
        void CompleteTransaction();
        void AbortTransaction();
        void AbortTransaction(bool throwIfAlreadyClosed);

        IDbCommand CreateStoreCommand();
    }
}
