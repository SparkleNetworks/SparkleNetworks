
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure.Data.Objects
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UniverseDomainName
    {
        public UniverseDomainName(UniverseDomainName item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.UniverseId = item.UniverseId;
            this.UniverseName = item.UniverseName;
            this.RedirectToMain = item.RedirectToMain;
        }

        public UniverseDomainName()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int UniverseId { get; set; }
        public string UniverseName { get; set; }
        public bool RedirectToMain { get; set; }
    }
}
