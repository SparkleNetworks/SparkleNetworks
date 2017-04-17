
namespace Sparkle.Data.Filters
{
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class UserProfileFieldsFilter
    {
        public static UserProfileField SingleByType(this IEnumerable<UserProfileField> query, ProfileFieldType type)
        {
            return query.Where(o => o.ProfileFieldType == type).SingleOrDefault();
        }

        public static string Site(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Site).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string Phone(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Phone).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource PhoneSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Phone).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string About(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.About).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource AboutSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.About).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string City(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.City).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource CitySource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.City).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string ZipCode(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ZipCode).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource ZipCodeSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ZipCode).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string FavoriteQuotes(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.FavoriteQuotes).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource FavoriteQuotesSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.FavoriteQuotes).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string CurrentTarget(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.CurrentTarget).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource CurrentTargetSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.CurrentTarget).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string Contribution(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Contribution).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource ContributionSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Contribution).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string Country(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Country).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource CountrySource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Country).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string ContactGuideline(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ContactGuideline).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource ContactGuidelineSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ContactGuideline).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string Industry(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Industry).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource IndustrySource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Industry).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string LinkedInPublicUrl(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.LinkedInPublicUrl).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource LinkedInPublicUrlSource(this IEnumerable<UserProfileField> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.LinkedInPublicUrl).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }
    }

    public static class UserProfileFieldsPocoFilter
    {
        public static string Site(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Site).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string Phone(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Phone).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource PhoneSource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Phone).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string About(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.About).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource AboutSource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.About).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string City(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.City).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource CitySource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.City).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string ZipCode(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ZipCode).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string FavoriteQuotes(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.FavoriteQuotes).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string CurrentTarget(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.CurrentTarget).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource? CurrentTargetSource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.CurrentTarget).SingleOrDefault();
            if (item != null)
                return new ProfileFieldSource?(item.SourceType);

            return null;
        }

        public static string Contribution(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Contribution).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static string Country(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Country).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource? CountrySource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Country).SingleOrDefault();
            if (item != null)
                return new ProfileFieldSource?(item.SourceType);

            return null;
        }

        public static string ContactGuideline(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ContactGuideline).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource ContactGuidelineSource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.ContactGuideline).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string Industry(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Industry).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource IndustrySource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.Industry).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }

        public static string LinkedInPublicUrl(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.LinkedInPublicUrl).SingleOrDefault();
            if (item != null)
                return item.Value;

            return null;
        }

        public static ProfileFieldSource LinkedInPublicUrlSource(this IEnumerable<UserProfileFieldPoco> query)
        {
            var item = query.Where(o => o.ProfileFieldType == ProfileFieldType.LinkedInPublicUrl).SingleOrDefault();
            return item != null ? item.SourceType : ProfileFieldSource.None;
        }
    }
}
