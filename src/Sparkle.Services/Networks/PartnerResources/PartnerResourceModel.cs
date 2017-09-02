
namespace Sparkle.Services.Networks.PartnerResources
{
    using Sparkle.Services.Networks.Tags;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PartnerResourceModel
    {
        public PartnerResourceModel()
        {
        }

        public PartnerResourceModel(Entities.Networks.PartnerResource item)
        {
            this.Set(item);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public bool Available { get; set; }

        public int CreatedByUserId { get; set; }

        public DateTime DateCreatedUtc { get; set; }

        public DateTime? DateDeletedUtc { get; set; }

        public int NetworkId { get; set; }

        public IList<Tag2Model> Tags { get; set; }

        public IList<Models.ProfileFieldValueModel> Fields { get; set; }

        public bool IsActive
        {
            get { return this.DateDeletedUtc == null && this.Available; }
        }

        private void Set(Entities.Networks.PartnerResource item)
        {
            this.Alias = item.Alias;
            this.Available = item.Available;
            this.CreatedByUserId = item.CreatedByUserId;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.DateDeletedUtc = item.DateDeletedUtc.AsUtc();
            ////this.DeletedByUserId = item.DeletedByUserId;
            this.Id = item.Id;
            this.Name = item.Name;
            this.NetworkId = item.NetworkId;

            if (item.Tags.IsLoaded)
            {
                this.Tags = new List<Tag2Model>(item.Tags.Count);
                foreach (var tag in item.Tags)
                {
                    if (tag.TagDefinition.CategoryReference.IsLoaded)
                        this.Tags.Add(new Tag2Model(tag.TagDefinition, tag.TagDefinition.Category));
                    else
                        this.Tags.Add(new Tag2Model(tag.TagDefinition));
                }
            }
        }
    }
}
