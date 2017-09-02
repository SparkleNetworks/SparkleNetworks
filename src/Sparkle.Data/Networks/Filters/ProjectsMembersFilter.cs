
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class ProjectMemberFilter
    {
        public static IQueryable<ProjectMember> WithProjectId(this IQueryable<ProjectMember> query, int ProjectId)
        {
            return query.Where(o => o.ProjectId == ProjectId);
        }

        public static IQueryable<ProjectMember> WithUserId(this IQueryable<ProjectMember> query, int peopleId)
        {
            return query.Where(o => o.UserId == peopleId);
        }
    }
}
