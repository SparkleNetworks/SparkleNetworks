using System;
using System.Collections.Generic;
using System.Linq;
using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    public class PeopleVisitsService : ServiceBase, IPeopleVisitsService
    {
        internal PeopleVisitsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IPeopleVisitsRepository peopleVisitsRepository
        {
            get { return this.Repo.PeopleVisits; }
        }

        public int Insert(UsersVisit item)
        {
            return this.peopleVisitsRepository.Insert(item).Id;
        }

        public UsersVisit Update(UsersVisit item)
        {
            return this.peopleVisitsRepository.Update(item);
        }

        public void Delete(UsersVisit item)
        {
            this.peopleVisitsRepository.Delete(item);
        }

        public UsersVisit GetByProfileAndUserAndDay(int profileId, int userId, DateTime date)
        {
            date = date.Date;

            return this.peopleVisitsRepository
               .Select()
               .Where(o => o.ProfileId == profileId && o.UserId == userId && o.Date == date)
               .FirstOrDefault();
        }

        public int CountByProfileAndDay(int profileId, DateTime date)
        {
            date = date.Date;

            var visites = this.peopleVisitsRepository
                .Select()
                .Where(o => o.ProfileId == profileId && o.Date == date)
                .ToList();

            return visites.Aggregate(0, (current, visit) => current + visit.ViewCount);
        }

        /// <summary>
        /// Counts the visits for specified person id.
        /// </summary>
        /// <param name="personId">The person id.</param>
        /// <returns></returns>
        public int Count(int personId)
        {
            return this.peopleVisitsRepository
                .Select()
                .Where(o => o.ProfileId == personId)
                .Count();
        }

        public List<UsersVisit> GetByProfileAndDay(int profileId, DateTime date)
        {
            date = date.Date;

            return this.peopleVisitsRepository
                .Select()
                .Where(o => o.ProfileId == profileId && o.Date == date)
                .ToList();
        }

        public int CountByProfileLastTwoWeeks(int profileId, DateTime date)
        {
            date = date.Date.AddDays(-15D);

            return this.peopleVisitsRepository
                .Select()
                .Where(o => o.ProfileId == profileId && o.Date > date)
                .Count();
        }

        public IList<UsersVisit> SelectAll()
        {
            return this.peopleVisitsRepository
                    .Select()
                    .ToList();
        }

        public IList<UsersVisit> GetByProfile(int personId)
        {
            return this.peopleVisitsRepository
               .Select()
               .Where(o => o.ProfileId == personId)
               .ToList();
        }
    }
}
