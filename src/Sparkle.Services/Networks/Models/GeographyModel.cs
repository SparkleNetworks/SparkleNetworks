
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Spatial;
    using System.Linq;
    using System.Text;

    public class GeographyModel
    {
        const string strf = "D2";

        public GeographyModel()
        {
        }

        public GeographyModel(DbGeography item)
        {
            this.Set(item);
        }

        public GeographyModel(double lat, double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string LatitudeStr
        {
            get
            {
                if (this.Latitude == null)
                    return null;

                var val = this.Latitude.Value;
                var p1 = Math.Floor(Math.Abs(val));
                var p2 = Math.Floor(Math.Abs((val - p1) * 60));
                var p3 = Math.Floor(Math.Abs((val - p1 - p2 / 60) * 3600));

                return p1 + "° " + ((int)p2).ToString(strf) + "' " + ((int)p3).ToString(strf) + "\" " + (val >= 0 ? "N" : "S");
            }
        }

        public string LongitudeStr
        {
            get
            {
                if (this.Longitude == null)
                    return null;

                var val = this.Longitude.Value;
                var p1 = Math.Floor(Math.Abs(val));
                var p2 = Math.Floor(Math.Abs((val - p1) * 60));
                var p3 = Math.Floor(Math.Abs((val - p1 - p2 / 60) * 3600));

                return p1 + "° " + ((int)p2).ToString(strf) + "' " + ((int)p3).ToString(strf) + "\" " + (val >= 0 ? "E" : "W");
            }
        }

        private void Set(DbGeography item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            this.Latitude = item.Latitude;
            this.Longitude = item.Longitude;
        }
    }
}
