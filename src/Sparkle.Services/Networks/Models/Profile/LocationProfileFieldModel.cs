
namespace Sparkle.Services.Networks.Models.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LocationProfileFieldModel
    {
        public string Description { get; set; }
        public bool IsHeadquarters { get; set; }
        public bool IsActive { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string RegionCode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }

        public void UpdateFrom(LinkedInNET.Companies.CompanyLocation item)
        {
            this.Description = item.Description;
            this.IsHeadquarters = item.IsHeadquarters;
            this.IsActive = item.IsActive;
            this.Street1 = item.Address != null ? item.Address.Street1 : null;
            this.Street2 = item.Address != null ? item.Address.Street2 : null;
            this.City = item.Address != null ? item.Address.City : null;
            this.State = item.Address != null ? item.Address.State : null;
            this.PostalCode = item.Address != null ? item.Address.PostalCode : null;
            this.CountryCode = item.Address != null ? item.Address.CountryCode : null;
            this.RegionCode = item.Address != null ? item.Address.RegionCode : null;
            this.Phone1 = item.ContactInfo != null ? item.ContactInfo.Phone1 : null;
            this.Phone2 = item.ContactInfo != null ? item.ContactInfo.Phone2 : null;
            this.Fax = item.ContactInfo != null ? item.ContactInfo.Fax : null;
        }
    }
}
