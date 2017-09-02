
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Repository]
    public interface IStripeTransactionsRepository : IBaseNetworkRepository<StripeTransaction>
    {
        StripeTransaction GetById(int stripeTransactionId);

        void ChargeIsCreated(StripeTransaction item, string chargeId);
        void SetAmount(StripeTransaction item, decimal? priceEur = null, decimal? priceUsd = null);
        void ChargeIsCaptured(StripeTransaction item);
    }
}