
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Entities.Networks.Neutral;

    public interface IProfileFieldsService
    {
        void Initialize();

        // ProfileFields iservices
        int CountProfileFields();
        int Insert(ProfileField item);
        ProfileField GetProfileFieldById(int id);

        // ProfileFieldsAvailiableValues
        int CountProfileFieldsAvailiablesValues();
        int Insert(ProfileFieldsAvailiableValue item);
        ProfileFieldsAvailiableValue GetProfileFieldsAvailableValueById(int id);
        ProfileFieldsAvailiableValue GetProfileFieldsAvailiableValueByValue(ProfileFieldType type, string value);
        IList<ProfileFieldsAvailiableValue> GetAllAvailiableValuesByType(ProfileFieldType type);
        ProfileFieldsAvailiableValue GetProfileFieldsAvailiableValueByIdAndType(int id, ProfileFieldType type);

        // UserProfileFields iservices
        UserProfileFieldModel SetUserProfileField(int userId, ProfileFieldType profileFieldType, string value);
        UserProfileFieldModel SetUserProfileField(int userId, ProfileFieldType profileFieldType, string value, ProfileFieldSource source);
        IList<UserProfileFieldModel> GetUserProfileFieldsByUserId(int userId);
        IDictionary<int, IList<UserProfileFieldModel>> GetUserProfileFieldsByUserIds(int[] userIds);

        IList<ProfileFieldModel> GetUserFields();
        IDictionary<string, ProfileFieldModel> GetUserFieldsDictionary();

        IList<ProfileFieldModel> GetCompanyFields(bool includeAvailableValues);
        IDictionary<string, ProfileFieldModel> GetCompanyFieldsDictionary(bool includeAvailableValues);

        int GetAvailableValueIdByName(string name);

        CompanyProfileFieldModel SetCompanyProfileField(int companyId, ProfileFieldType profileFieldType, string value, ProfileFieldSource source);
        IList<CompanyProfileFieldModel> GetCompanyProfileFieldsByCompanyId(int companyId);
        IList<ProfileFieldValueModel> GetCompanyValues(int companyId);

        void InsertManyUserProfileFields(IList<UserProfileFieldPoco> items, int? userId);
        void InsertManyCompanyProfileFields(IList<CompanyProfileFieldPoco> items, int? companyId);

        IDictionary<int, IList<CompanyProfileFieldModel>> GetAllCompanyProfileFieldsByCompany();
        IDictionary<int, IList<CompanyProfileFieldModel>> GetCompanyProfileFieldsByCompany(int? networkId = null);

        ////CompanyProfileFieldModel GetCompanyProfileFieldByCompanyIdAndType(int companyId, ProfileFieldType type); // WRONG

        IDictionary<int, IList<CompanyProfileFieldModel>> GetCompanyProfileFieldByCompanyIdAndType(int[] companyIds, ProfileFieldType[] profileFieldTypes);
        IList<CompanyProfileFieldModel> GetCompanyProfileFieldByCompanyIdAndType(int companyId, ProfileFieldType[] profileFieldTypes);
        IList<CompanyProfileFieldModel> GetCompanyProfileFieldByCompanyIdAndType(int companyId, ProfileFieldType profileFieldType);

        IList<UserProfileFieldModel> GetUserProfileFieldsByType(ProfileFieldType type);

        IDictionary<int, UserProfileFieldModel> GetUniqueUserProfileFieldsByUserIdsAndType(int[] userIds, ProfileFieldType type);

        IList<UserProfileFieldModel> GetUserProfileFieldsByTypeAndNetworkId(ProfileFieldType type, int networkId);

        /// <summary>
        /// Returns all the ProfileFields.
        /// Values come from the cache.
        /// </summary>
        IList<ProfileFieldModel> GetTypes();

        /// <summary>
        /// Returns all the ProfileFields with a usage count.
        /// Some values come from the cache.
        /// </summary>
        IList<ProfileFieldModel> GetTypes(bool withCounts);

        /// <summary>
        /// Returns all the ProfileFields.
        /// Values DO NOT come from the cache.
        /// </summary>
        IDictionary<int, ProfileFieldModel> GetAllForCache();

        IList<ProfileFieldAvailableValueModel> GetAvailiableValuesByType(int[] fieldIds);
    }
}
