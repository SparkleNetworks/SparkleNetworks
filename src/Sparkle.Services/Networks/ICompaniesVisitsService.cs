using System;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface ICompaniesVisitsService
    {
        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        int Insert(CompaniesVisit item);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        CompaniesVisit Update(CompaniesVisit item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(CompaniesVisit item);

        /// <summary>
        /// Gets the by company id and user id and day.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        CompaniesVisit GetByCompanyIdAndUserIdAndDay(int companyId, int userId, DateTime date);

        /// <summary>
        /// Counts the by company and day.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        int CountByCompanyAndDay(int companyId, DateTime day);

        /// <summary>
        /// Gets the by company and day.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        System.Collections.Generic.List<CompaniesVisit> GetByCompanyAndDay(int companyId, DateTime day);

        /// <summary>
        /// Selects the by company id.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        System.Collections.Generic.List<CompaniesVisit> SelectByCompanyId(int companyId);


        /// <summary>
        /// Counts the by company and month.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        int CountByCompanyAndMonth(int companyId, DateTime month);

        /// <summary>
        /// Counts the by company and range.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <param name="firstDate">The first date.</param>
        /// <param name="lastDate">The last date.</param>
        /// <returns></returns>
        int CountByCompanyAndRange(int companyId, DateTime firstDate, DateTime lastDate);

        /// <summary>
        /// Counts the by company.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        int CountByCompany(int companyId);
    }
}
