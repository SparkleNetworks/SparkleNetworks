
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class AchievementsCompany
    {
        public override string ToString()
        {
            return "Company " + this.CompanyId + " has achievement " + this.AchievementId;
        }
    }

    [Flags]
    public enum AchievementsCompanyOptions
    {
        None = 0x0000,
        Achievement = 0x0002,
        Company = 0x0004,
    }
}
