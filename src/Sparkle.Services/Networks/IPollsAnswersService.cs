
namespace Sparkle.Services.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;

    public interface IPollsAnswersService
    {
        int CreatePollAnswer(PollAnswer item);
        void DeletePollAnswer(PollAnswer item);
        List<PollAnswer> SelectAll();
        List<PollAnswer> SelectByChoiceId(int choiceId);
        PollAnswer SelectByPollAndChoiceId(int pollId, int choiceId, int userId);
        long Update(PollAnswer item);
    }
}
