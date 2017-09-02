
namespace Sparkle.Data.Networks.Filters
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class SubscriptionFilters
    {
        public static IQueryable<Subscription> OrderBy(this IQueryable<Subscription> query, Subscription.Columns column, bool sortAsc)
        {
            switch (column)
            {
                case Subscription.Columns.Id:
                    return sortAsc
                        ? query.OrderBy(s => s.Id)
                        : query.OrderByDescending(s => s.Id);

                case Subscription.Columns.TemplateId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.DateCreatedUtc:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.OwnerUserId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.OwnerCompanyId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.AppliesToUserId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.AppliesToCompanyId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.DateBeginUtc:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.DateEndUtc:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.AutoRenew:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.DurationValue:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.DurationKind:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.PriceUsdWithoutVat:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.PriceUsdWithVat:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.PriceEurWithoutVat:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.PriceEurWithVat:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.Name:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.IsForAllCompanyUsers:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                case Subscription.Columns.NetworkId:
                    return sortAsc
                        ? query.OrderBy(s => s.TemplateId)
                        : query.OrderByDescending(s => s.TemplateId);

                default:
                    return query;
            }
        }
    }
}
