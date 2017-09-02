
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IHintsService
    {
        void Initialize();

        HintToUserModel GetUserRelation(string hintAlias, int userId);
        HintToUserModel SetUserRelation(int hintId, int userId, DateTime dateDismissedUtc);

        IDictionary<int, HintModel> GetAllForCache();

        HintModel GetByAlias(string alias);

        HintModel GetByAliasNoCache(string alias);
    }
}
