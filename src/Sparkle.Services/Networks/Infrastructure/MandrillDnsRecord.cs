
namespace Sparkle.Services.Networks.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class MandrillDnsRecord
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string[] ValueParts { get; set; }
    }
}
