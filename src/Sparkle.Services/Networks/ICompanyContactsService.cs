using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ICompanyContactsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        CompanyContact Insert(CompanyContact item);

        /// <summary>
        /// Selects the by to company id.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        IList<CompanyContact> SelectByToCompanyId(int companyId);

        /// <summary>
        /// Selects the by to company id and from company id.
        /// </summary>
        /// <param name="toCompanyId">To company id.</param>
        /// <param name="fromCompanyId">From company id.</param>
        /// <returns></returns>
        IList<CompanyContact> SelectByToCompanyIdAndFromCompanyId(int toCompanyId, int fromCompanyId);

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        CompanyContact GetById(int id);

        /// <summary>
        /// Selects the by to company id and from user email.
        /// </summary>
        /// <param name="toCompanyId">To company id.</param>
        /// <param name="fromUserEmail">From user email.</param>
        /// <returns></returns>
        IList<CompanyContact> SelectByToCompanyIdAndFromUserEmail(int toCompanyId, string fromUserEmail);
    }
}
