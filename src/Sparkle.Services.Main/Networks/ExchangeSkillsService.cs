
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class ExchangeSkillsService : ServiceBase, IExchangeSkillsService
    {
        [DebuggerStepThrough]
        internal ExchangeSkillsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory) 
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IExchangeSkillsRepository exchangeSkillsRepository
        {
            get { return this.Repo.ExchangeSkills; }
        }

        public IList<ExchangeSkill> GetActiveExchanges()
        {
            return this.exchangeSkillsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .ToList();
        }

        public int Add(ExchangeSkill item)
        {
            this.SetNetwork(item);

            return this.exchangeSkillsRepository.Insert(item).Id;
        }
    }
}
