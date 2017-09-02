
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class PeoplesSkillsFilter
    {
        public static IQueryable<UserSkill> WithId(this IQueryable<UserSkill> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<UserSkill> WithUserId(this IQueryable<UserSkill> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<UserSkill> WithSkillId(this IQueryable<UserSkill> qry, int skillId)
        {
            return qry.Where(o => o.SkillId == skillId);
        }

        public static IQueryable<UserSkill> WithSkillIdAndUserId(this IQueryable<UserSkill> qry, int skillId, int userId)
        {
            return qry.Where(o => o.SkillId == skillId && o.UserId == userId);
        }
    }

    public static class CompaniesSkillsFilter
    {
        public static IQueryable<CompanySkill> WithId(this IQueryable<CompanySkill> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<CompanySkill> WithCompanyId(this IQueryable<CompanySkill> qry, int companyId)
        {
            return qry.Where(o => o.CompanyId == companyId);
        }

        public static IQueryable<CompanySkill> WithSkillIdAndCompanyId(this IQueryable<CompanySkill> qry, int skillId, int companyId)
        {
            return qry.Where(o => o.SkillId == skillId && o.CompanyId == companyId);
        }
        
    }

    public static class PeoplesInterestsFilter
    {
        public static IQueryable<UserInterest> WithId(this IQueryable<UserInterest> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<UserInterest> WithUserId(this IQueryable<UserInterest> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<UserInterest> WithInterestId(this IQueryable<UserInterest> qry, int interestId)
        {
            return qry.Where(o => o.InterestId == interestId);
        }

        public static IQueryable<UserInterest> WithInterestIdAndUserId(this IQueryable<UserInterest> qry, int interestId, int userId)
        {
            return qry.Where(o => o.InterestId == interestId && o.UserId == userId);
        }
    }

    public static class PeoplesRecreationsFilter
    {
        public static IQueryable<UserRecreation> WithId(this IQueryable<UserRecreation> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<UserRecreation> WithUserId(this IQueryable<UserRecreation> qry, int userId)
        {
            return qry.Where(o => o.UserId == userId);
        }

        public static IQueryable<UserRecreation> WithRecreationId(this IQueryable<UserRecreation> qry, int skillId)
        {
            return qry.Where(o => o.RecreationId == skillId);
        }

        public static IQueryable<UserRecreation> WithRecreationIdAndUserId(this IQueryable<UserRecreation> qry, int skillId, int userId)
        {
            return qry.Where(o => o.RecreationId == skillId && o.UserId == userId);
        }
    }
}
