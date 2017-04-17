
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Linq;
    using System.Text;
    using Sparkle.Data.Entity.Networks.Sql;

    public class UserProfileFieldsRepository : BaseNetworkRepositoryInt<UserProfileField>, IUserProfileFieldsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public UserProfileFieldsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory,  m => m.UserProfileFields)
        {
        }

        public UserProfileField GetByUserIdAndFieldType(int userId, ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .Where(o => o.ProfileFieldId == (int)type)
                .SingleOrDefault();
        }

        public IList<UserProfileField> GetManyByUserIdAndFieldType(int userId, ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .Where(o => o.ProfileFieldId == (int)type)
                .ToList();
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public IList<UserProfileField> GetByUserId(int userId)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public IDictionary<int, IList<UserProfileField>> GetByUserIds(int[] userIds)
        {
            return this.Set
                .Where(o => userIds.Contains(o.Id))
                .GroupBy(o => o.UserId)
                .ToDictionary(o => o.Key, o => (IList<UserProfileField>)o.ToList());
        }

        public IList<UserProfileField> GetManyByUserIdAndFieldType(int userId, params ProfileFieldType[] types)
        {
            var intTypes = types.Select(o => (int)o).ToArray();
            return this.Set
                .Where(o => o.UserId == userId)
                .Where(o => intTypes.Contains(o.ProfileFieldId))
                .ToList();
        }

        public IDictionary<int, IList<UserProfileField>> GetByUserIdAndFieldType(int[] userIds, ProfileFieldType[] fields)
        {
            var typeIds = fields.Cast<int>().ToArray();
            var items = this.Set
                .Where(f => userIds.Contains(f.UserId) && typeIds.Contains(f.ProfileFieldId))
                .ToArray()
                .GroupBy(f => f.UserId)
                .ToDictionary(f => f.Key, f => (IList<UserProfileField>)f.ToList());
            return items;
        }

        public IList<UserProfileField> GetAll()
        {
            return this.Set
                .ToList();
        }

        public IList<UserProfileField> GetByFieldType(ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.ProfileFieldId == (int)type)
                .ToList();
        }

        public IDictionary<int, IList<UserProfileField>> GetByUserIdsAndFieldType(int[] userIds, ProfileFieldType type)
        {
            return this.Set
                .Where(o => userIds.Contains(o.UserId) && o.ProfileFieldId == (int)type)
                .GroupBy(o => o.UserId)
                .ToDictionary(o => o.Key, o => (IList<UserProfileField>)o.ToList());
        }

        public IList<UserProfileField> GetByFieldTypeAndNetworkId(ProfileFieldType type, int networkId)
        {
            return this.Set
                .Where(o => o.ProfileFieldId == (int)type && o.User.NetworkId == networkId)
                .ToList();
        }

        public IDictionary<int, int> GetUsersCount(int networkId)
        {
            var commandText = @"select uf.UserId, COUNT(uf.Id) as [Count] from dbo.UserProfileFields uf
inner join dbo.Users u on u.Id = uf.UserId and u.NetworkId = @NetworkId
group by uf.UserId";
            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText(commandText)
                .AddParameter("@NetworkId", networkId);
            var values = new Dictionary<int, int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(reader.GetInt32(reader.GetOrdinal("UserId")), reader.GetInt32(reader.GetOrdinal("Count")));
                }
            }

            return values;
        }

        public int[][] GetCounts()
        {
            // select ProfileFieldId, COUNT(CompanyId) Subjects, COUNT(id) [Values] 
            // from UserProfileFields
            // group by ProfileFieldId

            var query = this.Set
                .GroupBy(x => x.ProfileFieldId)
                .Select(x => new
                {
                    ProfileFieldId = x.Key,
                    Subjects = x.GroupBy(y => y.UserId).Count(),
                    Values = x.Count(),
                });
            var result = query
                .ToArray()
                .Select(x => new int[] { x.ProfileFieldId, x.Subjects, x.Values, })
                .ToArray();
            return result;
        }
    }
}
