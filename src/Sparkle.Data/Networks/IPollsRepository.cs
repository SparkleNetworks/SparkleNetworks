
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IPollsRepository : IBaseNetworkRepository<Poll, int>
    {
        IDictionary<int, Poll> GetById(int[] ids, int networkId);
    }

    [Repository]
    public interface IPollsChoicesRepository : IBaseNetworkRepository<PollChoice, int>
    {
        IList<PollChoice> GetByPollId(int pollId);
    }

    [Repository]
    public interface IPollsAnswersRepository : IBaseNetworkRepository<PollAnswer, int>
    {
    }
}
