
namespace Sparkle.Services.Networks
{
    using System;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    public interface IPollsService
    {
        int CreatePoll(Poll item);
        IList<Poll> SelectAll();
        Poll SelectById(int pollId);
        IList<Poll> SelectLatestPollByUserId(int userId);
        long Update(Poll item);

        void Delete(Poll poll);

        IDictionary<int, Poll> GetById(int[] ids);
    }
}
