
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CompanyCategoryModel
    {
        public CompanyCategoryModel(CompanyCategory item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.NetworkId = item.NetworkId;
            this.KnownCategory = item.KnownCategoryValue;
            this.IsDefault = item.IsDefault;
        }

        public short Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public int? NetworkId { get; set; }

        public KnownCompanyCategory KnownCategory { get; set; }

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
