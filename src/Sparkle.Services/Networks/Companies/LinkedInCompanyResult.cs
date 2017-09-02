
namespace Sparkle.Services.Networks.Companies
{
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Users;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LinkedInCompanyResult : BaseResult<LinkedInCompanyRequest, LinkedInCompanyError>
    {
        public LinkedInCompanyResult(LinkedInCompanyRequest request)
            : base(request)
        {
            this.Changes = new List<CompanyFieldUpdate>();
        }

        public Company CompanyEntity { get; set; }

        public IList<CompanyFieldUpdate> Changes { get; set; }

        public int EntityChanges { get; set; }

        public string[] EmailDomains { get; set; }      // Use for apply proc
    }

    public class CompanyFieldUpdate
    {
        private readonly CompanyProfileFieldModel item;
        private readonly ProfileFieldChange change;

        public CompanyFieldUpdate(CompanyProfileFieldModel model, ProfileFieldChange change)
        {
            this.item = model;
            this.change = change;
        }

        public ProfileFieldChange Change
        {
            get { return this.change; }
        }

        public CompanyProfileFieldModel Item
        {
            get { return this.item; }
        }
    }

    public enum LinkedInCompanyError
    {
    }
}
