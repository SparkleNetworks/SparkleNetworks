
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PlaceCategoryModel
    {
        public PlaceCategoryModel()
        {
        }

        public PlaceCategoryModel(int id)
            : this()
        {
            this.Id = id;
        }

        public PlaceCategoryModel(PlaceCategory item)
            : this(item.Id)
        {
            this.Name = item.Name;
            this.Color = item.Color;
            this.Symbol = item.Symbol;
            this.ParentId = item.ParentId;
            this.FoursquareId = item.FoursquareId;
            this.LastUpdateDateUtc = item.LastUpdateDateUtc;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public IList<PlaceCategoryModel> Children { get; set; }

        public string Color { get; set; }

        public string Symbol { get; set; }

        public string FoursquareId { get; set; }

        public DateTime? LastUpdateDateUtc { get; set; }

        public int Depth { get; set; }
    }
}
