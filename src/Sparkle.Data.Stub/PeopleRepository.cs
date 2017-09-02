using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparkle.Data.Networks;
using Sparkle.Entities;

namespace Sparkle.Data.Stub
{
    public class PeopleRepository : BaseStubRepository, IPeopleRepository
    {
        public IQueryable<Person> SelectPeoples(IList<string> options)
        {
            return this.Data.People.AsQueryable();
        }

        public IQueryable<Person> SelectPeoples()
        {
            return this.Data.People.AsQueryable();
        }

        public Guid InsertPeople(Person item)
        {
            item.UserID = this.Data.NextPersonGuid;
            this.Data.People.Add(item);
            return item.UserID;
        }

        public Person UpdatePeople(Person item, bool saveFriends)
        {
            return item;
        }

        public void DeletePeople(Person item)
        {
            throw new NotSupportedException();
        }

        public IQueryable<Person> SelectContacts(string request, Guid UserId, int take)
        {
            //return from p in this.Data.People
            //       let 

            //return from f in this.Data.Friendships
            //       where f.TargetId == UserId
            //       from p in this.Data.People
            //       where p.UserID

            throw new NotImplementedException();
        }

        public IQueryable<Person> SelectUnvitedByGroupId(string request, int? GroupId, Guid UserId, int take)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Person> SelectUnvitedByEventId(string request, int? EventId, Guid UserId, int take)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Person> NewQuery(Options.PersonOptions options) {
            throw new NotImplementedException();
        }

        public IList<Objects.PersonExtended> GetWithDetails() {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
