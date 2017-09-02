
namespace Sparkle.Services.Networks.Users
{
    using Sparkle.Services.Networks.Lang;
    using SrkToolkit.DataAnnotations;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class RegionSettingsRequest : BaseRequest
    {
        [Timezone]
        [Display(Name = "Timezone", ResourceType = typeof(NetworksLabels))]
        public string TimezoneId { get; set; }

        public TimeZoneInfo DefaultTimezone { get; set; }

        [CultureInfo]
        [Display(Name = "Culture_", ResourceType = typeof(NetworksLabels))]
        public string CultureName { get; set; }

        public IDictionary<string, string> AvailableTimezones { get; set; }

        public IDictionary<string, string> AvailableCultures { get; set; }


        public string ReturnUrl { get; set; }
    }

    public class RegionSettingsResult : BaseResult<RegionSettingsRequest, RegionSettingsError>
    {
        public RegionSettingsResult(RegionSettingsRequest request)
            : base(request)
        {
        }
    }

    public enum RegionSettingsError
    {
        NoSuchUserOrInactive,
        TimezoneNotSupported,
        CultureNotSupported,
    }
}
