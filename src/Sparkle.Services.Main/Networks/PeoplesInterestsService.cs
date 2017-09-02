
namespace Sparkle.Services.Main.Networks
{
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models.Tags;
    using System;

    /// <summary>
    /// THIS IS PART OF TAGS V1 PACKAGE. THIS HAS TO BE REMOVED.
    /// </summary>
    public class PeoplesInterestsService : ServiceBase, IPeoplesInterestsService
    {
        /// <summary>
        /// Gets the peoples interests repository.
        /// </summary>
        protected IPeoplesInterestsRepository peoplesInterestsRepository
        {
            get { return this.Repo.PeoplesInterests; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeoplesInterestsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PeoplesInterestsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Updates the peoples interest.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public UserInterest UpdatePeoplesInterest(UserInterest item)
        {
            return this.peoplesInterestsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the peoples interest.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int InsertPeoplesInterest(UserInterest item)
        {
            return this.peoplesInterestsRepository.Insert(item).Id;
        }

        /// <summary>
        /// Deletes the peoples interest.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeletePeoplesInterest(UserInterest item)
        {
            this.peoplesInterestsRepository.Delete(item);
        }

        /// <summary>
        /// Selects the peoples interest by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public UserInterest SelectPeoplesInterestById(int id)
        {
            return this.peoplesInterestsRepository.Select()
                .WithId(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the peoples interest by skill id and user id.
        /// </summary>
        /// <param name="interestId">The skill id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public UserInterest SelectPeoplesInterestBySkillIdAndUserId(int interestId, int userId)
        {
            return this.peoplesInterestsRepository.Select()
                .WithInterestIdAndUserId(interestId, userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the peoples interests by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<UserInterest> SelectPeoplesInterestsByUserId(int userId, UserTagOptions options = UserTagOptions.None)
        {
            return this.peoplesInterestsRepository
                .NewQuery(options)
                .WithUserId(userId)
                .ToList();
        }

        public int[] GetInterestsIdsByUserId(int userId)
        {
            return this.Repo.PeoplesInterests
                .GetInterestsByUserId(userId)
                .Select(o => o.InterestId)
                .ToArray();
        }

        public void AddPeopleInterest(TagModel item, int userId)
        {
            var interest = this.Services.Interests.GetByName(item.Name);
            if (interest == null)
            {
                interest = this.Services.Interests.Insert(new Interest
                {
                    ParentId = 0,
                    TagName = item.Name.TrimTextRight(50),
                    Date = DateTime.UtcNow,
                    CreatedByUserId = userId,
                });
            }

            this.InsertPeoplesInterest(new UserInterest
            {
                InterestId = interest.Id,
                Date = DateTime.UtcNow,
                UserId = userId,
            });
        }
    }
}
