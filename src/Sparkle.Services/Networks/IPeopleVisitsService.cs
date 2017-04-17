using System;
using Sparkle.Entities.Networks;
using System.Collections.Generic;

namespace Sparkle.Services.Networks
{
    public interface IPeopleVisitsService
    {
        int Insert(UsersVisit item);
        UsersVisit Update(UsersVisit item);
        void Delete(UsersVisit item);

        UsersVisit GetByProfileAndUserAndDay(int profileId, int userId, System.DateTime dateTime);
        int CountByProfileAndDay(int profileId, DateTime date);

        System.Collections.Generic.List<UsersVisit> GetByProfileAndDay(int profileId, DateTime date);

        int CountByProfileLastTwoWeeks(int personId, DateTime date);

        int Count(int personId);

        IList<UsersVisit> GetByProfile(int personId);
    }
}
