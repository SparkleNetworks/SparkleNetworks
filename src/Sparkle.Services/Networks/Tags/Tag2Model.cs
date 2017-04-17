
namespace Sparkle.Services.Networks.Tags
{
    using Newtonsoft.Json.Linq;
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;

    public class Tag2Model
    {
        public Tag2Model()
        {
        }

        public Tag2Model(Sparkle.Entities.Networks.TagDefinition item)
        {
            this.Set(item);
        }

        public Tag2Model(Sparkle.Entities.Networks.TagDefinition item, Sparkle.Entities.Networks.TagCategory category)
        {
            this.Set(item);
            ////this.Set(category);
        }

        public Tag2Model(Sparkle.Entities.Networks.TagDefinition item, TagCategoryModel category)
        {
            this.Set(item);
            ////this.Set(category);
        }

        public Tag2Model(TagDefinitionPoco item)
        {
            this.Set(item);
        }

        public Tag2Model(Sparkle.Entities.Networks.Skill item)
        {
            this.Set(item);
        }

        public Tag2Model(Sparkle.Entities.Networks.Interest item)
        {
            this.Set(item);
        }

        public Tag2Model(Sparkle.Entities.Networks.Recreation item)
        {
            this.Set(item);
        }

        /// <summary>
        /// The tag's unique ID.
        /// </summary>
        public int Id { get; set; }

        public int NetworkId { get; set; }

        /// <summary>
        /// The display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  The URL name.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// The tag category ID.
        /// </summary>
        public int CategoryId { get; set; }

        public Entities.Networks.TagType CategoryValue { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public string Description { get; set; }

        public int Payload { get; set; }

        public Tag2Model Clone()
        {
            return new Tag2Model
            {
                Alias = this.Alias,
                CategoryId = this.CategoryId,
                CategoryValue = this.CategoryValue,
                CreatedDateUtc = this.CreatedDateUtc,
                Description = this.Description,
                Id = this.Id,
                Name = this.Name,
                NetworkId = this.NetworkId,
                Payload = this.Payload,
            };
        }

        public JToken Data { get; set; }

        public override string ToString()
        {
            return "Tag2Model " + this.Id + " " + (this.CategoryValue) + " " + this.Alias;
        }

        private void Set(Entities.Networks.TagDefinition item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.CategoryId = item.CategoryId;
            this.CategoryValue = item.CategoryValue;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.Description = item.Description;
            this.CreatedDateUtc = item.CreatedDateUtc.AsUtc();
            this.Data = !string.IsNullOrEmpty(item.Data) ? JObject.Parse(item.Data) : null;
        }

        private void Set(Entities.Networks.Neutral.TagDefinitionPoco item)
        {
            this.Id = item.Id;
            this.NetworkId = item.NetworkId;
            this.CategoryId = item.CategoryId;
            this.CategoryValue = item.CategoryValue;
            this.Name = item.Name;
            this.Alias = item.Alias;
            this.Description = item.Description;
            this.CreatedDateUtc = item.CreatedDateUtc.AsUtc();
            this.Data = !string.IsNullOrEmpty(item.Data) ? JObject.Parse(item.Data) : null;
        }

        private void Set(Entities.Networks.Skill item)
        {
            this.Set(item, 1);
        }

        private void Set(Entities.Networks.Interest item)
        {
            this.Set(item, 2);
        }

        private void Set(Entities.Networks.Recreation item)
        {
            this.Set(item, 3);
        }

        private void Set(Entities.Networks.ITag item, int categoryId)
        {
            this.Id = item.Id;
            this.Name = item.TagName;
            this.Alias = item.Id.ToString();
            this.CategoryId = categoryId;
        }
    }

    public class CityTagModel : Tag2Model
    {
        public CityTagModel(Sparkle.Entities.Networks.TagDefinition item)
            : base(item)
        {
            var parse = this.Name.Split(new char[] { ',', }, 3);
            this.CityName = parse[0];
            this.CityCountry = "Unknown";
            this.CityContinent = "Others";

            if (parse.Length > 1)
            {
                this.CityCountry = parse[1].Trim();
                if (parse.Length > 2)
                    this.CityContinent = parse[2].Trim();
            }
        }

        public string CityName { get; set; }

        public string CityCountry { get; set; }

        public string CityContinent { get; set; }
    }
}
