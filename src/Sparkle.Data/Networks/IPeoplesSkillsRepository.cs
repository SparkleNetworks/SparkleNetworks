
namespace Sparkle.Data.Networks
{
    using System.Linq;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;

    [Repository]
    public interface IPeoplesInterestsRepository : IBaseNetworkRepository<UserInterest, int>, ITagsV1RelationRepository
    {
        IList<UserInterest> GetInterestsByUserId(int userId);

        IQueryable<UserInterest> NewQuery(UserTagOptions options);
    }

    [Repository]
    public interface IPeoplesRecreationsRepository : IBaseNetworkRepository<UserRecreation, int>, ITagsV1RelationRepository
    {
        IQueryable<UserRecreation> NewQuery(UserTagOptions options);
    }
}
