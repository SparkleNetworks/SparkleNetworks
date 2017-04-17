
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class IndustryModel
    {
        public IndustryModel(ProfileFieldsAvailiableValue item)
        {
            this.SelecterId = item.Id;
            this.Value = item.Value;
        }

        public IndustryModel(int id, string value)
        {
            this.SelecterId = id;
            this.Value = value;
        }

        public int SelecterId { get; set; }

        public string Value { get; set; }
    }

    public class CountryModel
    {
        public CountryModel(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public CountryModel(int id, RegionInfo region)
        {
            this.Id = id;
            this.Region = region;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public RegionInfo Region { get; set; }
    }
}
