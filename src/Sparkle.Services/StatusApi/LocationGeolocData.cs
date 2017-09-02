
namespace Sparkle.Services.StatusApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class LocationGeolocData
    {
        [DataMember]
        public string[] Geocodes { get; set; }
    }
}
