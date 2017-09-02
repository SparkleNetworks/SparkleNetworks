
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
    public class PeoplesRecreationsService : ServiceBase, IPeoplesRecreationsService
    {
        /// <summary>
        /// Gets the peoples recreations repository.
        /// </summary>
        protected IPeoplesRecreationsRepository peoplesRecreationsRepository
        {
            get { return this.Repo.PeoplesRecreations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeoplesRecreationsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal PeoplesRecreationsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Updates the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public UserRecreation UpdatePeoplesSkill(UserRecreation item)
        {
            return this.peoplesRecreationsRepository.Update(item);
        }

        /// <summary>
        /// Inserts the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int InsertPeoplesSkill(UserRecreation item)
        {
            return this.peoplesRecreationsRepository.Insert(item).Id;
        }

        /// <summary>
        /// Deletes the peoples skill.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeletePeoplesSkill(UserRecreation item)
        {
            this.peoplesRecreationsRepository.Delete(item);
        }

        /// <summary>
        /// Selects the peoples recreation by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public UserRecreation SelectPeoplesRecreationById(int id)
        {
            return this.peoplesRecreationsRepository.Select()
                .WithId(id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the peoples recreation by skill id and user id.
        /// </summary>
        /// <param name="skillId">The skill id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public UserRecreation SelectPeoplesRecreationBySkillIdAndUserId(int skillId, int userId)
        {
            return this.peoplesRecreationsRepository.Select()
                .WithRecreationIdAndUserId(skillId, userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Selects the peoples recreations by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<UserRecreation> SelectPeoplesRecreationsByUserId(int userId, UserTagOptions options = UserTagOptions.None)
        {
            return this.peoplesRecreationsRepository
                .NewQuery(options)
                .WithUserId(userId)
                .ToList();
        }

        public void AddPeopleRecreation(TagModel item, int userId)
        {
            var recreation = this.Services.Recreations.GetByName(item.Name);
            int recreationId = 0;
            if (recreation == null)
            {
                recreationId = this.Services.Recreations.Insert(new Recreation
                {
                    ParentId = 0,
                    TagName = item.Name.TrimTextRight(50),
                    Date = DateTime.UtcNow,
                    CreatedByUserId = userId,
                });
            }

            this.InsertPeoplesSkill(new UserRecreation
            {
                RecreationId = recreation != null ? recreation.Id : recreationId,
                Date = DateTime.UtcNow,
                UserId = userId,
            });
        }
    }
}
