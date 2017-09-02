
namespace Sparkle.NetworksStatus.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class GeocodeFarmModel
    {
        [DataMember(Name = "geocoding_results")]
        public GeocodingResult Result { get; set; }
    }

    public class GeocodingResult
    {
        // LEGAL_COPYRIGHT

        // STATUS

        // ACCOUNT

        // RESULTS
        [DataMember(Name = "RESULTS")]
        public IList<GeocodingResults> Results { get; set; }
    }

    public class GeocodingResults
    {
        [DataMember(Name = "result_number")]
        public int ResultNumber { get; set; }

        [DataMember(Name = "formatted_address")]
        public string FormattedAddress { get; set; }

        [DataMember(Name = "accuracy")]
        public string Accuracy { get; set; }
        public GeocodeFarmAccuracy? AccuracyValue
        {
            get
            {
                GeocodeFarmAccuracy val;
                if (Enum.TryParse(this.Accuracy, out val))
                    return val;

                return default(GeocodeFarmAccuracy?);
            }
        }

        // ADDRESS

        // LOCATION_DETAILS

        [DataMember(Name = "COORDINATES")]
        public GeocodeFarmCoordinates Coordinates { get; set; }
    }

    public class GeocodeFarmCoordinates
    {
        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }
    }

    public enum GeocodeFarmAccuracy
    {
        EXACT_MATCH,
        HIGH_ACCURACY,
        MEDIUM_ACCURACY,
        UNKNOWN_ACCURACY
    }
}
