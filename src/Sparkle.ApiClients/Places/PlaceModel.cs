
namespace Sparkle.ApiClients.Places
{
    using Sparkle.ApiClients.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class PlaceModel
    {
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

        ////[DataMember]
        ////public PlaceCategoryModel Category { get; set; }

        [DataMember]
        public string GeographicDistanceKilometers { get; set; }

        ////[DataMember]
        ////public GeographyModel Location { get; set; }

        [DataMember]
        public string SymbolRelativeUrl { get; set; }

        // indicates this place is not yet in database
        [DataMember]
        public bool IsFoursquarePlace { get; set; }

        [DataMember]
        public string AddressString { get; set; }
    }
}
