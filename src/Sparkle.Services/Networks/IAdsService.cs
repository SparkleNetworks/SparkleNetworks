
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Ads;
    using Sparkle.Services.Networks.Tags;

    public interface IAdsService
    {
        EditAdRequest GetEditRequest(int? id, EditAdRequest request);
        EditAdResult Edit(EditAdRequest request);
        AdModel GetById(int id, AdOptions options);
        IDictionary<int, AdModel> GetById(int[] id, AdOptions options);
        AdModel GetByAlias(string alias, AdOptions options);

        [Obsolete]
        int Publish(User me, Ad item);
        [Obsolete]
        void Delete(Ad item);
        [Obsolete]
        Ad Update(Ad item);

        [Obsolete]
        IList<Ad> SelectAll();

        [Obsolete]
        Ad SelectById(int adId);

        int Count();

        [Obsolete]
        IDictionary<int, Ad> GetByIdFromAnyNetwork(IList<int> ids, AdOptions options);
        [Obsolete]
        IDictionary<int, Ad> GetByIdInNetwork(int[] adIds, AdOptions options);

        string GetProfileUrl(Ad ad, UriKind uriKind);

        IList<AdModel> GetList(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options);
        IList<AdModel> GetUserList(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options, int userId);
        IList<AdModel> GetByDateRange(Ad.Columns sort, bool desc, bool openOnly, int offset, int pageSize, AdOptions options, DateTime from, DateTime to);
        int CountByDateRange(bool openOnly, DateTime past, DateTime start);

        IList<AdModel> GetPendingList(AdOptions options);
        int GetPendingCount();

        int Count(bool openOnly);

        bool IsUserAuthorized(int adId, int actingUserId, AdAction adAction);

        IList<Tag2Model> GetTypes();

        ValidateAdResult Validate(ValidateAdRequest request);

        int CountNewAdsForUser(int userId);


    }
}
