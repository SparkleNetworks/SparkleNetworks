
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public interface IProjectsService
    {
        void DeleteProject(Project item);
        int InsertProject(Project item);
        IList<Project> SelectByCompanyId(int companyId);
        IList<Project> SelectProjects(int visibility);
        Project SelectProjectById(int id);
        Project UpdateProject(Project item);

        string GetProfileUrl(Project project, UriKind uriKind);
    }
}
