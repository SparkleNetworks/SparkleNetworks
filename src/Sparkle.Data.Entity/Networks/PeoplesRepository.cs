
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using Sparkle.Data.Networks;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;

    public class PeopleRepository : BaseNetworkRepositoryInt<User>, IPeopleRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PeopleRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Users)
        {
        }

        public IQueryable<User> SelectContacts(string request, int userId, int take)
        {
            // TODO: @Kevin review this query (compare with previous version) and verify performance
            List<string> Includes = new List<string>() { "SeekFriends1", "SeekFriends", "Job", "Company", "GroupMembers" };

            IQueryable<User> ppls = this.SelectWithOptions(this.Set, Includes)
                .Where(o => o.SeekFriends1.Any(n => n.TargetId == userId)
                         || o.SeekFriends.Any(i => i.TargetId == userId));

            return ppls.Where(o => o.FirstName.ToLower().Contains(request.ToLower())
                                || o.LastName.ToLower().Contains(request.ToLower()))
                       .Take(take);
        }

        public IQueryable<User> SelectUnvitedByGroupId(string request, int? groupId, int userId, int take)
        {
            List<string> Includes = new List<string>() { "SeekFriends1", "SeekFriends", "Job", "Company", "GroupMembers" };

            // TODO: @Kevin review this query (compare with previous version) and verify performance
            IQueryable<User> ppls = this.SelectWithOptions(this.Set, Includes)
                .Where(o => o.SeekFriends1.Any(n => n.TargetId == userId)
                            ||
                            o.SeekFriends.Any(i => i.TargetId == userId));

            if (groupId != null)
            {
                return ppls.Where(o => o.GroupMembers.Count == 0 | o.GroupMembers.Any(x => x.GroupId != groupId.Value)
                            && o.FirstName.ToLower().Contains(request.ToLower()) | o.LastName.ToLower().Contains(request.ToLower()))
                            .Take(take);
            }
            else
            {
                return ppls.Where(o => o.FirstName.ToLower().Contains(request.ToLower()) | o.LastName.ToLower().Contains(request.ToLower()))
                            .Take(take);
            }
        }

        public IQueryable<User> SelectUnvitedByEventId(string request, int? eventId, int userId, int take)
        {
            List<string> Includes = new List<string>() { "SeekFriends1", "SeekFriends", "Job", "Company", "EventMembers" };

            // TODO: @Kevin review this query (compare with previous version) and verify performance
            IQueryable<User> ppls = this.SelectWithOptions(this.Set, Includes)
                .Where(o => o.SeekFriends1.Any(n => n.TargetId == userId)
                            ||
                            o.SeekFriends.Any(i => i.TargetId == userId));

            if (eventId != null)
            {
                return ppls.Where(o => o.EventMembers.Count == 0 | o.EventMembers.Any(x => x.EventId != eventId.Value)
                            && o.FirstName.ToLower().Contains(request.ToLower()) | o.LastName.ToLower().Contains(request.ToLower()))
                            .Take(take);
            }
            else
            {
                return ppls.Where(o => o.FirstName.ToLower().Contains(request.ToLower()) | o.LastName.ToLower().Contains(request.ToLower()))
                            .Take(take);
            }
        }

        public IList<UserExtended> GetWithDetails()
        {
            var companies = this.Context.Companies
                .Select(c => new Company
                {
                    ID = c.ID,
                    Name = c.Name,
                    Alias = c.Alias,
                });

            var jobs = this.Context.Jobs
                .Select(c => new Job
                {
                    Id = c.Id,
                    Libelle = c.Libelle,
                    Alias = c.Alias,
                });

            var query = this.Set
                .Select(p => new UserExtended
                {
                    Person = p,
                });

            return new PersonExtendedList(query)
            {
                Companies = companies.ToList(),
                Jobs = jobs.ToList(),
            };
        }

        public IQueryable<User> NewQuery(PersonOptions options)
        {
            ObjectQuery<User> query = this.Set;

            if ((options & Options.PersonOptions.Company) == Options.PersonOptions.Company)
                query = query.Include("Company");

            if ((options & Options.PersonOptions.Job) == Options.PersonOptions.Job)
                query = query.Include("Job");

            if ((options & Options.PersonOptions.Notification) == Options.PersonOptions.Notification)
                query = query.Include("Notification");

            if ((options & Options.PersonOptions.Interests) == Options.PersonOptions.Interests)
                query = query.Include("UserInterests");

            if ((options & Options.PersonOptions.Skills) == Options.PersonOptions.Skills)
                query = query.Include("UserSkills");

            if ((options & Options.PersonOptions.Recreations) == Options.PersonOptions.Recreations)
                query = query.Include("UserRecreations");

            if ((options & Options.PersonOptions.InterestsValues) == Options.PersonOptions.InterestsValues)
                query = query.Include("UserInterests.Interest");

            if ((options & Options.PersonOptions.SkillsValues) == Options.PersonOptions.SkillsValues)
                query = query.Include("UserSkills.Skill");

            if ((options & Options.PersonOptions.RecreationsValues) == Options.PersonOptions.RecreationsValues)
                query = query.Include("UserRecreations.Recreation");

            if ((options & PersonOptions.Contacts) == PersonOptions.Contacts)
                query = query.Include("Contacts");

            if ((options & PersonOptions.ContactsOf) == PersonOptions.ContactsOf)
                query = query.Include("ContactsOf");

            return query;
        }

        public int CountRegisteredCompanyMembers(int id)
        {
            return this.Set
                .Where(u => u.CompanyID == id
                         && u.NetworkAccessLevel > 0
                         && u.CompanyAccessLevel > 0)
                .Count();
        }

        public int CountPendingCompanyMembers(int id)
        {
            return this.Set
                .Where(u => u.CompanyID == id
                         && u.NetworkAccessLevel > 0
                         && u.CompanyAccessLevel > 0)
                .Count();
        }

        public int CountMustBeValidateUsersByCompany(int networkId)
        {
            return this.Set
                .ByNetwork(networkId)
                .Where(u => u.AccountClosed == true && u.NetworkAccessLevel > 0 && u.CompanyAccessLevel > 0 && u.IsEmailConfirmed)
                .Count();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds)
        {
            return this.Context.UsersViews
                .Where(p => usersIds.Contains(p.Id))
                .LiteSelect()
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .Where(p => usersIds.Contains(p.Id))
                .LiteSelect()
                .ToList();
        }

        public Sparkle.Entities.Networks.Neutral.Person GetLiteById(int userId)
        {
            return this.Context.UsersViews
                .Where(p => userId == p.Id)
                .LiteSelect()
                .SingleOrDefault();
        }

        public Sparkle.Entities.Networks.Neutral.Person GetLiteById(int userId, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .Where(p => userId == p.Id)
                .LiteSelect()
                .SingleOrDefault();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByEmail(string[] emails, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .Where(p => emails.Contains(p.Email))
                .LiteSelect()
                .ToList();
        }

        public Sparkle.Entities.Networks.Neutral.Person GetActiveLiteById(int id)
        {
            return this.Context.UsersViews
                .ActiveAccount()
                .Where(p => id == p.Id)
                .ActiveAccount()
                .LiteSelect()
                .SingleOrDefault();
        }

        public Sparkle.Entities.Networks.Neutral.Person GetActiveLiteById(int id, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .ActiveAccount()
                .Where(p => id == p.Id)
                .LiteSelect()
                .SingleOrDefault();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetActiveLiteById(int[] ids)
        {
            return this.Context.UsersViews
                .ActiveAccount()
                .Where(p => ids.Contains(p.Id))
                .LiteSelect()
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetActiveLiteById(int[] ids, int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .ActiveAccount()
                .Where(p => ids.Contains(p.Id))
                .LiteSelect()
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetAllLite(int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .LiteSelect()
                .ToList();
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetAllActiveLite(int networkId)
        {
            return this.Context.UsersViews
                .ByNetwork(networkId)
                .ActiveAccount()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .LiteSelect()
                .ToList();
        }

        public IEnumerable<GetExportableListOfUsers_Result> GetExportableListOfUsers(int networkId)
        {
            return this.Context.GetExportableListOfUsers(networkId);
        }

        public bool IsEmailAddressInUse(string value)
        {
            var count = this.Set
                .Where(u => u.Email == value || u.PersonalEmail == value)
                .Count();
            return count > 0;
        }

        public bool IsEmailAddressInUse(string accountPart, string domainPart)
        {
            return this.Select()
                .Where(o => (o.Email.StartsWith(accountPart + "+") && o.Email.EndsWith("@" + domainPart)) ||
                                o.Email == accountPart + "@" + domainPart)
                .Count() > 0;
        }

        public IList<Sparkle.Entities.Networks.Neutral.Person> GetPendingEmailAddressConfirmation(int networkId)
        {
            return this.Context.UsersViews
                .Where(u => !u.IsEmailConfirmed)
                .OrderBy(u => u.FirstName)
                .LiteSelect()
                .ToList();
        }

        public IDictionary<int, User> GetActiveById(int[] ids, int networkId, PersonOptions options)
        {
            return this.NewQuery(options)
                .ByNetwork(networkId)
                .ActiveAccount()
                .Where(p => ids.Contains(p.Id))
                .ToDictionary(u => u.Id, u => u);
        }

        public int[] GetMembershipLockedOutUserIds(int networkId)
        {
            return this.Context.GetMembershipLockedOutUserIds(networkId).Where(x => x != null).Select(x => x.Value).ToArray();
        }

        public User GetByEmail(string email, int networkId, PersonOptions options)
        {
            return this.NewQuery(options)
                .ByNetwork(networkId)
                .WithProMail(email)
                .SingleOrDefault();
        }

        public IList<UsersView> GetByEmailOtherNetwork(string email, int networkId)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("The value cannot be empty", "email");

            return this.Context.UsersViews
                .Where(u => u.NetworkId != networkId && email.Equals(u.Email))
                .ToList();
        }

        public User GetByLogin(string login, int networkId, PersonOptions options)
        {
            return this.NewQuery(options)
                .ByNetwork(networkId)
                .WithLogin(login)
                .SingleOrDefault();
        }

        public User GetActiveById(int id, PersonOptions options)
        {
            return this.NewQuery(options)
                .ActiveAccount()
                .Where(u => u.Id == id)
                .SingleOrDefault();
        }

        public bool UsernameExists(string username)
        {
            return this.Set
                .Where(u => u.Login == username)
                .Any();
        }

        public bool MembershipUsernameExists(string username)
        {
            return this.Context.AspnetUser
                .Where(u => u.UserName == username)
                .Any();
        }

        public IList<User> GetAllUsersForRolesStats(int networkId)
        {
            return this.Context.Users
                .ByNetwork(networkId)
                .WithoutDisabledRole()
                .WithoutUserRole()
                .WithoutSparkleStaffRole()
                .ActiveAccount()
                .ToList();
        }

        public IDictionary<int, UsersView> GetUsersView(int networkId)
        {
            return this.Context.UsersViews
                .Where(u => u.NetworkId == networkId)
                .ToDictionary(u => u.Id, u => u);
        }

        public IDictionary<int, UsersView> GetUsersViewById(int[] userIds, int networkId)
        {
            return this.Context.UsersViews
                .Where(u => u.NetworkId == networkId && userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u);
        }

        public IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins, int networkId)
        {
            return this.Context.UsersViews
                .Where(u => u.NetworkId == networkId && userLogins.Contains(u.Login))
                .ToDictionary(u => u.Login, u => u);
        }

        public IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins)
        {
            return this.Context.UsersViews
                .Where(u => userLogins.Contains(u.Login))
                .ToDictionary(u => u.Login, u => u);
        }

        public IDictionary<int, UsersView> GetUsersViewById(int[] userIds)
        {
            return this.Context.UsersViews
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u);
        }

        public int CountCompleteUserProfiles(int networkId)
        {
            var item = this.Context.CountCompleteUserProfiles(networkId).SingleOrDefault();
            if (item.HasValue)
                return item.Value;
            return 0;
        }

        public IList<User> GetActiveByCompanyAndAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options)
        {
            int accessLevelValue = (int)accessLevel;
            return this.NewQuery(options)
                .ActiveAccount()
                .WithCompanyId(companyId)
                .Where(u => u.CompanyAccessLevel == accessLevelValue)
                .OrderBy(u => u.LastName)
                .OrderBy(u => u.FirstName)
                .ToList();
        }

        public IList<User> GetAllByCompanyAndAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options)
        {
            int accessLevelValue = (int)accessLevel;
            return this.NewQuery(options)
                .WithCompanyId(companyId)
                .Where(u => u.CompanyAccessLevel == accessLevelValue)
                .OrderBy(u => u.LastName)
                .OrderBy(u => u.FirstName)
                .ToList();
        }

        public IList<User> GetByIds(int[] createdUserIds)
        {
            return this.Set
                .Where(o => createdUserIds.Contains(o.Id))
                .ActiveAccount()
                .ToList();
        }

        public IList<User> GetByIds(int[] ids, int networkId, PersonOptions options)
        {
            return this.NewQuery(options)
                .Where(o => ids.Contains(o.Id) && o.NetworkId == networkId)
                .ActiveAccount()
                .ToList();
        }

        public User GetById(int id, PersonOptions options)
        {
            return this.NewQuery(options).SingleOrDefault(u => u.Id == id);
        }

        public User GetById(int id, int networkId, PersonOptions options)
        {
            return this.NewQuery(options).SingleOrDefault(u => u.Id == id && u.NetworkId == networkId);
        }

        public User GetActiveByLinkedInId(string linkedInId)
        {
            return this.Set
                .ActiveAccount()
                .Where(o => o.LinkedInUserId == linkedInId)
                .SingleOrDefault();
        }

        public User GetActiveByEmail(string email)
        {
            return this.Set
                .ActiveAccount()
                .Where(o => o.Email == email)
                .SingleOrDefault();
        }

        public bool IsLinkedInIdAvailable(string linkedInId)
        {
            return this.Set
                .Where(o => o.LinkedInUserId == linkedInId)
                .SingleOrDefault() == null;
        }

        public IList<User> GetAllWithoutIds(int networkId, int[] ids, PersonOptions options)
        {
            return this.NewQuery(options)
                .Where(o => o.NetworkId == networkId && !ids.Contains(o.Id))
                .ToList();
        }

        public IList<User> GetAll(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .ToList();
        }

        public IList<UsersView> GetUsersViewByCompanyId(int companyId)
        {
            return this.Context.UsersViews
                .Where(o => o.CompanyId == companyId)
                .ToList();
        }

        public int CountActiveUsers()
        {
            return this.Context.UsersViews
                .Where(u => u.AccountClosed != true && u.Company_IsEnabled && u.IsEmailConfirmed && u.NetworkAccessLevel > 0 && u.CompanyAccessLevel > 0)
                .Count();
        }

        public int CountInactiveUsers()
        {
            return this.Context.UsersViews
                .Where(u => u.AccountClosed == true || !u.Company_IsEnabled || !u.IsEmailConfirmed || u.NetworkAccessLevel <= 0 || u.CompanyAccessLevel <= 0)
                .Count();
        }

        public User WhoUsesThisEmail(int networkId, string email)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId && o.Email == email)
                .SingleOrDefault();
        }

        public IList<UsersView> GetAllUsersViewActive(int networkId)
        {
            return this.Context.UsersViews
                .Where(o => o.NetworkId == networkId)
                .ActiveAccount()
                .ToList();
        }

        public IList<UsersView> GetUsersViewActiveByIds(int[] userIds, int networkId)
        {
            return this.Context.UsersViews
                .Where(o => o.NetworkId == networkId && userIds.Contains(o.Id))
                .ActiveAccount()
                .ToList();
        }

        public IList<UsersView> GetUsersViewActiveWithoutIds(int[] userIds, int networkId)
        {
            return this.Context.UsersViews
                .Where(o => o.NetworkId == networkId && !userIds.Contains(o.Id))
                .ActiveAccount()
                .ToList();
        }

        public int[] GetLatestIds(int networkId, int count, DateTime minDateUtc)
        {
            return this.Set
                .Where(u => u.CreatedDateUtc >= minDateUtc && u.NetworkId == networkId)
                .OrderByDescending(u => u.CreatedDateUtc)
                .Take(count)
                .Select(u => u.Id)
                .ToArray();
        }

        public int[] GetLatestIds(int count, DateTime minDateUtc)
        {
            return this.Set
                .Where(u => u.CreatedDateUtc >= minDateUtc)
                .OrderByDescending(u => u.CreatedDateUtc)
                .Take(count)
                .Select(u => u.Id)
                .ToArray();
        }

        public UsersView GetAnyByCompanyId(int companyId)
        {
            return this.Context.UsersViews
                .Where(o => o.CompanyId == companyId)
                .FirstOrDefault();
        }

        public IList<User> GetByPersonalEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("The value cannot be empty", "email");

            return this.Set
                .Where(u => email.Equals(u.PersonalEmail))
                .ToList();
        }

        public User GetFirst(int networkId, PersonOptions options)
        {
            var query = this.NewQuery(options)
                .OrderBy(x => x.Id);
            var item = query.FirstOrDefault();
            return item;
        }

        public IDictionary<int, int> GetJobCounts(int networkId)
        {
            var query = this.Set
                .ByNetwork(networkId)
                .Where(x => x.JobId != null)
                .GroupBy(x => x.JobId.Value)
                .ToDictionary(g => g.Key, g => g.Count());
            return query;
        }

        public int CountByJobId(int jobId)
        {
            var items = this.Set
                .Where(x => x.JobId == jobId)
                .Count();
            return items;
        }
    }
}
