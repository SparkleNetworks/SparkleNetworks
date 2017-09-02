
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class ProfileField : IEntityInt32Id
    {
        public override string ToString()
        {
            return string.Format(
                "ProfileField {0} {1}",
                this.Id,
                this.Name);
        }
    }

    public enum ProfileFieldType
    {
        Unknown = 0,
        Site = 1,
        Phone = 2,
        About = 3,
        City = 4,
        ZipCode = 5,
        FavoriteQuotes = 6,
        CurrentTarget = 7,
        Contribution = 8,
        Country = 9,
        Headline = 10,
        ContactGuideline = 11,
        Industry = 12,
        LinkedInPublicUrl = 13,
        Language = 14,
        Education = 15,
        Twitter = 16,
        GTalk = 17,
        Msn = 18,
        Skype = 19,
        Yahoo = 20,
        Volunteer = 21,
        Certification = 22,
        Patent = 23,
        Location = 24,
        Contact = 25,
        Recommendation = 26,
        Email = 27,
        Facebook = 28,
        AngelList = 29,
        NetworkTeamRole = 30,
        NetworkTeamDescription = 31,
        NetworkTeamGroup = 32,
        NetworkTeamOrder = 33,
        Position = 69,
    }
}
