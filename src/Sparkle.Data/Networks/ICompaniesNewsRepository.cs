
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface ICompaniesNewsRepository
    {
        IQueryable<news> SelectCompaniesNews();
        void DeleteCompaniesNews(CompaniesNews item);
        int InsertCompaniesNews(CompaniesNews item);
        CompaniesNews UpdateCompaniesNews(CompaniesNews item);
    }
}