
namespace Sparkle.Services.Networks
{
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Data.Options;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Networks.Tags;
    using Sparkle.Services.Networks.Models.Profile;
    
    public interface ICompanyService
    {
        int Count();
        int CountNonEmpty();
        int CountEmpty();
        int CountCompleteProfiles();
        int CountActive();

        Company Insert(Company item);

        /// <summary>
        /// Creates a new company and send an email to users with NetworkAccessLevel.SparkleStaff or NetworkAccessLevel.AddCompany.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        CompanyRequest CreateForApproval(CompanyRequest item);

        long Update(Company item, bool allNetworks = false);
        
        IList<Company> Search(string request);

        void DeleteUnapprovedCompany(Company item, bool allNetworks = false);

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        IList<Company> SelectAll();

        IList<Company> SelectBetaAllow();
        
        Company SelectByDomainName(string domainName);

        /// <summary>
        /// Gets the company by alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        Company GetByAlias(string alias);

        /// <summary>
        /// Gets the company by alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="allNetworks">if set to <c>true</c> [all networks].</param>
        /// <returns></returns>
        Company GetByAlias(string alias, bool allNetworks);

        /// <summary>
        /// Gets the company by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Company GetById(int id);

        /// <summary>
        /// Gets the company by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="allNetworks">if set to <c>true</c> [all networks].</param>
        /// <returns></returns>
        Company GetById(int id, bool allNetworks);

        /// <summary>
        /// Gets the company by panorama id.
        /// </summary>
        /// <param name="panoramaId">The panorama id.</param>
        /// <returns></returns>
        IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetActiveLight();
        IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetAllLight();
        IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> SearchActiveLight(string nameLike);

        IList<Company> GetLastApproved(int max);

        IList<Company> GetBySkill(int skillId, bool allNetworks = false);

        IList<Company> SelectByNetworkId(int networkId);

        IList<CompanyExtended> GetWithStatsAndSkills();
        IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetWithStatsAndSkillsAndJobs();

        IList<CompanyExtended> GetWithStats();
        IList<CompanyExtended> GetInactiveWithStats();

        int CountByNetworkId(int networkId);

        IList<CompanyExtended> GetWaitingApprobation();

        IList<CompanyExtended> SelectByApprovedDateUpper(DateTime dateTime);
        IList<CompanyExtended> SelectByApprovedDateRange(DateTime from, DateTime to);

        int CountPublications(int companyId);
        int CountPendingMembers(int id);
        int CountRegisteredMembers(int id);

        string FindEmailDomainFromNameAndEmail(string companyName, string emilAddress);

        string MakeAlias(string companyName);

        CompanyRequest GetRequest(Guid id);
        CompanyRequest GetRequest(int id);
        IList<CompanyRequest> GetPendingRequests();
        IList<CompanyRequest> GetAllRequests();

        Company AcceptRequest(int companyRequestId, int userId);
        CompanyRequest RejectRequest(int companyRequestId, string reason, int userId);

        CompanyRequest BlockRequest(int id, int userId, string reason);

        int CountPendingRequests();

        int CountWaitingApprobation();

        IList<Company> GetAllForImportScripts();

        IList<CompanyCategoryModel> GetAllCategories();

        IList<UserModel> GetAdministrators(int companyId);

        ProfilePictureModel GetProfilePicture(int companyId, PictureAccessMode mode);
        ProfilePictureModel GetProfilePicture(string companyAlias, PictureAccessMode mode);
        ProfilePictureModel GetProfilePicture(Company item, PictureAccessMode mode);
        ProfilePictureModel GetProfilePicture(int id, string alias, PictureAccessMode mode);

        Users.SetProfilePictureResult SetProfilePicture(Users.SetProfilePictureRequest setProfilePictureRequest);

        bool IsActive(Company company);

        IDictionary<int, CompanyModel> GetByIdFromAnyNetwork(IList<int> ids, CompanyOptions options);
        CompanyModel GetByIdFromAnyNetwork(int ids, CompanyOptions options);

        string GetProfileUrl(Company company, UriKind uriKind);
        string GetProfileUrl(string companyAlias, UriKind uriKind);

        string GetProfilePictureUrl(string alias, CompanyPictureSize pictureSize, UriKind uriKind);
        string GetProfilePictureUrl(Company company, CompanyPictureSize pictureSize, UriKind uriKind);

        IList<CompanyExtended> GetEnabledWithStats();
        IList<CompanyExtended> GetDisabledWithStats();

        ToggleCompanyRequest GetToggleCompanyRequest(string alias);
        ToggleCompanyResult ToggleCompany(ToggleCompanyRequest request);

        ApplyCompanyModel VerifyEmailDomainForApply(string email, string search, Guid? key);

        CreateCompanyRequest GetCreateRequest(CreateCompanyRequest request, int? userId);

        void UpdateFieldsFromLinkedInCompany(LinkedInCompanyResult result, LinkedInNET.Companies.Company company, bool saveFields);
        CreateCompanyResult ApplyCreateRequest(CreateCompanyRequest request, bool isApproved);

        IList<UserModel> GetActiveUsersByAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options);
        IList<UserModel> GetAllUsersByAccessLevel(int companyId, CompanyAccessLevel accessLevel, PersonOptions options);
        IList<AdminWorkModel> GetCompanyAccessLevelIssues();

        IList<Sparkle.Services.Networks.Users.ApplyRequestModel> GetPendingApplyRequests();

        IList<CompanyModel> GetAllActiveWithModel();

        IList<ApplyRequestModel> GetApplyRequestsWithCompanyId(int companyId);

        CompanyCategoryModel GetCategoryByName(string name);

        IList<CompanyCategoryModel> GetCategoriesForDashboard();

        void SetDefaultCategory(short categoryId);

        EditCompanyCategoryRequest GetEditCompanyCategoryRequest(short? id);
        EditCompanyCategoryResult UpdateCompanyCategory(EditCompanyCategoryRequest request);

        void Initialize();

        CompanyCategoryModel GetCategoryByAlias(string alias);

        CompanyCategoryModel GetDefaultCategory();
        CompanyCategoryModel GetCategoryById(short id);
        IDictionary<short, CompanyCategoryModel> GetCategoryById(short[] ids);

        IList<CompanyPlaceModel> GetPlacesFromCompanyId(int companyId);

        IList<CompanyPlaceModel> GetCompaniesAtPlace(int placeId);

        IList<CompanyModel> GetCompaniesNearLocation(string[] geocodes);

        void AddCompanyPlaceFromCompanyProfileField(string logPath, Company company, ICompanyProfileFieldValue item);

        AjaxTagPickerModel GetAjaxTagPickerModel(int companyId, int actingUserId);

        CompanyListModel Search(string keywords, string location, int[] tagIds, bool combinedTags, int offset, int count, CompanyOptions options);

        EditProfileFieldsRequest GetEditCompanyFieldsRequest(int? companyId, EditProfileFieldsRequest request);
        EditProfileFieldsResult EditCompanyFields(EditProfileFieldsRequest request);
    }
}
