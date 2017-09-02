
namespace Sparkle.Services.Main.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;

    public class TeamsService : ServiceBase, ITeamsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <param name="serviceFactory">The service factory.</param>
        internal TeamsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the teams repository.
        /// </summary>
        protected ITeamsRepository TeamsRepository
        {
            get { return this.Repo.Teams; }
        }

        /// <summary>
        /// Updates the team.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Team UpdateTeam(Team item)
        {
            return this.TeamsRepository.Update(item);
        }

        /// <summary>
        /// Creates the team.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int CreateTeam(Team item)
        {
            return this.TeamsRepository.Insert(item).Id;
        }

        /// <summary>
        /// Deletes the team.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeleteTeam(Team item)
        {
            this.TeamsRepository.Delete(item);
        }

        /// <summary>
        /// Selects the team by id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public Team SelectTeamById(int Id)
        {
            return this.TeamsRepository.Select()
                    .WithId(Id)
                    .SingleOrDefault();
        }

        /// <summary>
        /// Selects the teams.
        /// </summary>
        /// <returns></returns>
        public IList<Team> SelectTeams()
        {
            return this.TeamsRepository.Select()
                    .ToList();
        }

        /// <summary>
        /// Selects the by company id.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        public IList<Team> SelectByCompanyId(int companyId)
        {
            return this.TeamsRepository.Select()
                .WithCompanyId(companyId)
                .ToList();
        }

        public string GetProfileUrl(Team team, UriKind uriKind)
        {
            var path = "Teams/Team/" + team.Id;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }
    }
}
