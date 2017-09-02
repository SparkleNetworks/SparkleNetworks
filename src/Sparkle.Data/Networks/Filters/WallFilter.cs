
namespace Sparkle.Data.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    public static class TimelineItemFilter
    {
        public static IQueryable<TimelineItem> WithPublicationId(this IQueryable<TimelineItem> qry, int publicationId)
        {
            return qry.Where(o => o.Id == publicationId);
        }

        // Vérifie si une publication de mise à jour du profil d'une societe a déjà été faite ce jour
        public static IQueryable<TimelineItem> CompanyProfileUpdated(this IQueryable<TimelineItem> qry, int companyId)
        {
            string today = DateTime.Now.Date.ToString();
            return qry.Where(o => o.ItemType == 7 && o.CompanyId == companyId && o.Text == today);
        }
        // Vérifie si une publication de mise à jour du profil d'une personne a déjà été faite ce jour
        public static IQueryable<TimelineItem> PeopleProfileUpdated(this IQueryable<TimelineItem> qry, int profileId)
        {
            string today = DateTime.Now.Date.ToString();
            string today2 = today + " 00:00:00";
            return qry.Where(o => o.ItemType == 12 && o.UserId == profileId && (o.Text == today || o.Text == today2));
        }

        public static IQueryable<TimelineItem> ExcludeDeleted(this IQueryable<TimelineItem> qry, bool showDeleted)
        {
            return qry.Where(i => showDeleted || i.DeleteReason == null);
        }

        public static IQueryable<TimelineItem> WithUserId(this IQueryable<TimelineItem> qry, int guid)
        {
            return qry.Where(o => o.PostedByUserId == guid);
        }

        public static IQueryable<TimelineItem> WithId(this IQueryable<TimelineItem> qry, int id)
        {
            return qry.Where(o => o.Id == id);
        }

        public static IQueryable<TimelineItem> WithItemType(this IQueryable<TimelineItem> qry, int itemType)
        {
            return qry.Where(o => o.ItemType == itemType);
        }

        public static IQueryable<TimelineItem> NewRegistrants(this IQueryable<TimelineItem> qry)
        {
            DateTime dateTemp = DateTime.Now.Date.AddDays(-1);
            while (dateTemp.DayOfWeek != DayOfWeek.Monday)
            {
                dateTemp = dateTemp.AddDays(-1);
            }

            return qry.Where(o =>  o.ItemType == 1 && o.CreateDate > dateTemp);
        }

        public static IQueryable<TimelineItem> LastRegistrants(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.ItemType == 1);
        }

        public static IQueryable<TimelineItem> WithProfileId(this IQueryable<TimelineItem> qry, int profileId)
        {
            return qry.Where(o => o.UserId == profileId);
        }

        public static IQueryable<TimelineItem> WithProfileIdOrNull(this IQueryable<TimelineItem> qry, int profileId)
        {
            return qry.Where(o => o.UserId == profileId || o.UserId == 0);
        }

        public static IQueryable<TimelineItem> WithCompanyId(this IQueryable<TimelineItem> qry, int companyId)
        {
            return qry.Where(o => o.CompanyId == companyId);
        }

        public static IQueryable<TimelineItem> WithEventId(this IQueryable<TimelineItem> qry, int eventId)
        {
            return qry.Where(o => o.EventId == eventId);
        }

        public static IQueryable<TimelineItem> WeekCompaniesPublications(this IQueryable<TimelineItem> qry, DateTime start)
        {
            return qry.Where(o => o.ItemType == 0 && o.PrivateMode <= 0 && o.CompanyId > 0 && o.CreateDate > start);
        }

        public static IQueryable<TimelineItem> WeekCompaniesPublicationsForDevices(this IQueryable<TimelineItem> qry, DateTime start)
        {
            return qry.Where(o => o.ItemType == 0 && o.PrivateMode == -2 && o.CompanyId > 0 && o.CreateDate > start);
        }

        public static IQueryable<TimelineItem> External(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.PrivateMode == -1);
        }

        public static IQueryable<TimelineItem> WithGroupId(this IQueryable<TimelineItem> qry, int groupId)
        {
            return qry.Where(o => o.GroupId == groupId);
        }

        public static IQueryable<TimelineItem> WithPlaceId(this IQueryable<TimelineItem> qry, int placeId)
        {
            return qry.Where(o => o.PlaceId == placeId);
        }

        public static IQueryable<TimelineItem> WithProjectId(this IQueryable<TimelineItem> qry, int projectId)
        {
            return qry.Where(o => o.ProjectId == projectId);
        }

        public static IQueryable<TimelineItem> WithTeamId(this IQueryable<TimelineItem> qry, int teamId)
        {
            return qry.Where(o => o.TeamId == teamId);
        }

        // Utilisé pour différencier les timelines réseau de l'entreprise et timeline publique de l'entreprise
        public static IQueryable<TimelineItem> PublicMode(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.PrivateMode <= 0);
        }
        public static IQueryable<TimelineItem> PrivateMode(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.PrivateMode == 1);
        }

        public static IQueryable<TimelineItem> Network(this IQueryable<TimelineItem> qry, IList<int> contacts)
        {
            // News : (de mes contacts OU publiques) ET n'étant pas du réseau
            return qry.Where(o => (o.PrivateMode <= 0 && o.CompanyId == null) || (o.CompanyId > 0 && o.ItemType > 0) || (contacts.Contains(o.PostedByUserId) && o.CompanyId == null && o.PlaceId == null && o.TeamId == null && o.ProjectId == null));
        }

        public static IQueryable<TimelineItem> SponsoredNetwork(this IQueryable<TimelineItem> qry)
        {
            DateTime dateTemp = DateTime.Now.Date.AddDays(-7D);
            return qry.Where(o => o.CreateDate > dateTemp && o.PrivateMode <= 0 && o.CompanyId > 0 && o.ItemType <= 0);
        }

        public static IQueryable<TimelineItem> CompaniesNews(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.PrivateMode <= 0 && o.CompanyId > 0 && o.ItemType == 0);
        }

        public static IQueryable<TimelineItem> PeopleNews(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.CompanyId == null);
        }

        public static IQueryable<TimelineItem> ExternalCompanyNews(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.PrivateMode < 0 && o.CompanyId > 0);
        }

        public static IQueryable<TimelineItem> Range(this IQueryable<TimelineItem> qry, int take, int skip, int firstItem)
        {
            if (firstItem > 0)
                return qry.Where(o => o.Id > firstItem);

            return qry.Skip(skip).Take(take);
        }

        public static IQueryable<TimelineItem> WithoutRelations(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.ItemType != 2);
        }

        public static IQueryable<TimelineItem> WithoutJoinNetwork(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.ItemType != 1);
        }

        public static IQueryable<TimelineItem> Relations(this IQueryable<TimelineItem> qry)
        {
            return qry.Where(o => o.ItemType == 2);
        }

        public static IQueryable<TimelineItem> IsContact(this IQueryable<TimelineItem> qry, IList<int> contacts)
        {
            return qry.Where(o => contacts.Contains(o.PostedByUserId));
        }

        public static IQueryable<TimelineItem> IsContact(this IQueryable<TimelineItem> qry, bool isContact, int userId, int profileId)
        {
            if (isContact)
                return qry.Where(o => (o.PrivateMode == 1 || o.PrivateMode <= 0) && (o.UserId == profileId || (o.PostedByUserId == userId && o.UserId == null)) && o.CompanyId == null && o.TeamId == null && o.ProjectId == null);

            return qry.Where(o => (o.PrivateMode <= 0) && (o.UserId == profileId || (o.PostedByUserId == userId && o.UserId == null)) && o.CompanyId == null && o.TeamId == null && o.ProjectId == null);
        }

        public static IQueryable<TimelineItem> FullAccess(this IQueryable<TimelineItem> qry, int userId, int profileId)
        {
            return qry.Where(o => (o.PrivateMode == 1 || o.PrivateMode <= 0) && (o.UserId == profileId || (o.PostedByUserId == userId && o.UserId == null)) && o.TeamId == null && o.ProjectId == null);
        }

        public static IEnumerable<TimelineItem> GetLike(this IEnumerable<TimelineItem> walls, int? CurrentId)
        {
            foreach (var o in walls)
            {
                o.IsLikeByCurrentId = o.Likes
                                        .Where(i => i.IsLiked != null && i.IsLiked == true)
                                        .Any(n => n.UserId == CurrentId);
                yield return o;
            }
        }

        public static TimelineItem GetLike(this TimelineItem wall, int? CurrentId)
        {
            if (CurrentId.HasValue && wall != null)
            {
                wall.IsLikeByCurrentId = wall.Likes != null && wall.Likes
                                                                    .Where(o => o.IsLiked != null && o.IsLiked == true)
                                                                    .Any(o => o.UserId == CurrentId);
            }
            return wall;
        }
    }
}