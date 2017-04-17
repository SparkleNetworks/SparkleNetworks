
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface ITeamsMembersService
    {
        TeamMember AddTeamMember(TeamMember item);
        void DeleteTeamMember(TeamMember item);
        bool IsTeamRequest(int userId);
        IList<TeamMember> SelectMyTeamsForTeamsService(int peopleId);
        TeamMember SelectTeamMemberByTeamAndUserId(int TeamId, int userId);
        IList<TeamMember> SelectTeamMembers(int TeamId);
        IList<TeamMember> SelectTeamMembersByUserId(int userId);
        TeamMember UpdateTeamMember(TeamMember item);
    }
}
