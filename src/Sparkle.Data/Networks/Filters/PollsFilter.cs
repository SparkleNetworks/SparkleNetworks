
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public static class PollsFilter
    {
        public static IQueryable<Poll> WithUserId(this IQueryable<Poll> qry, int userId)
        {
            return qry.Where(o => o.CreatedByUserId == userId);
        }

        public static IQueryable<Poll> WithId(this IQueryable<Poll> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<PollChoice> WithPollId(this IQueryable<PollChoice> qry, int pollId)
        {
            return qry.Where(o => o.PollId == pollId);
        }

        public static IQueryable<PollAnswer> WithChoiceId(this IQueryable<PollAnswer> qry, int choiceId)
        {
            return qry.Where(o => o.ChoiceId == choiceId);
        }

        public static IQueryable<PollAnswer> WithpollAndChoiceId(this IQueryable<PollAnswer> qry, int pollId, int choiceId, int userId)
        {
            return qry.Where(o => o.UserId == userId && o.PollId == pollId && o.ChoiceId == choiceId);
        }
    }
}
