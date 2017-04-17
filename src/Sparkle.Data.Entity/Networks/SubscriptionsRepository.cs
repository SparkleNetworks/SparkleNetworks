
namespace Sparkle.Data.Entity.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Filters;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Globalization;
    using System.Linq;
    using Sparkle.Data.Entity.Networks.Sql;

    public class SubscriptionsRepository : BaseNetworkRepositoryInt<Subscription>, ISubscriptionsRepository
    {
        [System.Diagnostics.DebuggerStepThrough]
        public SubscriptionsRepository(NetworksEntities context, Func<NetworksEntities> factory)
            : base(context, factory, m => m.Subscriptions)
        {
        }

        public Subscription Insert(Subscription item, int transactId)
        {
            if (this.Context.IsTransactionnal)
                throw new InvalidOperationException("SubscriptionsRepository.Insert: This cannot be used within a transaction.");

            using (var model = this.GetNewContext())
            {
                this.OnInsertOverride(model, item);

                var transactItem = model.StripeTransactions.Where(o => o.Id == transactId).Single();
                item.StripeTransactions.Add(transactItem);

                model.SaveChanges();
            }
            return item;
        }

        public IList<SubscriptionPoco> GetCurrent(int networkId, DateTime dateTime)
        {
            return this.Set
                .Where(s => s.NetworkId == networkId
                    && s.DateBeginUtc != null && s.DateBeginUtc.Value < dateTime
                    && (s.DateEndUtc == null || dateTime < s.DateEndUtc.Value))
                .Select(s => new SubscriptionPoco
                {
                    Id = s.Id,
                    NetworkId = s.NetworkId,
                    TemplateId = s.TemplateId,
                    OwnerCompanyId = s.OwnerCompanyId,
                    OwnerUserId = s.OwnerUserId,
                    AppliesToCompanyId = s.AppliesToCompanyId,
                    ////AppliesToCompany = s.AppliesToCompanyId != null ? new CompanyPoco
                    ////{
                    ////    ID = s.AppliesToCompany.ID,
                    ////    Name = s.AppliesToCompany.Name,
                    ////    Alias = s.AppliesToCompany.Alias,
                    ////    IsApproved = s.AppliesToCompany.IsApproved,
                    ////    IsEnabled = s.AppliesToCompany.IsEnabled,
                    ////} : default(CompanyPoco),
                    AppliesToUserId = s.AppliesToUserId,
                    AppliesToUser = new UserPoco
                    {
                        Id = s.AppliesToUserId != null ? s.AppliesToUser.Id : 0,
                        ////FirstName = s.AppliesToUser.FirstName,
                        ////LastName = s.AppliesToUser.LastName,
                        Login = s.AppliesToUserId != null ? s.AppliesToUser.Login : null,
                        Email = s.AppliesToUserId != null ? s.AppliesToUser.Email : null,
                        ////CompanyAccessLevel = s.AppliesToUser.CompanyAccessLevel,
                        ////NetworkAccessLevel = s.AppliesToUser.NetworkAccessLevel,
                        ////NetworkId = s.AppliesToUser.NetworkId,
                        ////IsEmailConfirmed = s.AppliesToUser.IsEmailConfirmed,
                        ////AccountClosed = s.AppliesToUser.AccountClosed,
                    },
                    AutoRenew = s.AutoRenew,
                    DateBeginUtc = s.DateBeginUtc,
                    DateCreatedUtc = s.DateCreatedUtc,
                    DateEndUtc = s.DateEndUtc,
                    DurationKind = s.DurationKind,
                    DurationValue = s.DurationValue,
                    IsForAllCompanyUsers = s.IsForAllCompanyUsers,
                    Name = s.Name,
                    PriceEurWithoutVat = s.PriceEurWithoutVat,
                    PriceEurWithVat = s.PriceEurWithVat,
                    PriceUsdWithoutVat = s.PriceUsdWithoutVat,
                    PriceUsdWithVat = s.PriceUsdWithVat,
                    IsPaid = s.IsPaid,
                })
                .ToList();
        }

        public DataPage<Subscription> GetList(int networkId, int[] userId, int[] templateId, bool? active, bool? past, Subscription.Columns sortColumn, bool sortAsc, int offset, int pageSize)
        {
            var query = this.GetListQuery(networkId, userId, templateId, active, past);
            query = query.OrderBy(sortColumn, sortAsc);
            query = query.Skip(offset).Take(pageSize);
            return new DataPage<Subscription>
            {
                Items = query.ToList(),
                Offset = offset,
                Size = pageSize,
            };
        }

        public int CountList(int networkId, int[] userId, int[] templateId, bool? active, bool? past)
        {
            var query = this.GetListQuery(networkId, userId, templateId, active, past);
            return query.Count();
        }

        private IQueryable<Subscription> GetListQuery(int networkId, int[] userId, int[] templateId, bool? active, bool? past)
        {
            var query = this.Set.AsQueryable().Where(s => s.NetworkId == networkId);
            var now = DateTime.UtcNow;

            if (userId != null)
            {
                query = query.Where(s => s.AppliesToUserId != null && userId.Contains(s.AppliesToUserId.Value));
            }

            if (templateId != null)
            {
                query = query.Where(s => templateId.Contains(s.TemplateId));
            }

            if (active == true)
            {
                query = query.Where(s => s.IsPaid
                    && (s.DateBeginUtc == null || s.DateBeginUtc.Value <= now)
                    && (s.DateEndUtc == null || now <= s.DateEndUtc.Value));
            }
            else if (active == false)
            {
                query = query.Where(s => !s.IsPaid
                    || (s.DateBeginUtc != null && now < s.DateBeginUtc.Value)
                    || (s.DateEndUtc != null && s.DateEndUtc.Value < now));
            }

            return query;
        }

        public void SubIsPaid(Subscription item)
        {
            item.IsPaid = true;

            this.Update(item);
        }

        public IQueryable<Subscription> NewQuery(SubscriptionOptions options)
        {
            ObjectQuery<Subscription> query = this.Set;

            if ((options & SubscriptionOptions.Notifications) == SubscriptionOptions.Notifications)
                query = query.Include("SubscriptionNotifications");

            if ((options & SubscriptionOptions.OwnerCompany) == SubscriptionOptions.OwnerCompany)
                query = query.Include("OwnerCompany");

            if ((options & SubscriptionOptions.OwnerUser) == SubscriptionOptions.OwnerUser)
                query = query.Include("OwnerUser");

            if ((options & SubscriptionOptions.AppliesToCompany) == SubscriptionOptions.AppliesToCompany)
                query = query.Include("AppliesToCompany");

            if ((options & SubscriptionOptions.AppliesToUser) == SubscriptionOptions.AppliesToUser)
                query = query.Include("AppliesToUser");

            return query;
        }

        public IList<Subscription> GetByIds(int[] ids, SubscriptionOptions options = SubscriptionOptions.None)
        {
            return this.NewQuery(options)
                .Where(o => ids.Contains(o.Id))
                .ToList();
        }

        public IList<Subscription> GetByApplyUserId(int applyUserId)
        {
            return this.Set
                .Where(o => o.IsPaid)
                .Where(o => o.AppliesToUserId == applyUserId)
                .ToList();
        }

        public IList<Subscription> GetCurrentAndFuture(int networkId, DateTime now)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId && o.DateEndUtc >= now)
                .ToList();
        }

        public IList<Subscription> GetAllActive(int networkId)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId)
                .Where(o => o.DateBeginUtc <= DateTime.UtcNow && o.DateEndUtc >= DateTime.UtcNow && o.IsPaid)
                .ToList();
        }

        public int[] GetUserIdsSubscribedAmongIds(int networkId, int[] applyingUserIds, DateTime now)
        {
            return this.Set
                .Where(o => o.NetworkId == networkId && o.AppliesToUserId.HasValue && applyingUserIds.Contains(o.AppliesToUserId.Value))
                .Where(o => o.IsPaid && o.DateBeginUtc.HasValue && o.DateBeginUtc <= now && (!o.DateEndUtc.HasValue || o.DateEndUtc >= now))
                .Select(o => o.AppliesToUserId.Value)
                .Distinct()
                .ToArray();
        }

        public Subscription GetFirstInTime()
        {
            return this.Set
                .Where(s => s.DateBeginUtc != null)
                .OrderBy(s => s.DateBeginUtc)
                .FirstOrDefault();
        }
        
        public Subscription GetLastInTime()
        {
            return this.Set
                .Where(s => s.DateEndUtc != null)
                .OrderByDescending(s => s.DateEndUtc)
                .FirstOrDefault();
        }

        public bool HasAnyUnlimited()
        {
            return this.Set
                .Where(s => s.DateBeginUtc != null && s.DateEndUtc == null)
                .Any();
        }

        public IDictionary<int, int> CountActiveSubscriptionsInDateRange(DateTime begin, DateTime end)
        {
            var cmd = ((EntityConnection)this.Context.Connection).StoreConnection.EnsureOpen().CreateCommand()
                .SetText("[dbo].[CountActiveSubscriptionsInDateRange]", true)
                .AddParameter("@begin", begin)
                .AddParameter("@end", end);
            var values = new Dictionary<int, int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(
                        reader.GetInt32(reader.GetOrdinal("TemplateId")),
                        reader.GetInt32(reader.GetOrdinal("SubscriptionCount")));
                }
            }

            return values;
        }

        public IList<Subscription> GetUsersAppliedSubscriptions(int[] userIds, DateTime date)
        {
            return this.Set
                .Where(o => o.IsPaid
                    && o.AppliesToUserId.HasValue
                    && userIds.Contains(o.AppliesToUserId.Value)
                    && o.DateBeginUtc <= date
                    && (o.DateEndUtc == null || o.DateEndUtc >= date))
                .ToList();
        }

        public int CountSubscribedPeople(int networkId)
        {
            var date=DateTime.UtcNow;
            return this.Set
                .Where(o => o.NetworkId == networkId
                    && o.IsPaid
                    && o.AppliesToUserId != null
                    && o.DateBeginUtc <= date && (o.DateEndUtc == null || date <= o.DateEndUtc))
                .GroupBy(o => o.AppliesToUserId)
                .Count();
        }

        public decimal GetSumOfAllSubscriptionAmounts(int networkId)
        {
            return this.Set
                .Where(s => s.NetworkId == networkId && s.IsPaid)
                .Select<Subscription, decimal?>(s => (s.PriceEurWithVat ?? 0M) + (s.PriceUsdWithVat ?? 0M))
                .Sum() ?? 0M;
        }

        public int CountByAppliedUser(int userId)
        {
            var items = this.Set
                .Where(s => s.AppliesToUserId == userId)
                .Count();
            return items;
        }

        public int CountDefaultSubscriptionsByAppliedUser(int userId)
        {
            var items = this.Set
                .Where(s => s.AppliesToUserId == userId)
                .Count();
            return items;
        }

        public int CountByAppliedUser(int userId, int templateId)
        {
            var items = this.Set
                .Where(s => s.AppliesToUserId == userId && s.TemplateId == templateId)
                .Count();
            return items;
        }
    }
}
