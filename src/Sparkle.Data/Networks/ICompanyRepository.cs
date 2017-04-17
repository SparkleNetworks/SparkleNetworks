
namespace Sparkle.Data.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks.Neutral;

    /// <summary>
    /// 
    /// </summary>
    [Repository("Companies")]
    public interface ICompanyRepository : IBaseNetworkRepository<Company, int>
    {
        IQueryable<Company> CreateQuery(CompanyOptions options);

        /// <summary>
        /// Gets all with stats and skills.
        /// </summary>
        /// <returns></returns>
        IList<CompanyExtended> GetAllWithStatsAndSkills();

        /// <summary>
        /// Gets the with stats and skills.
        /// </summary>
        /// <param name="networkId">The network id.</param>
        /// <returns></returns>
        IList<CompanyExtended> GetWithStatsAndSkills(int networkId);

        /// <summary>
        /// Gets the with stats and skills.
        /// </summary>
        /// <param name="networkId">The network id.</param>
        /// <returns></returns>
        IList<Sparkle.Entities.Networks.Neutral.CompanyPoco> GetWithStatsAndSkillsAndJobs(int networkId);

        /// <summary>
        /// Gets the with stats.
        /// </summary>
        /// <param name="networkId">The network id.</param>
        /// <returns></returns>
        IList<CompanyExtended> GetWithStats(int networkId);

        IList<CompanyExtended> GetInactiveWithStats(int networkId);

        /// <summary>
        /// Gets the name of the with network.
        /// </summary>
        /// <returns></returns>
        IList<CompanyExtended> GetWithNetworkName();

        /// <summary>
        /// Gets the waiting approbation.
        /// </summary>
        /// <param name="networkId">The network id.</param>
        /// <returns></returns>
        IList<CompanyExtended> GetWaitingApprobation(int networkId);

        Company GetByAlias(string alias, CompanyOptions options);
        int? GetIdByAlias(string alias);

        int CountWaitingApprobation(int networkId);

        Company GetById(int id, CompanyOptions options);
        IList<Company> GetById(int[] ids);
        IList<Company> GetActiveById(int[] ids);
        IList<Company> GetById(int[] ids, int networkId);
        IList<Company> GetActiveById(int[] ids, int networkId);
        IDictionary<int, Company> GetById(IList<int> ids, CompanyOptions options);

        IList<Company> GetAllForImportScripts(int networkId);

        IList<Person> GetAdministrators(int companyId, int networkId);

        IList<GetCompaniesAccessLevelReport_Result> GetCompaniesAccessLevelReport(int networkId);
        GetCompaniesAccessLevelReport_Result GetCompaniesAccessLevelReport(int networkId, int companyId);

        IList<Company> GetAllActive(int networkId);
        IList<Company> GetActiveByName(int networkId, string name);

        IList<CompanySearchRow> Search(int[] networkId, string[] splittedKeywords, string[] geocodes, int[] tagIds, bool combinedTags, int offset, int count, CompanyOptions options);
    }

    [Repository]
    public interface ICompanyAdminRepository : IBaseNetworkRepository<CompanyAdmin, int>
    {
    }
}
