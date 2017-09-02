
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProfileFieldAvailableValueModel
    {
        public ProfileFieldAvailableValueModel()
        {
        }

        public ProfileFieldAvailableValueModel(Entities.Networks.ProfileFieldsAvailiableValue item)
        {
            this.Id = item.Id;
            this.ProfileFieldId = item.ProfileFieldId;
            this.Value = item.Value;
        }

        public int Id { get; set; }
        public int ProfileFieldId { get; set; }
        public string Value { get; set; }
    }
}
