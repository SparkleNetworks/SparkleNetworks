
namespace Sparkle.Data.Entity.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Objects;

    public class PeoplesInterestsRepository : BaseNetworkRepositoryInt<UserInterest>, IPeoplesInterestsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PeoplesInterestsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserInterests)
        {
        }

        public IList<UserInterest> GetInterestsByUserId(int userId)
        {
            return this.Set
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public IQueryable<UserInterest> NewQuery(UserTagOptions options)
        {
            ObjectQuery<UserInterest> query = this.Set;

            if ((options & UserTagOptions.Tag) == UserTagOptions.Tag)
                query = query.Include("Interest");

            if ((options & UserTagOptions.User) == UserTagOptions.User)
                query = query.Include("User");

            return query;
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.InterestId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }

    public class PeoplesRecreationsRepository : BaseNetworkRepositoryInt<UserRecreation>, IPeoplesRecreationsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public PeoplesRecreationsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.UserRecreations)
        {
        }

        public IQueryable<UserRecreation> NewQuery(UserTagOptions options)
        {
            ObjectQuery<UserRecreation> query = this.Set;

            if ((options & UserTagOptions.Tag) == UserTagOptions.Tag)
                query = query.Include("Recreation");

            if ((options & UserTagOptions.User) == UserTagOptions.User)
                query = query.Include("User");

            return query;
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetAll()
        {
            return this.Set.ToList().Cast<ITagV1Relation>().ToList();
        }

        IList<ITagV1Relation> ITagsV1RelationRepository.GetByTagId(int tagId)
        {
            return this.Set.Where(x => x.RecreationId == tagId).ToList().Cast<ITagV1Relation>().ToList();
        }
    }
}
