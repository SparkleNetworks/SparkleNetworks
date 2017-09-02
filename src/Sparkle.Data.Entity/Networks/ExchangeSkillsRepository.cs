
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;

    public class ExchangeSkillsRepository : BaseNetworkRepositoryInt<ExchangeSkill>, IExchangeSkillsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ExchangeSkillsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ExchangeSkills)
        {
        }
    }
}
