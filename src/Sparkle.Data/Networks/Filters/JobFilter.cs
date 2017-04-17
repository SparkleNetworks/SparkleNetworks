// <copyright file="JobFilter.cs" company="Sparkle Networks">
//     Sparkle Networks. All rights reserved.
// </copyright>
// <author>kevin.alexandre</author>

namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    /// <summary>
    /// the job filter
    /// </summary>
    public static class JobFilter
    {
        /// <summary>
        /// filter with the job id.
        /// </summary>
        /// <param name="qry">The job query.</param>
        /// <param name="id">The job id.</param>
        /// <returns>list of jobs</returns>
        public static IQueryable<Job> WithId(this IQueryable<Job> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Job> WithAlias(this IQueryable<Job> qry, string alias)
        {
            return qry.Where(o => o.Alias == alias);
        }


        public static IQueryable<Job> Contain(this IQueryable<Job> qry, string request)
        {
            return qry.Where(o =>
                o.Libelle.ToLower().Contains(request)
                );
        }
    }
}