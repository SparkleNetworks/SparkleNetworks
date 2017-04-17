
namespace Sparkle.Services.Networks.Models.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LanguageProfileFieldModel
    {
        public int?     LinkedInId { get; set; }
        public string   Name { get; set; }
        public string   ProficiencyLevel { get; set; }
        public string   ProficiencyLevelFriendly { get; set; }

        public void UpdateFrom(LinkedInNET.Profiles.Language item)
        {
            this.LinkedInId                 = item.Id;
            this.Name                       = item.LanguageInfo != null ? item.LanguageInfo.Name : null;
            this.ProficiencyLevel           = item.Proficiency != null ? item.Proficiency.Level : null;
            this.ProficiencyLevelFriendly   = item.Proficiency != null ? item.Proficiency.Name : null;
        }
    }
}
