
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TagStat
    {
        public TagStat()
        {
        }

        public TagStat(GetTopSkills_Result item)
        {
            this.TagId = item.Id;
            this.TagName = item.TagName;
            this.UsersCount = item.UsersCount ?? 0;
            this.CompaniesCount = item.CompaniesCount ?? 0;
            this.TotalCount = item.TotalCount ?? 0;
        }

        public int TagId { get; set; }
        public string TagName { get; set; }
        public int UsersCount { get; set; }
        public int CompaniesCount { get; set; }
        public int TotalCount { get; set; }
    }
}
