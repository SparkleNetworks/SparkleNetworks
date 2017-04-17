
namespace Sparkle.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    public static class CultureHelper
    {
        public static IList<SelectListItem> GetCountries(Func<RegionInfo, bool> selected)
        {
            var list = new List<SelectListItem>();
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    var region = new RegionInfo(culture.LCID);
                    if (list.Any(i => i.Text == region.DisplayName))
                        continue;
                    list.Add(new SelectListItem
                    {
                        Text = region.EnglishName,
                        Value = region.TwoLetterISORegionName,
                        Selected = selected(region),
                    });
                }
                catch (ArgumentException)
                {
                }
            }

            return list.OrderBy(i => i.Text).ToList();
        }

        public static IList<SelectListItem> GetTimezones(Func<TimeZoneInfo, bool> selected)
        {
            var list = new List<SelectListItem>();
            var timezones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var timezone in timezones)
            {
                list.Add(new SelectListItem
                {
                    Text = timezone.DisplayName,
                    Value = timezone.Id,
                    Selected = selected(timezone),
                });
            }
            return list;
        }

        public static IList<SelectListItem> GetCultures(Func<CultureInfo, bool> selected)
        {
            var list = new List<SelectListItem>();
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    var region = new RegionInfo(culture.LCID);
                    if (list.Any(i => i.Text == region.EnglishName))
                        continue;
                    list.Add(new SelectListItem
                    {
                        Text = culture.Name + " " + culture.EnglishName,
                        Value = culture.Name,
                        Selected = selected(culture),
                    });
                }
                catch (ArgumentException)
                {
                }
            }
            return list.OrderBy(i => i.Text).ToList();
        }
    }
}
