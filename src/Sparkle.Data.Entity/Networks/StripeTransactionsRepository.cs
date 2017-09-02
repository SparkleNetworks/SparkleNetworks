
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StripeTransactionsRepository : BaseNetworkRepositoryInt<StripeTransaction>, IStripeTransactionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public StripeTransactionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.StripeTransactions)
        {
        }

        public void ChargeIsCreated(StripeTransaction item, string chargeId)
        {
            item.ChargeId = chargeId;
            item.IsChargeCreated = true;
            item.DateUpdatedUtc = DateTime.UtcNow;

            this.Update(item);
        }

        public void SetAmount(StripeTransaction item, decimal? priceEur = null, decimal? priceUsd = null)
        {
            if (!priceEur.HasValue && !priceUsd.HasValue)
                throw new ArgumentNullException("priceEur & priceUsd are null, one should not be");

            if (priceEur.HasValue)
                item.AmountEur = priceEur;
            else
                item.AmountUsd = priceUsd;
            item.DateUpdatedUtc = DateTime.UtcNow;

            this.Update(item);
        }

        public void ChargeIsCaptured(StripeTransaction item)
        {
            item.IsChargeCaptured = true;
            item.DateUpdatedUtc = DateTime.UtcNow;

            this.Update(item);
        }
    }
}
