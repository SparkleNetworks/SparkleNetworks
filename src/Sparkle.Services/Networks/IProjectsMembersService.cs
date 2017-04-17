using System.Collections.Generic;
using Sparkle.Entities.Networks;

namespace Sparkle.Services.Networks
{
    public interface IProjectsMembersService
    {
        int AddProjectMember(ProjectMember item);
        void DeleteProjectMember(ProjectMember item);
        IList<string> OptionsList { get; set; }
        IList<ProjectMember> SelectProjectsMembersByProjectId(int projectId);
        IList<ProjectMember> SelectProjectsMembersByUserId(int userId);
        ProjectMember SelectProjectsMembersByProjectIdAndUserId(int projectId, int userId);
        ProjectMember UpdateProjectMember(ProjectMember item);
    }
}
