
namespace Sparkle.Services.Networks.Ads
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AdCategoryModel
    {
        public AdCategoryModel()
        {
        }

        public AdCategoryModel(AdCategory item)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.NetworkId = item.NetworkId;
        }


        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public int? NetworkId { get; set; }
    }
}
