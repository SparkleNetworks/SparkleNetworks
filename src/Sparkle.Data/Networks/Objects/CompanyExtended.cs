
namespace Sparkle.Data.Networks.Objects
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public class CompanyExtended
    {
        public Company Company { get; set; }
        public int PeopleCount { get; set; }
        public int InvitedCount { get; set; }
        public string NetworkName { get; set; }
        public IList<Skill> Skills { get; set; }

        public IList<User> Users { get; set; }
    }
}
