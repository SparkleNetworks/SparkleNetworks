// <copyright file="CompanyFilter.cs" company="Sparkle Networks">
//     Sparkle Networks. All rights reserved.
// </copyright>
// <author>kevin.alexandre</author>


namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    /// <summary>
    /// the company filter
    /// </summary>
    public static class CompanyFilter
    {
        /// <summary>
        /// filter with the id.
        /// </summary>
        /// <param name="qry">The company query.</param>
        /// <param name="id">The company id.</param>
        /// <returns>list of companies</returns>
        public static IQueryable<Company> WithId(this IQueryable<Company> qry, long id)
        {
            return qry.Where(o => o.ID == id);
        }

        public static IQueryable<Company> WithAlias(this IQueryable<Company> qry, string alias)
        {
            return qry.Where(o => o.Alias == alias);
        }

        public static IQueryable<Company> HasDomainName(this IQueryable<Company> qry, string domainName)
        {
            return qry.Where(o => !string.IsNullOrEmpty(o.EmailDomain) && o.EmailDomain.ToLower().Contains(domainName));
        }

        /// <summary>
        /// Contains the specified qry.
        /// </summary>
        /// <param name="qry">The company query.</param>
        /// <param name="request">The contain request.</param>
        /// <returns>list of companies</returns>
        public static IQueryable<Company> Contain(this IQueryable<Company> qry, string request)
        {
            return qry.Where(o => !string.IsNullOrEmpty(o.Name) && o.Name.ToLower().Contains(request));
        }

        public static IQueryable<Company> Active(this IQueryable<Company> query)
        {
            return query.Where(c => c.IsApproved);
        }

        public static IQueryable<Company> Inactive(this IQueryable<Company> query)
        {
            return query.Where(c => !c.IsApproved);
        }

        public static IQueryable<Company> Enabled(this IQueryable<Company> query)
        {
            return query.Where(c => c.IsEnabled);
        }

        public static IQueryable<Company> Disabled(this IQueryable<Company> query)
        {
            return query.Where(c => !c.IsEnabled);
        }
    }

    public static class CompanyAdminFilter
    {
        public static IQueryable<CompanyAdmin> WithId(this IQueryable<CompanyAdmin> qry, long companyId)
        {
            return qry.Where(o => o.CompanyId == companyId);
        }

        public static IQueryable<CompanyAdmin> WithCompanyIdAndUserId(this IQueryable<CompanyAdmin> qry, long companyId, int userId)
        {
            return qry.Where(o => o.CompanyId == companyId && o.UserId == userId);
        }
    }
}