
namespace Sparkle.Data.Networks.Objects
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public class PersonExtendedList : List<UserExtended> {
        public PersonExtendedList(IEnumerable<UserExtended> collection) : base(collection) {
        }

        public IList<Company> Companies { get; set; }
        public IList<Job> Jobs { get; set; }
    }
}
