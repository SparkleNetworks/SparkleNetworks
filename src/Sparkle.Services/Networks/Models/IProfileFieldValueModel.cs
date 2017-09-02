
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IProfileFieldValueModel
    {
        int Id { get; set; }
        int EntityId { get; set; }
        ProfileFieldType Type { get; set; }
        int TypeId { get; set; }
        string Value { get; set; }
        ProfileFieldSource Source { get; set; }
        string Data { get; set; }
    }

    public static class ProfileFieldValueModelExtensions
    {
        public static string GetValue(this IEnumerable<IProfileFieldValueModel> collection, ProfileFieldType profileFieldId)
        {
            return collection.GetValue((int)profileFieldId);
        }

        public static string GetValue(this IEnumerable<IProfileFieldValueModel> collection, int profileFieldId)
        {
            var item = collection.SingleOrDefault(x => x.TypeId == profileFieldId);
            if (item != null)
            {
                return item.Value;
            }
            else
            {
                return null;
            }
        }

        public static ProfileFieldSource GetSource(this IEnumerable<IProfileFieldValueModel> collection, ProfileFieldType profileFieldId)
        {
            return collection.GetSource((int)profileFieldId);
        }

        public static ProfileFieldSource GetSource(this IEnumerable<IProfileFieldValueModel> collection, int profileFieldId)
        {
            var item = collection.SingleOrDefault(x => x.TypeId == profileFieldId);
            if (item != null)
            {
                return item.Source;
            }
            else
            {
                return ProfileFieldSource.None;
            }
        }
    }
}
