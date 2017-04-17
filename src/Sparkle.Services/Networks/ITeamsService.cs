
namespace Sparkle.Services.Networks
{
    using System;
    using Entities = Sparkle.Entities.Networks;

    public interface ITeamsService
    {
        int CreateTeam(Entities.Team item);
        void DeleteTeam(Entities.Team item);
        System.Collections.Generic.IList<Entities.Team> SelectByCompanyId(int companyId);
        Entities.Team SelectTeamById(int Id);
        System.Collections.Generic.IList<Entities.Team> SelectTeams();
        Entities.Team UpdateTeam(Entities.Team item);

        string GetProfileUrl(Entities.Team team, UriKind uriKind);
    }
}
