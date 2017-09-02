
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HintModel
    {
        public HintModel()
        {
        }

        public HintModel(Hint item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Set(item);
        }

        public int Id { get; set; }

        public string Alias { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public HintType HintType { get; set; }

        public bool IsEnabled { get; set; }

        public string Location { get; set; }

        public int? NetworkId { get; set; }

        public override string ToString()
        {
            return "HintModel '" + (this.Alias ?? this.Id.ToString()) + "' (" + this.HintType + ")";
        }

        private void Set(Hint item)
        {
            this.Id = item.Id;
            this.IsEnabled = item.IsEnabled;
            this.Location = item.Location;
            this.NetworkId = item.NetworkId;
            this.Title = item.Title;
            this.Alias = item.Alias;
            this.Description = item.Description;
            this.HintType = item.HintType;
        }
    }
}
