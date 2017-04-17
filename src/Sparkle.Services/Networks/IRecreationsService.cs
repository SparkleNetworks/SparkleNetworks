
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Tags;

    public interface IRecreationsService
    {
        void Delete(Recreation item);
        int Insert(Recreation item);
        IList<Recreation> Search(string request, int take);
        IList<Recreation> SelectAll();
        Recreation GetById(int id);
        Recreation SelectRecreationsByName(string name);
        IList<Recreation> SelectRecreationsFromUserId(Guid userId);
        Recreation Update(Recreation item);

        Recreation GetByName(string name);

        IDictionary<int, Tag2Model> GetAllForCache();
    }
}
