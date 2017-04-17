
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Services.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class NetworkModel
    {
        public NetworkModel()
        {
        }

        public NetworkModel(Sparkle.Entities.Networks.Network item, Sparkle.Entities.Networks.NetworkType type)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.DisplayName = item.DisplayName;
            this.About = item.About;
            this.Address = item.Address;
            this.City = item.City;
            this.Color = item.Color;
            this.Country = item.Country;
            this.FoursquareId = item.FoursquareId;
            this.lat = item.lat;
            this.lon = item.lon;
            this.NetworkTypeId = item.NetworkTypeId;
            this.Sector = item.Sector;
            this.ZipCode = item.ZipCode;
            this.SiteUrl = item.SiteUrl;
            this.BlogUrl = item.BlogUrl;
            this.TwitterUrl = item.TwitterUrl;
            this.FacebookUrl = item.FacebookUrl;

            if (type == null && item.NetworkTypeReference.IsLoaded)
            {
                type = item.NetworkType;
            }

            if (type != null)
            {
                this.Type = new NetworkTypeModel(type);
            }
            else
            {
                this.Type = new NetworkTypeModel(item.NetworkTypeId);
            }
        }

        public NetworkModel(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }

        [StringLength(48, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "Required")]
        [StringLength(80, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(4000, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string About { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string City { get; set; }

        [StringLength(7, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Color { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Country { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FoursquareId { get; set; }

        public double? lat { get; set; }

        public double? lon { get; set; }

        public int NetworkTypeId { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string Sector { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string SiteUrl { get; set; }

        [StringLength(10, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string ZipCode { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string TwitterUrl { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FacebookUrl { get; set; }

        [Sparkle.Common.DataAnnotations.Url]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string BlogUrl { get; set; }

        public NetworkTypeModel Type { get; set; }

        public NetworkModel Clone()
        {
            return new NetworkModel
            {
                About = this.About,
                Address = this.Address,
                BlogUrl = this.BlogUrl,
                City = this.City,
                Color = this.Color,
                Country = this.Country,
                DisplayName = this.DisplayName,
                FacebookUrl = this.FacebookUrl,
                FoursquareId = this.FoursquareId,
                Id = this.Id,
                lat = this.lat,
                lon = this.lon,
                Name = this.Name,
                NetworkTypeId = this.NetworkTypeId,
                Sector = this.Sector,
                SiteUrl = this.SiteUrl,
                TwitterUrl = this.TwitterUrl,
                Type = this.Type.Clone(),
                ZipCode = this.ZipCode,
            };
        }

        public override string ToString()
        {
            return this.Id + "-" + this.Name;
        }
    }
}
