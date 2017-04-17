
namespace Sparkle.ApiClients.Companies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompanyCategoryApiModel
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public int? NetworkId { get; set; }

        public string KnownCategory { get; set; }

        public bool IsDefault { get; set; }

        public int UsedByActivCount { get; set; }

        public int UsedByInactivCount { get; set; }

        public override string ToString()
        {
            return "CompanyCategory " + this.Id + " " + this.Alias
                + " on N:" + (this.NetworkId != null ? this.NetworkId.Value.ToString() : "all")
                + (this.IsDefault ? " IsDefault" : "");
        }
    }
}
