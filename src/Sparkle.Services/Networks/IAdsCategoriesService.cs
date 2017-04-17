
namespace Sparkle.Services.Networks
{
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IAdsCategoriesService
    {
        IList<string> OptionsList { get; set; }
        int Insert(AdCategory item);
        void Delete(AdCategory item);
        AdCategory Update(AdCategory item);

        IList<AdCategory> SelectAll();

        AdCategory SelectById(int adCategoryId);

        int Count();

        AdCategory GetById(int id);
    }
}
