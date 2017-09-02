
namespace Sparkle.Services.Networks.Models
{
    using FourSquare.SharpSquare.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class PlaceModel : IAspectObject
    {
        private readonly AspectList root;
        private string symbolName;

        public PlaceModel()
        {
            this.root = new AspectList(this.GetType());
        }

        public PlaceModel(Entities.Networks.Place item)
            : this()
        {
            this.Set(item);
        }

        public PlaceModel(int id)
            : this()
        {
            this.Id = id;
        }

        public PlaceModel(Venue item, Func<string, string> langMaker)
            : this()
        {
            this.Set(item, langMaker);
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int? ParentId { get; set; }

        [DataMember]
        public int? CompanyId { get; set; }

        [DataMember]
        public int CreatedByUserId { get; set; }

        [DataMember]
        public int NetworkId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int CategoryId { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public double? Latitude { get; set; }

        [DataMember]
        public double? Longitude { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public int? Floor { get; set; }

        [DataMember]
        public string Door { get; set; }

        [DataMember]
        public string Building { get; set; }

        [DataMember]
        public string FoursquareId { get; set; }

        [DataMember(Name = "IsMainNetworkPlace")]
        public bool IsMain { get; set; }

        [DataMember]
        public PlaceCategoryModel Category { get; set; }

        [DataMember]
        public string GeographicDistanceKilometers
        {
            get { return this.GeographicDistance.HasValue ? (this.GeographicDistance.Value / 1000.0).ToString("0.0") : null; }
        }

        public IList<PlaceModel> Children { get; set; }

        public System.Data.Spatial.DbGeography Geography { get; set; }

        public double? GeographicDistance { get; set; }

        [DataMember]
        public GeographyModel Location { get; set; }

        public CompanyModel Company { get; set; }

        public PlaceModel Parent { get; set; }

        public AspectList AspectManager
        {
            get { return this.root; }
        }

        [DataMember]
        public string SymbolRelativeUrl
        {
            get
            {
                var name = "default";
                if (this.Category != null && !string.IsNullOrEmpty(this.Category.Symbol))
                    name = this.Category.Symbol;

                return "/Content/places/" + Uri.EscapeDataString(name) + ".png";
            }
        }

        // indicates this place is not yet in database
        [DataMember]
        public bool IsFoursquarePlace { get; set; }

        public string RelativePlaceUrl
        {
            get { return "/Place/" + this.Alias; }
        }

        [DataMember]
        public string AddressString
        {
            get
            {
                var elements = new List<string>(4);
                elements.Add(this.Address);
                elements.Add(this.ZipCode);
                elements.Add(this.City);
                elements.Add(this.Country);
                elements.RemoveAll(x => string.IsNullOrWhiteSpace(x));

                if (elements.Count > 1)
                    return string.Join(", ", elements);
                else
                    return null;
            }
            set { }
        }

        public override string ToString()
        {
            return "Place #" + this.Id + (this.ParentId != null ? (" Parent=" + this.ParentId.Value) : null) + " " + this.Name;
        }

        private void Set(Entities.Networks.Place item)
        {
            if (item != null)
            {
                this.Id = item.Id;
                this.ParentId = item.ParentId;
                this.CompanyId = item.CompanyOwner;
                this.CreatedByUserId = item.CreatedByUserId;
                this.NetworkId = item.NetworkId;
                
                this.Name = item.Name;
                this.Alias = item.Alias;
                this.Description = item.Description;
                
                this.Address = item.Address;
                this.CategoryId = item.CategoryId;
                this.City = item.City;
                this.Country = item.Country;
                this.Latitude = item.lat;
                this.Longitude = item.lon;
                this.Phone = item.Phone;
                this.ZipCode = item.ZipCode;
                this.Floor = item.Floor;
                this.Door = item.Door;
                this.Building = item.Building;

                this.FoursquareId = item.FoursquareId;
                
                this.IsMain = item.Main;
                this.Geography = item.Geography;

                if (item.Geography != null)
                {
                    this.Location = new GeographyModel(item.Geography);
                }
                else if (item.lat != null && item.lon != null)
                {
                    this.Location = new GeographyModel(item.lat.Value, item.lon.Value);
                }
                
                this.Category = new PlaceCategoryModel(item.CategoryId);
            }
        }

        private void Set(Venue item, Func<string, string> langMaker)
        {
            if (item != null)
            {
                this.IsFoursquarePlace = true;

                this.FoursquareId = item.id;
                this.Name = item.name;

                Category category = null;
                if (item.categories != null && item.categories.Count > 0 && (category = item.categories.SingleOrDefault(o => o.primary)) != null)
                {
                    this.Description = langMaker(category.name);
                }

                if (item.location != null)
                {
                    this.Location = new GeographyModel(item.location.lat, item.location.lng);

                    var address = item.location.address;
                    var city = item.location.city;
                    if (category != null && (!string.IsNullOrEmpty(address) || !string.IsNullOrEmpty(city)))
                    {
                        this.Description += " - ";
                        if (!string.IsNullOrEmpty(address))
                        {
                            if (!string.IsNullOrEmpty(city))
                                this.Description += address + ", " + city;
                            else
                                this.Description += address;
                        }
                        else
                        {
                            this.Description += city;
                        }
                    }
                }
            }
        }
    }
}
