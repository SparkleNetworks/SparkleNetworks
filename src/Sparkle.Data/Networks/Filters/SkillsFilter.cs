
namespace Sparkle.Data.Filters
{
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class SkillFilter
    {
        public static IQueryable<Skill> Contain(
        this IQueryable<Skill> qry, string request)
        {
            return qry.Where(o =>
                o.TagName.ToLower().Contains(request.ToLower())
                );
        }

        public static IQueryable<Skill> ChildOf(
        this IQueryable<Skill> qry, int parentId)
        {
            return qry.Where(o => o.ParentId == parentId);
        }

        public static IQueryable<Skill> ById(
        this IQueryable<Skill> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }
        public static IQueryable<Skill> ByName(
        this IQueryable<Skill> qry, string name)
        {
            return qry.Where(o => o.TagName.ToLower() == name.ToLower());
        }
    }

    public static class InterestFilter
    {
        public static IQueryable<Interest> Contain(
        this IQueryable<Interest> qry, string request)
        {
            return qry.Where(o =>
                o.TagName.ToLower().Contains(request.ToLower())
                );
        }

        public static IQueryable<Interest> ChildOf(
        this IQueryable<Interest> qry, int parentId)
        {
            return qry.Where(o => o.ParentId == parentId);
        }

        public static IQueryable<Interest> ById(
        this IQueryable<Interest> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Interest> ByName(
        this IQueryable<Interest> qry, string name)
        {
            return qry.Where(o => o.TagName.ToLower() == name.ToLower());
        }
    }

    public static class RecreationFilter
    {
        public static IQueryable<Recreation> Contain(
        this IQueryable<Recreation> qry, string request)
        {
            return qry.Where(o =>
                o.TagName.ToLower().Contains(request.ToLower())
                );
        }

        public static IQueryable<Recreation> ChildOf(
        this IQueryable<Recreation> qry, int parentId)
        {
            return qry.Where(o => o.ParentId == parentId);
        }

        public static IQueryable<Recreation> ById(
        this IQueryable<Recreation> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<Recreation> ByName(
       this IQueryable<Recreation> qry, string name)
        {
            return qry.Where(o => o.TagName.ToLower() == name.ToLower());
        }
    }
}
