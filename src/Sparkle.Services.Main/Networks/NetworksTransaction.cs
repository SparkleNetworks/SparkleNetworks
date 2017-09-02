
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Services.Networks;
    using SrkToolkit;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NetworksTransaction : INetworksTransaction
    {
        private readonly IServiceFactory factory;
        private readonly IDataTransaction dataTransaction;
        private IList<Action<INetworksTransaction>> postSaveActions;
        private bool rootTransaction;
        private bool isDisposed;
        private bool isCompleted;

        [DebuggerStepThrough]
        public NetworksTransaction(IServiceFactory factory, IDataTransaction dataTransaction)
            : base()
        {
            this.factory = factory;
            this.dataTransaction = dataTransaction;

            this.rootTransaction = true;
        }

        [DebuggerStepThrough]
        public NetworksTransaction(IServiceFactory factory, IDataTransaction dataTransaction, IList<Action<INetworksTransaction>> postSaveActions)
            : base()
        {
            this.factory = factory;
            this.dataTransaction = dataTransaction;
            this.postSaveActions = postSaveActions;
        }

        public IRepositoryFactory Repositories
        {
            [DebuggerStepThrough]
            get { return this.dataTransaction.Repositories; }
        }

        public IServiceFactory Services
        {
            [DebuggerStepThrough]
            get { return this.factory; }
        }

        public IDataTransaction DataTransaction
        {
            get { return this.dataTransaction; }
        }

        public IList<Action<INetworksTransaction>> PostSaveActions
        {
            get { return this.postSaveActions ?? (this.postSaveActions = new List<Action<INetworksTransaction>>()); }
        }

        public IDisposable BeginTransaction()
        {
            var tran = this.dataTransaction.BeginTransaction();
            return new DisposableOnce(() =>
            {
                tran.Dispose();
                this.Dispose();
            });
        }

        public void CompleteTransaction()
        {
            this.dataTransaction.CompleteTransaction();
            this.isCompleted = true;

            if (rootTransaction)
            {
                var exceptions = new List<Exception>();
                foreach (var action in this.PostSaveActions)
                {
                    try
                    {
                        action(this);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count > 0)
                {
                    var aggregate = new AggregateException(exceptions);
                    throw aggregate;
                }

                this.postSaveActions.Clear();
            }
        }

        public void AbortTransaction()
        {
            this.dataTransaction.AbortTransaction();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.factory != null)
                        this.factory.Dispose();

                    if (this.postSaveActions != null)
                        this.postSaveActions.Clear();

                    if (this.dataTransaction != null)
                        this.dataTransaction.Dispose();
                }

                this.isDisposed = true;
            }
        }
    }
}
