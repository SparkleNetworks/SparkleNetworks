
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Linq;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public class PollsRepository : BaseNetworkRepository<Poll, int>, IPollsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PollsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Polls)
        {
        }

        protected override Poll GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(Poll item)
        {
            return item.Id;
        }

        public IDictionary<int, Poll> GetById(int[] ids, int networkId)
        {
            return this.Set
                .Where(p => ids.Contains(p.Id) /*&& p.User.NetworkId == networkId*/)
                .ToDictionary(p => p.Id, p => p);
        }
    }

    public class PollsChoicesRepository : BaseNetworkRepository<PollChoice, int>, IPollsChoicesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PollsChoicesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PollChoices)
        {
        }

        protected override PollChoice GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(PollChoice item)
        {
            return item.Id;
        }

        public IList<PollChoice> GetByPollId(int pollId)
        {
            return this.Set
                .Where(c => c.PollId == pollId)
                .ToList();
        }
    }

    public class PollsAnswersRepository : BaseNetworkRepository<PollAnswer, int>, IPollsAnswersRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PollsAnswersRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.PollAnswers)
        {
        }

        protected override PollAnswer GetById(NetworksEntities model, int id)
        {
            return this.GetSet(model).SingleOrDefault(x => x.Id == id);
        }

        protected override int GetEntityId(PollAnswer item)
        {
            return item.Id;
        }
    }
}
