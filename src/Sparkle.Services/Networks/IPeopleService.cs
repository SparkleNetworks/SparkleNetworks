
namespace Sparkle.Services.Networks
{
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Objects;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Team;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Objects;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public interface IPeopleService
    {
        User Insert(User item);

        [Obsolete("This method makes use of OptionsList.")]
        IList<string> OptionsList { get; set; }

        [Obsolete("This method makes use of OptionsList. Do not use it. Use GetAll*.")]
        IList<User> SelectAll();

        IList<User> SelectFromCompany(long companyId);
        IList<User> SelectFromJob(long jobId);

        IList<User> SearchContacts(string request, int Take, int UserId);

        User GetByGuid(Guid userId);

        [Obsolete("This method makes use of OptionsList. Do not use it. Use GetEntityByIdInNetwork or GetEntityByIdFromAnyNetwork.")]
        User SelectWithId(int userId);

        User SelectWithId(int userId, PersonOptions options);
        User SelectWithShortId(int shortId);
        User SelectWithProMail(string mail);
        User SelectWithLogin(string login);
        User SelectWithLogin(string login, PersonOptions options);

        [Obsolete("Use UpdateUserProfile instead.")]
        long Update(User item);

        IList<User> SelectUnvitedByGroupId(string request, int? GroupId, int Take, int UserId);
        IList<User> SelectUnvitedByEventId(string request, int? EventId, int Take, int UserId);

        IList<User> Search(string request);
        IList<User> SearchFromCompanyId(string request, int take, int CompanyId);
        IList<User> SearchPeopleWithoutTeam(string request, int take, int CompanyId);

        int CountCompanyPeoples(long companyId);
        int CountAll();
        int CountActive();

        IList<NewsletterSubscriber> SelectForNewsletter();
        WeeklyMailSubscriber SelectForWeeklyMailByLogin(string login);
        IList<WeeklyMailSubscriber> SelectForWeeklyMail(bool invited, bool registered);
        IList<WeeklyMailSubscriber> SelectForDailyMail(bool invited, bool registered);
        int CountByCompany(int company);

        CompanyAccessLevel GetAccountRight(int userId);

        IList<User> GetActiveByCompany(int companyId);
        IList<User> GetAllByCompanyId(int companyId);

        IEnumerable<User> QueryActivePeople(PersonOptions options);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteById(int[] usersIds);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByIdFromAnyNetwork(int[] usersIds);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetLiteByEmail(string[] emails);
        IList<UserPoco> GetLiteById(int[] usersIds, Func<User, UserPoco> selector);
        UserPoco GetLiteById(int userId, Func<User, UserPoco> selector);
        IList<UserPoco> GetActiveLiteById(int[] usersIds, Func<User, UserPoco> selector);
        UserModel GetActiveById(int userId, PersonOptions options);
        UserModel GetActiveByLogin(string username, PersonOptions options);

        int CountCompleteProfiles();
        int CountMustBeValidateUsersByCompanyId(int companyId);

        IList<User> GetBySkill(int skillId);
        IList<User> GetByInterest(int interestId);
        IList<User> GetByRecreation(int recreationId);

        IList<User> SelectCompanyContacts(int companyId);

        User GetForCoffeeRoulette(User currentUser);

        int CountForNewsletter();
        int CountForDailyNewsletter();
        int CountForWeeklyNewsletter();
        int CountForNoNewsletter();
        int CountForNoDailyNewsletter();
        int CountForNoWeeklyNewsletter();

        User GetForSessionById(Guid id);
        User GetForSessionByLogin(string login);

        Users.SetProfilePictureResult SetProfilePicture(Users.SetProfilePictureRequest request);
        Models.ProfilePictureModel GetProfilePicture(int userId, PictureAccessMode mode);
        Models.ProfilePictureModel GetProfilePicture(User user, PictureAccessMode mode);
        string GetProfilePictureUrl(string login, UserProfilePictureSize pictureSize, UriKind uriKind);
        string GetProfilePictureUrl(string login, string pictureName, UserProfilePictureSize pictureSize, UriKind uriKind);
        string GetProfilePictureUrl(User user, UserProfilePictureSize pictureSize, UriKind uriKind);
        string GetProfilePictureUrl(UserModel user, UserProfilePictureSize pictureSize, UriKind uriKind);
        string GetProfileUrl(User user, UriKind uriKind);
        string GetProfileUrl(UserModel user, UriKind uriKind);

        CreateEmailPassordAccountResult CreateEmailPasswordAccount(CreateEmailPassordAccountRequest model);
        void NotifyAdministratorsOfPendingUserRegistrations(User user);

        SendActivationEmailResult SendActivationEmail(string email, string message = null);
        ConfirmEmailResult ConfirmEmail(int actionId, string secret);
        IList<UserModel> GetPendingEmailAddressConfirmation();

        IList<User> GetPendingUserRegistrations(int networkId);

        MembershipUser GetMembershipUser(Guid guid);
        void LockMembershipAccount(string username);
        IList<UserModel> GetMembershipLockedOutUsers();

        int CountMustBeValidateUsersByCompany();
        string MakeUsernameFromName(string firstname, string lastname);

        IList<UsersView> GetAllUsersViewActive();
        IList<UserModel> GetAll(
            bool withLastActivity = false,
            ProfileFieldType[] withProfileFields = null);
        IList<Sparkle.Entities.Networks.Neutral.Person> GetAllLite();
        IList<Sparkle.Entities.Networks.Neutral.Person> GetAllActiveLite();

        IList<UserModel> GetExportableList();

        ////IList<User> GetLastRegistrants(short max); // replaced by GetLatest()

        bool IsActive(User user);
        bool IsActive(UserPoco user);
        bool IsEmailAddressInUse(string value);
        bool IsEmailAddressInUse(string accountPart, string domainPart);


        SendPasswordRecoveryEmailResult SendPasswordRecoveryEmail(int userId, string message = null, bool sendEmail = true, string subject = null);
        SendPasswordRecoveryEmailResult SendPasswordRecoveryEmail(string email, string message = null, bool sendEmail = true, string subject = null);
        SendPasswordRecoveryEmailResult SendPasswordRecoveryEmailOnAutoRecover(string id, string message = null, bool sendEmail = true, string subject = null);

        IList<Sparkle.Entities.Networks.Neutral.Person> GetByNetworkAccessLevel(NetworkAccessLevel networkAccessLevel);
        BasicResult<SetNetworkAccessLevelError> SetNetworkAccessLevel(int id, NetworkAccessLevel networkAccessLevel);
        bool CanChangeUserCompanyAccess(User targetUser, User actingUser, CompanyAccessLevel levelToSetOnTarget);

        IList<User> GetSubscribedToMainTimelineItems();
        IList<User> GetSubscribedToMainTimelineComments();

        bool IsLoginAvailable(string username);

        CreateEmailPassordAccountRequest GetCreateEmailPassordAccountModel(CreateEmailPassordAccountRequest model, Guid? invitationCode);

        User GetEntityByIdInNetwork(int userId, PersonOptions options);
        User GetEntityByIdFromAnyNetwork(int userId, PersonOptions options);
        UserModel GetByIdFromAnyNetwork(int userId, PersonOptions options);
        UserRolesModel GetUserRolesModel();

        UserRoleFormModel GetRolesFormModel(User user, int? currentUserId);

        void UpdateUserRolesFromModel(UserRoleFormModel model);

        int[] KeepActiveUserIds(int[] userIds);

        IDictionary<int, UserModel> GetModelByIdFromAnyNetwork(int[] ids, PersonOptions options);

        CultureInfo GetCulture(UserModel user);
        CultureInfo GetCulture(User user);
        CultureInfo GetCulture(Person user);
        CultureInfo GetCulture(string userCulture);
        CultureInfo GetCulture(int userId);
        CultureInfo GetCulture(User user, CultureInfo[] fallbackCultures);
        CultureInfo GetCulture(Person user, CultureInfo[] fallbackCultures);
        CultureInfo GetCulture(string userCulture, CultureInfo[] fallbackCultures);
        TimeZoneInfo GetTimezone(UserModel user);
        TimeZoneInfo GetTimezone(User user);
        TimeZoneInfo GetTimezone(Person user);
        TimeZoneInfo GetTimezone(string userTimezone);
        TimeZoneInfo GetTimezone(int userId);
        ChangeUserCultureResult ChangeUserCulture(ChangeUserCultureRequest request);

        IList<User> GetSubscribedToCompanyNetworkItems(int companyId);
        IList<User> GetSubscribedToCompanyNetworkComments(int companyId);

        LinkedInPeopleResult UpdateFromLinkedIn(LinkedInPeopleRequest request);

        ProfileEditRequest GetProfileEditRequest(int? loadUserId, ProfileEditRequest request);
        ProfileEditResult UpdateUserProfile(ProfileEditRequest model);

        ApplyRequestRequest GetApplyRequestRequest(Guid? Key, ApplyRequestRequest request, int? userId, string companyCategory);
        ApplyRequestResult SaveApplyRequest(ApplyRequestRequest request, bool submit);
        ApplyRequestResult AppendSocialNetworkConnection(
            ApplyRequestRequest request,
            SocialNetworkConnectionType type,
            string username,
            string oAuthToken,
            string oAuthVerifier,
            bool isActive,
            DateTime? oAuthTokenDateUtc = null,
            int? oAuthTokenDurationMinutes = null);
        LinkedInPeopleResult AppendLinkedInProfileToApplyRequest(ApplyRequestRequest request, string userRemoteAddress);
        IList<ApplyRequestModel> GetPendingApplyRequests();
        string GetApplyRequestUrl(ApplyRequestModel applyRequest);
        string GetApplyRequestConfirmUrl(ApplyRequestModel applyRequest);
        string GetApplyRequestJoinUrl(Guid id);
        string GetApplyRequestJoinUrl(ApplyRequestModel applyRequest);
        ApplyRequestModel GetApplyRequest(Guid key);
        ApplyRequestModel GetApplyRequest(int id);
        ConfirmApplyRequestEmailAddressResult ConfirmApplyRequestEmailAddress(ConfirmApplyRequestEmailAddressRequest request);

        IList<ApplyRequestModel> GetAllApplyRequests(int userId);
        PagedListModel<ApplyRequestModel> GetAllApplyRequests(int userId, int offset, int pageSize);
        int CountAllApplyRequests(int userId);

        ValidateApplyRequestResult ValidateApplyRequestToJoin(Guid key, string secret);
        AcceptApplyRequestResult AcceptApplyRequest(AcceptApplyRequestRequest request);
        RefuseApplyRequestResult RefuseApplyRequest(RefuseApplyRequestRequest request);

        RegionSettingsRequest GetRegionSettingsRequest(int userId);
        RegionSettingsResult SaveRegionSettings(RegionSettingsRequest request, int userId);
        void FillRegionSettingsRequestLists(RegionSettingsRequest model);

        ConnectWithLinkedInResult GetUserFromLinkedInId(ConnectWithLinkedInRequest request);

        void TryUpdateLinkedInId(User user, string linkedInId, string email);

        IList<UserModel> GetNotSubscribedUsers(bool includeInactive);

        IList<UsersView> GetUsersViewFromCompany(int companyId);
        IDictionary<int, UsersView> GetUsersViewById(int[] userIds);
        IDictionary<string, UsersView> GetUsersViewByLogin(string[] userLogins);

        void CleanNotSubmittedApplyRequests();

        string GetInviteWithApplyUrl(int inviterUserId, int networkId);
        bool IsApplyInviterCodeValid(string code);

        UserModel WhoUsesThisEmail(string email);

        bool IsLinkedInIdAvailable(string linkedInId);
        void InsertManyTags(IList<Sparkle.Services.Networks.Models.Tags.TagModel> items, int userId);
        void UpdateProfilePictureFromApply(ApplyRequestModel model, User user);

        IList<SearchResultModel<UserModel>> Search(string query, bool searchName = true, bool searchEmail = false, bool searchTags = false, bool includeInactive = false);

        IDictionary<int, UserModel> GetAllForCache();
        IDictionary<int, UserModel> GetForCache(int[] ids);

        IList<UserModel> GetLatest(int count);

        void Refresh(UserModel items, bool refreshPictureUrl = false, bool useFullUrls = false);
        void Refresh(IEnumerable<UserModel> items, bool refreshPictureUrl = false, bool useFullUrls = false);

        NetworkUserGender GetDefaultGender();

        InviteWithApplyResult InviteWithApply(InviteWithApplyRequest request);

        void UpdateApplyRequestData(ApplyRequestModel model);

        UserModel GetById(int id, PersonOptions options);

        UserModel GetByUsername(string login, PersonOptions options);

        ApplyRequestModel GetUsersApplyRequest(int userId);

        AdminProceduresResult ChangeUserCompany(AdminProceduresRequest request, int actingUserId);

        TeamPageModel GetTeamPageModel();
        EditNetworkRoleRequest GetEditNetworkRoleRequest(string login);
        EditNetworkRoleResult UpdateNetworkRole(EditNetworkRoleRequest request);

        void DeleteUserNetworkRole(string login);

        void EditTeamNetworkGroupOrder(int[] order);
        void EditTeamNetworkPeopleOrder(int[] person);

        AjaxTagPickerModel GetAjaxTagPickerModel(User person, User actingUser);

        IDictionary<int, int> GetProfileFieldsCount();

        void UpdatePresence(int userId, DateTime day, DateTime now);

        int GetUserPresenceDaysCount(int userId);
        UserPresenceStats GetUserPresenceStats();

        SetSingleProfileFieldResult SetSingleProfileFieldOnApply(SetSingleProfileFieldRequest request);

        UserModel GetFirst(PersonOptions options);

        DateTime? GetLastUserPresence(int userId);
    }
}
