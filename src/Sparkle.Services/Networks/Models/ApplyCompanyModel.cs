
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Services.Networks.Lang;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class ApplyCompanyModel
    {
        public IList<CompanyModel> Companies { get; set; }

        public IDictionary<Guid, CompanyModel> LinkedInCompanies { get; set; }

        public CompanyModel DomainNameMatch { get; set; }

        public Tuple<Guid, CompanyModel> LinkedInDomainNameMatch { get; set; }

        public CompanyModel DefaultCompany { get; set; }

        public string CompanyId { get; set; }
    }
}
