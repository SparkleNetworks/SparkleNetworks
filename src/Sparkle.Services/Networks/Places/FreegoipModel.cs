
namespace Sparkle.Services.Networks.Places
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class FreeGeoIpModel
    {
        [DataMember(Name = "ip")]
        public string Ip { get; set; }

        [DataMember(Name = "country_code")]
        public string CountryCode { get; set; }

        [DataMember(Name = "country_name")]
        public string CountryName { get; set; }

        [DataMember(Name = "region_code")]
        public string RegionCode { get; set; }

        [DataMember(Name = "region_name")]
        public string RegionName { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip_code")]
        public string ZipCode { get; set; }

        [DataMember(Name = "time_zone")]
        public string Timezone { get; set; }

        [DataMember(Name = "latitude")]
        public double? Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double? Longitude { get; set; }

        [DataMember(Name = "metro_code")]
        public string MetroCode { get; set; }
    }
}
