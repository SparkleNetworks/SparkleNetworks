
namespace Sparkle.Data.Filters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;

    public static class PeopleFilter
    {
        public static IQueryable<User> WithId(this IQueryable<User> qry, int userId)
        {
            return qry.Where(o => o.Id == userId);
        }

        public static IQueryable<User> WithIds(this IQueryable<User> qry, IEnumerable<int> userIds)
        {
            return qry.Where(o => userIds.Contains(o.Id));
        }

        public static IQueryable<User> WithGuid(this IQueryable<User> qry, Guid guid)
        {
            return qry.Where(o => o.UserId == guid);
        }

        public static IQueryable<User> WithLogin(this IQueryable<User> qry, string login)
        {
            return qry.Where(o => o.Login == login);
        }

        public static IQueryable<User> WithProMail(this IQueryable<User> qry, string mail)
        {
            return qry.Where(o => o.Email == mail);
        }

        public static IQueryable<User> WithFirstAndLastName(this IQueryable<User> qry, string firstname, string lastname)
        {
            return qry.Where(o => o.FirstName.ToLower() == firstname.ToLower() && o.LastName.ToLower() == lastname.ToLower());
        }

        public static IQueryable<User> WithoutTeam(this IQueryable<User> qry, int companyId)
        {
            return qry.Where(o => o.CompanyID == companyId && (o.IsTeamMember == null || o.IsTeamMember == false));
        }

        public static IQueryable<User> WithShortId(this IQueryable<User> qry, int shortId)
        {
            return qry.Where(o => o.Id == shortId);
        }

        public static IQueryable<User> WithCompanyId(this IQueryable<User> qry, long companyId)
        {
            return qry.Where(o => o.CompanyID == companyId);
        }

        public static IQueryable<UsersView> WithCompanyId(this IQueryable<UsersView> qry, long companyId)
        {
            return qry.Where(o => o.CompanyId == companyId);
        }

        public static IQueryable<User> WithGender(this IQueryable<User> qry, int gender)
        {
            return qry.Where(o => o.Gender == gender);
        }

        public static IQueryable<UsersView> ActiveAccount(this IQueryable<UsersView> qry)
        {
            return qry.Where(o =>
                o.CompanyAccessLevel > 0
             && o.NetworkAccessLevel > 0
             && o.IsEmailConfirmed
             && (!o.AccountClosed.Value || !o.AccountClosed.HasValue)
             && o.Company_IsEnabled);
        }

        public static IQueryable<User> ActiveAccount(this IQueryable<User> qry)
        {
            return qry.Where(o =>
                o.CompanyAccessLevel > 0
             && o.NetworkAccessLevel > 0
             && o.IsEmailConfirmed
             && (!o.AccountClosed.Value || !o.AccountClosed.HasValue)
             && o.Company.IsEnabled);
        }

        public static IEnumerable<User> ActiveAccount(this IEnumerable<User> qry)
        {
            return qry.Where(o =>
                o.CompanyAccessLevel > 0
             && o.NetworkAccessLevel > 0
             && o.IsEmailConfirmed
             && (o.AccountClosed == false || !o.AccountClosed.HasValue)
             && o.Company.IsEnabled);
        }

        public static IQueryable<User> WithJobId(this IQueryable<User> qry, long jobId)
        {
            return qry.Where(o => o.JobId == jobId);
        }

        public static IQueryable<User> Contain(this IQueryable<User> qry, string request)
        {
            return qry.Where(o =>
                o.FirstName.ToLower().Contains(request)
             || o.LastName.ToLower().Contains(request)
             || (o.FirstName.ToLower() + o.LastName.ToLower()).Contains(request)
             || (o.FirstName.ToLower() + " " + o.LastName.ToLower()).Contains(request) 
            );
        }

        public static IQueryable<Sparkle.Entities.Networks.Neutral.Person> LiteSelect(this IQueryable<UsersView> query)
        {
            return query
                .Select(q => new Sparkle.Entities.Networks.Neutral.Person
                {
                    Id = q.Id,
                    Username = q.Login,
                    FirstName = q.FirstName,
                    LastName = q.LastName,
                    ShortId = q.Id,
                    UserId = q.UserId,
                    CompanyId = q.CompanyId,
                    CompanyName = q.Company_Name,
                    CompanyAlias = q.Company_Alias,
                    JobId = q.JobId,
                    JobName = q.Job_Name,
                    JobAlias = q.JobAlias,
                    Email = q.Email,
                    Culture = q.Culture,
                    Timezone = q.Timezone,
                    PictureName = q.Picture,
                    IsEmailConfirmed = q.IsEmailConfirmed,
                    NetworkAccessLevel = q.NetworkAccessLevel,
                    CompanyAccessLevel = q.CompanyAccessLevel,
                    CompanyIsApproved = q.Company_IsApproved,
                    CompanyIsEnabled = q.Company_IsEnabled,
                });
        }

        public static IQueryable<User> MustBeValidatedByCompany(this IQueryable<User> query)
        {
            return query.Where(o => o.AccountClosed.HasValue && o.AccountClosed.Value && o.NetworkAccessLevel > 0);
        }

        public static IEnumerable<User> MustBeValidatedByCompany(this IEnumerable<User> query)
        {
            return query.Where(o => o.AccountClosed.HasValue && o.AccountClosed.Value && o.NetworkAccessLevel > 0);
        }

        public static IQueryable<User> AlreadyValidatedByCompany(this IQueryable<User> query)
        {
            return query.Where(o => (!o.AccountClosed.HasValue || o.AccountClosed.Value == false) && o.NetworkAccessLevel > 0);
        }

        public static IEnumerable<User> AlreadyValidatedByCompany(this IEnumerable<User> query)
        {
            return query.Where(o => (!o.AccountClosed.HasValue || o.AccountClosed.Value == false) && o.NetworkAccessLevel > 0);
        }

        public static IQueryable<User> WithoutUserRole(this IQueryable<User> query)
        {
            return query.Where(o => (NetworkAccessLevel)o.NetworkAccessLevel != NetworkAccessLevel.User);
        }

        public static IQueryable<User> WithoutDisabledRole(this IQueryable<User> query)
        {
            return query.Where(o => (NetworkAccessLevel)o.NetworkAccessLevel != NetworkAccessLevel.Disabled);
        }

        public static IQueryable<User> WithoutSparkleStaffRole(this IQueryable<User> query)
        {
            return query.Where(o => (NetworkAccessLevel)o.NetworkAccessLevel != NetworkAccessLevel.SparkleStaff);
        }
    }
}
