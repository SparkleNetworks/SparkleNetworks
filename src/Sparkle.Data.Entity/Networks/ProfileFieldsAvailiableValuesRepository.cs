
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks;
    
    public class ProfileFieldsAvailiableValuesRepository : BaseNetworkRepositoryInt<ProfileFieldsAvailiableValue>, IProfileFieldsAvailiableValuesRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public ProfileFieldsAvailiableValuesRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.ProfileFieldsAvailiableValues)
        {
        }

        public int Count()
        {
            return this.Set.Count();
        }

        public ProfileFieldsAvailiableValue GetByValue(ProfileFieldType type, string value)
        {
            return this.Set
                .Where(o => o.Value == value && o.ProfileFieldId == (int)type)
                .SingleOrDefault();
        }

        public IList<ProfileFieldsAvailiableValue> GetByType(ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.ProfileFieldId == (int)type)
                .ToList();
        }

        public ProfileFieldsAvailiableValue GetByIdAndType(int id, ProfileFieldType type)
        {
            return this.Set
                .Where(o => o.Id == id && o.ProfileFieldId == (int)type)
                .SingleOrDefault();
        }

        public ProfileFieldsAvailiableValue GetByName(string name)
        {
            return this.Set
                .Where(o => o.Value == name)
                .SingleOrDefault();
        }

        public IList<ProfileFieldsAvailiableValue> GetAll()
        {
            return this.Set
                .ToList();
        }

        public IDictionary<int, int> CountByType()
        {
            return this.Set
                .GroupBy(x => x.ProfileFieldId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public IList<ProfileFieldsAvailiableValue> GetByFieldId(int fieldId)
        {
            return this.Set
                .Where(o => o.ProfileFieldId == fieldId)
                .ToList();
        }

        public IList<ProfileFieldsAvailiableValue> GetByFieldId(int[] fieldIds)
        {
            return this.Set
                .Where(o => fieldIds.Contains(o.ProfileFieldId))
                .ToList();
        }
    }
}
