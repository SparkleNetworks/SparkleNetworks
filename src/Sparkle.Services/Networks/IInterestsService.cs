
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Tags;

    public interface IInterestsService
    {
        void Delete(Interest item);
        Interest Insert(Interest item);
        IList<Interest> Search(string request, int take);
        IList<Interest> SelectAll();
        Interest GetById(int id);
        Interest SelectInterestsByName(string name);
        IList<Interest> SelectInterestsFromUserId(int userId);
        Interest Update(Interest item);

        IDictionary<string, int> GetByNames(string[] names);

        Interest GetByName(string name);

        IDictionary<int, Tag2Model> GetAllForCache();
    }
}
