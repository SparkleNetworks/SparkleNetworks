
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IPeopleRepository : IBaseNetworkRepository<User, int>
    {
        IQueryable<User> NewQuery(PersonOptions options);

        IQueryable<User> SelectContacts(string request, int userId, int take);
        IQueryable<User> SelectUnvitedByGroupId(string request, int? groupId, int userId, int take);
        IQueryable<User> SelectUnvitedByEventId(string request, int? eventId, int userId, int take);

        IList<UserExtended> GetWithDetails();

        int CountRegisteredCompanyMembers(int id);
        int CountPendingCompanyMembers(int id);

        int CountMustBeValidateUsersByCompany(int networkId);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds, int networkId);
        Sparkle.Entities.Networks.Neutral.Person GetLiteById(int usersIds);
        Sparkle.Entities.Networks.Neutral.Person GetLiteById(int usersIds, int networkId);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByEmail(string[] emails, int networkId);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetActiveLiteById(int[] ids);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetActiveLiteById(int[] ids, int networkId);
        Sparkle.Entities.Networks.Neutral.Person GetActiveLiteById(int id);
        Sparkle.Entities.Networks.Neutral.Person GetActiveLiteById(int id, int networkId);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetAllLite(int networkId);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetAllActiveLite(int networkId);

        IDictionary<int, User> GetActiveById(int[] ids, int networkId, PersonOptions options);

        IEnumerable<GetExportableListOfUsers_Result> GetExportableListOfUsers(int networkId);

        bool IsEmailAddressInUse(string value);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetPendingEmailAddressConfirmation(int networkId);
        bool IsEmailAddressInUse(string accountPart, string domainPart);

        int[] GetMembershipLockedOutUserIds(int networkId);

        User GetByEmail(string email, int networkId, PersonOptions options);
        IList<UsersView> GetByEmailOtherNetwork(string email, int networkId);
        User GetByLogin(string login, int networkId, PersonOptions options);

        User GetActiveById(int id, PersonOptions options);

        bool UsernameExists(string username);
        bool MembershipUsernameExists(string username);

        IList<User> GetAllUsersForRolesStats(int networkId);

        IDictionary<int, UsersView> GetUsersView(int networkId);
        IDictionary<int, UsersView> GetUsersViewById(int[] userIds, int networkId);
        IDictionary<int, UsersView> GetUsersViewById(int[] userIds);
        IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins, int networkId);
        IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins);

        int CountCompleteUserProfiles(int networkId);

        IList<User> GetActiveByCompanyAndAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options);
        IList<User> GetAllByCompanyAndAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options);

        User GetById(int id, PersonOptions options);
        User GetById(int id, int networkId, PersonOptions options);
        IList<User> GetByIds(int[] createdUserIds);
        IList<User> GetByIds(int[] ids, int networkId, PersonOptions options);

        User GetActiveByLinkedInId(string linkedInId);
        User GetActiveByEmail(string email);

        bool IsLinkedInIdAvailable(string linkedInId);

        IList<User> GetAllWithoutIds(int networkId, int[] ids, PersonOptions options);

        IList<User> GetAll(int networkId);

        IList<UsersView> GetUsersViewByCompanyId(int companyId);

        int CountActiveUsers();

        int CountInactiveUsers();

        User WhoUsesThisEmail(int networkId, string email);

        IList<UsersView> GetAllUsersViewActive(int networkId);
        IList<UsersView> GetUsersViewActiveByIds(int[] userIds, int networkId);
        IList<UsersView> GetUsersViewActiveWithoutIds(int[] userIds, int networkId);

        int[] GetLatestIds(int count, DateTime minDateUtc);
        int[] GetLatestIds(int networkId, int count, DateTime minDateUtc);

        UsersView GetAnyByCompanyId(int companyId);

        IList<User> GetByPersonalEmail(string email);

        User GetFirst(int networkId, PersonOptions options);

        IDictionary<int, int> GetJobCounts(int networkId);

        int CountByJobId(int jobId);
    }
}
