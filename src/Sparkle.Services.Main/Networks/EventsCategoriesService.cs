
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Infrastructure;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class EventsCategoriesService : ServiceBase, IEventsCategoriesService
    {
        [DebuggerStepThrough]
        internal EventsCategoriesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IEventsCategoriesRepository eventsCategoriesRepository
        {
            get { return this.Repo.EventsCategories; }
        }

        public EventCategory UpdateEventCategory(EventCategory item)
        {
            return this.eventsCategoriesRepository.Update(item);
        }

        public int InsertEventCategory(EventCategory item)
        {
            return this.eventsCategoriesRepository.Insert(item).Id;
        }

        public void DeleteEventCategory(EventCategory item)
        {
            this.eventsCategoriesRepository.Delete(item);
        }

        public EventCategory SelectEventCategoryById(int Id)
        {
            return this.eventsCategoriesRepository.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .WithId(Id)
                .SingleOrDefault();
        }

        public IList<EventCategory> SelectAll()
        {
            return this.eventsCategoriesRepository.Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                      .ToList();
        }

        public int Count()
        {
            return this.eventsCategoriesRepository.Count();
        }

        public IList<EventCategoryModel> GetAll()
        {
            return this.eventsCategoriesRepository
                .Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .ToArray()
                .Select(c => new EventCategoryModel(c))
                .ToList();
        }

        public IList<EventCategoryModel> GetAllWithStats()
        {
            var items = this.eventsCategoriesRepository
                .Select()
                .ByNetworkOrCommon(this.Services.NetworkId)
                .Select(e => new
                {
                    EventType = e,
                    Events = e.Events.Count(),
                })
                .ToArray();
            return items
                .Select(c => new EventCategoryModel(c.EventType)
                {
                    EventsCount = c.Events,
                })
                .ToList();
        }

        public IDictionary<int, EventCategoryModel> GetAllForCache()
        {
            return this.Repo.EventsCategories.GetAll(this.Services.NetworkId).Values
                .ToDictionary(c => c.Id, c => new EventCategoryModel(c));
        }

        public IDictionary<int, EventCategoryModel> GetForCache(int[] ids)
        {
            return this.Repo.EventsCategories.GetById(ids).Values
                .ToDictionary(c => c.Id, c => new EventCategoryModel(c));
        }

        public void Initialize()
        {
            var all = this.Repo.EventsCategories.GetAll(this.Services.NetworkId);
            var names = new string[] { "Conférence", "Afterwork", "Sortie", "Salon", "Rencontre sportive", "Atelier", "Formation", };

            bool missingAlias = all.Values.Any(c => c.Alias == null);
            bool missingNames = false;
            foreach (var name in names)
            {
                if (!all.Values.Any(c => c.Name == name))
                {
                    missingNames = true;
                    break;
                }
            }

            if (!missingNames && !missingAlias)
            {
                this.Services.Logger.Verbose("EventsCategoriesService.Initialize", ErrorLevel.Success, "All categories OK.");
                return;
            }

            var transaction = this.Services.Repositories.NewTransaction();
            using (transaction.BeginTransaction())
            {
                all = transaction.Repositories.EventsCategories.GetAll(this.Services.NetworkId);
                for (int i = 0; i < names.Length; i++)
                {
                    if (!all.Values.Any(c => c.Name == names[i]))
                    {
                        var item = new EventCategory
                        {
                            Name = names[i],
                            NetworkId = this.Services.NetworkId,
                            Alias = names[i].MakeUrlFriendly(true),
                        };
                        transaction.Repositories.EventsCategories.Insert(item);
                        this.Services.Logger.Info("EventsCategoriesService.Initialize", ErrorLevel.Success, "Created event category " + item.ToString() + ".");
                    }
                }

                foreach (var item in all.Values)
                {
                    if (item.Alias == null)
                    {
                        item.Alias = item.Name.MakeUrlFriendly(true);
                    }
                }

                transaction.CompleteTransaction();
            }
        }
    }
}
