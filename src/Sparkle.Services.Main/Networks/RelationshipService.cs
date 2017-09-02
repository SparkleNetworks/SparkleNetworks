
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;

    public class RelationshipService : ServiceBase, IRelationshipService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal RelationshipService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the relationship repository.
        /// </summary>
        protected IRelationshipRepository RelationshipRepository
        {
            get { return this.Repo.Relationship; }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public IList<Relationship> SelectAll()
        {
            return this.RelationshipRepository.Select()
                    .ToList();
        }

        /// <summary>
        /// Selects the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Relationship SelectById(int id)
        {
            return this.RelationshipRepository.Select()
                .WithId(id)    
                .FirstOrDefault();
        }

    }
}
