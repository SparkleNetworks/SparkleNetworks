
namespace Sparkle.Services.Networks.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class DnsEntryModel
    {
        public DnsEntryModel(string name, string recordType)
            : this(name, recordType, recordType)
        {
        }

        public DnsEntryModel(string name, string type, string recordType)
        {
            this.Name = name;
            this.Type = type;
            this.RecordType = recordType;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public string RecordType { get; set; }

        public string ExpectedValue { get; set; }

        public string ActualValue { get; set; }

        public string[] ActualValues { get; set; }

        public bool? IsActualValueValid { get; set; }

        public IList<string> Errors { get; set; }

        public string DnsServers { get; set; }

        public string ExpectedValueHint { get; set; }
    }
}
