
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;

    public class TeamsMembersService : ServiceBase, ITeamsMembersService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsMembersService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal TeamsMembersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the teams members service repository.
        /// </summary>
        protected ITeamsMembersRepository TeamsMembersServiceRepository
        {
            get { return this.Repo.TeamsMembers; }
        }

        /// <summary>
        /// Adds the team member.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public TeamMember AddTeamMember(TeamMember item)
        {
            return this.TeamsMembersServiceRepository.Insert(item);
        }

        /// <summary>
        /// Updates the team member.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public TeamMember UpdateTeamMember(TeamMember item)
        {
            return this.TeamsMembersServiceRepository.Update(item);
        }

        /// <summary>
        /// Deletes the team member.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeleteTeamMember(TeamMember item)
        {
            this.TeamsMembersServiceRepository.Delete(item);
        }

        /// <summary>
        /// Determines whether [is team request] [the specified user id].
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>
        ///   <c>true</c> if [is team request] [the specified user id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTeamRequest(int userId)
        {
            TeamMember member = this.TeamsMembersServiceRepository.Select(OptionsList)
                                .WithUserId(userId)
                                .FirstOrDefault();
            return member != null;
        }

        /// <summary>
        /// Selects the team member by team and user id.
        /// </summary>
        /// <param name="TeamId">The team id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public TeamMember SelectTeamMemberByTeamAndUserId(int TeamId, int userId)
        {
            return this.TeamsMembersServiceRepository.Select(OptionsList)
                    .WithUserId(userId)
                    .WithTeamId(TeamId)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Selects the team members by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<TeamMember> SelectTeamMembersByUserId(int userId)
        {
            IList<string> Includes = new List<string>() { "User" };
            return this.TeamsMembersServiceRepository.Select(Includes)
                                .WithUserId(userId)
                                .ToList();
        }

        /// <summary>
        /// Selects the team members.
        /// </summary>
        /// <param name="TeamId">The team id.</param>
        /// <returns></returns>
        public IList<TeamMember> SelectTeamMembers(int TeamId)
        {
            IList<string> Includes = new List<string>() { "User" };
            return this.TeamsMembersServiceRepository.Select(Includes)
                                .WithTeamId(TeamId)
                                .ToList();
        }

        //public IList<Person> SelectTeamRequestMembers(int TeamId)
        //{
        //    return this.TeamsMembersServiceRepository.SelectTeamsMembers(OptionsList)
        //                        .WithTeamId(TeamId)
        //                        .IsRequest()
        //                        .ToList();
        //}

        /// <summary>
        /// Selects my teams for teams service.
        /// </summary>
        /// <param name="userId">The people id.</param>
        /// <returns></returns>
        public IList<TeamMember> SelectMyTeamsForTeamsService(int userId)
        {
            return this.TeamsMembersServiceRepository
                .Select(OptionsList)
                .WithUserId(userId)
                .ToList();
        }
    }
}
