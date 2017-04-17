
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;

    public interface IPollsChoicesService
    {
        int CreatePollChoice(PollChoice item);
        List<PollChoice> SelectAll();
        List<PollChoice> SelectByPollId(int choiceId);
        long Update(PollChoice item);
    }
}
