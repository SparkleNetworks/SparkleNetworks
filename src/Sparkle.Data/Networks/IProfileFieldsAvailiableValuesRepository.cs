
namespace Sparkle.Data.Networks
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Repository]
    public interface IProfileFieldsAvailiableValuesRepository : IBaseNetworkRepository<ProfileFieldsAvailiableValue, int>
    {
        int Count();
        ProfileFieldsAvailiableValue GetByValue(ProfileFieldType type, string value);

        IList<ProfileFieldsAvailiableValue> GetByType(ProfileFieldType type);

        ProfileFieldsAvailiableValue GetByIdAndType(int id, ProfileFieldType type);

        ProfileFieldsAvailiableValue GetByName(string name);

        IList<ProfileFieldsAvailiableValue> GetAll();

        IDictionary<int, int> CountByType();

        IList<ProfileFieldsAvailiableValue> GetByFieldId(int fieldId);
        IList<ProfileFieldsAvailiableValue> GetByFieldId(int[] fieldIds);
    }
}
