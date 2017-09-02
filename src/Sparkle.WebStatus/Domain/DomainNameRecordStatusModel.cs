
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DomainNameRecordStatusModel
    {
        private readonly DomainNameRecord domainNameRecord;

        public DomainNameRecordStatusModel(DomainNameRecord item)
        {
            this.domainNameRecord = item;
        }

        public DomainNameRecord Item
        {
            get { return this.Item; }
        }
    }
}
