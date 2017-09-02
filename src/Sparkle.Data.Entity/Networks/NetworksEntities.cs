
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Data.EntityClient;
    using System.Data.Objects;

    public partial class NetworksEntities : ObjectContext
    {
        private bool isTransactional = false;

        public NetworksEntities(EntityConnection connection, bool isTransactional)
            : base(connection, "NetworksEntities")
        {
            this.isTransactional = isTransactional;

            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }

        public bool IsTransactionnal
        {
            get { return this.isTransactional; }
        }
    }
}
