
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompanyListModel
    {
        public int Offset { get; set; }

        public int Count { get; set; }

        public int Total { get; set; }

        public IList<CompanyModel> Items { get; set; }

        public string[] LocationGeocodes { get; set; }
    }
}
