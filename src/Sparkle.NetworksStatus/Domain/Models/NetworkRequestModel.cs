
namespace Sparkle.NetworksStatus.Domain.Models
{
    using Sparkle.NetworksStatus.Data;
    using Sparkle.NetworksStatus.Domain.Lang;
    using SrkToolkit.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class NetworkRequestModel
    {
        public NetworkRequestModel()
        {
        }

        public NetworkRequestModel(NetworkRequest item)
        {
            this.Id = item.Id;
            this.WebId = item.WebId ?? Guid.Empty;
            this.AdminCode = item.AdminCode ?? Guid.Empty;
            this.DateCreatedUtc = item.DateCreatedUtc.AsUtc();
            this.RemoteAddress = item.RemoteAddress;
            this.Culture = item.Culture;

            this.ContactEmailAccount = item.ContactEmailAccount;
            this.ContactEmailDomain = item.ContactEmailDomain;
            this.ContactEmailTag = item.ContactEmailTag;
            this.ContactFirstname = item.ContactFirstname;
            this.ContactLastname = item.ContactLastname;
            this.ContactPhoneNumber = item.ContactPhoneNumber;

            if (this.ContactEmailAccount != null && this.ContactEmailDomain != null)
            {
                var email = new SrkToolkit.Common.Validation.EmailAddress(this.ContactEmailAccount, this.ContactEmailTag, this.ContactEmailDomain);
                this.ContactEmailString = email.Value;
            }

            this.NetworkCity = item.NetworkCity;
            this.NetworkCountry = item.NetworkCountry;
            this.NetworkName = item.NetworkName;
            this.NetworkSize = item.NetworkSize;
            this.NetworkSubdomain = item.NetworkSubdomain;
        }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public DateTime DateCreatedUtc { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string RemoteAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string Culture { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string ContactFirstname { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string ContactLastname { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        [PhoneNumber]
        public string ContactPhoneNumber { get; set; }

        public string ContactEmailAccount { get; set; }
        public string ContactEmailTag { get; set; }
        public string ContactEmailDomain { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        [EmailAddressEx]
        public string ContactEmailString { get; set; }
        ////{
        ////    get { return this.contactEmailString; }
        ////    set
        ////    {
        ////        this.contactEmailString = value;
        ////        if (string.IsNullOrEmpty(value))
        ////        {
        ////            this.ContactEmailTag = this.ContactEmailAccount = this.ContactEmailDomain = null;
        ////        }
        ////        else
        ////        {
        ////            SrkToolkit.Common.Validation.EmailAddress email = null;
        ////            if (((email = SrkToolkit.Common.Validation.EmailAddress.TryCreate(value)) != null))
        ////            {
        ////                this.ContactEmailAccount = email.AccountPart;
        ////                this.ContactEmailTag = email.TagPart;
        ////                this.ContactEmailDomain = email.DomainPart;
        ////            }
        ////            else
        ////            {
        ////                this.ContactEmailTag = this.ContactEmailAccount = this.ContactEmailDomain = null;
        ////            }
        ////        }
        ////    }
        ////}

        public Guid AdminCode { get; set; }

        public Guid WebId { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string NetworkCity { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        [Display(Name = "Label_Country", ResourceType = typeof(ValidationStrings))]
        public string NetworkCountry { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string NetworkName { get; set; }

        [Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public long NetworkSize { get; set; }

        ////[Required(ErrorMessageResourceType = typeof(ValidationStrings), ErrorMessageResourceName = "Required")]
        public string NetworkSubdomain { get; set; }

        public IList<System.Globalization.RegionInfo> AvailableCountries { get; set; }
    }
}
