
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IUserProfileFieldsRepository : IBaseNetworkRepository<UserProfileField, int>
    {
        /// <summary>
        /// This methods crashes when multiple values may be returned.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        UserProfileField GetByUserIdAndFieldType(int userId, ProfileFieldType type);

        IList<UserProfileField> GetManyByUserIdAndFieldType(int userId, ProfileFieldType type);
        IList<UserProfileField> GetManyByUserIdAndFieldType(int userId, params ProfileFieldType[] types);

        int Count();

        IList<UserProfileField> GetByUserId(int userId);
        IDictionary<int, IList<UserProfileField>> GetByUserIds(int[] userId);

        IDictionary<int, IList<UserProfileField>> GetByUserIdAndFieldType(int[] userIds, ProfileFieldType[] fields);
        IDictionary<int, IList<UserProfileField>> GetByUserIdsAndFieldType(int[] userIds, ProfileFieldType type);

        IList<UserProfileField> GetAll();

        IList<UserProfileField> GetByFieldType(ProfileFieldType type);

        IList<UserProfileField> GetByFieldTypeAndNetworkId(ProfileFieldType type, int networkId);

        IDictionary<int, int> GetUsersCount(int networkId);

        /// <summary>
        /// Returns a list of profile field types with numbers: ProfileFieldId, COUNT(UserId), COUNT(id).
        /// </summary>
        int[][] GetCounts();
    }
}
