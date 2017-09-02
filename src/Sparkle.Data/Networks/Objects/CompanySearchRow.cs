
namespace Sparkle.Data.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompanySearchRow
    {
        public int Id { get; set; }

        public int? PlaceId { get; set; }

        public int NetworkId { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public double? Distance { get; set; }
    }
}
