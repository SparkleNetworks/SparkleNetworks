using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparkle.Data;
using Sparkle.Data.Filters;
using Sparkle.Data.Networks;
using Sparkle.Entities;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    public class TouchCommunicationItemsService : ServiceBase, ITouchCommunicationItemsService
    {
        internal TouchCommunicationItemsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ITouchCommunicationItemsRepository touchCommunicationItemsRepository
        {
            get { return this.Repo.TouchCommunicationItems; }
        }

        public int Insert(TouchCommunicationItem item)
        {
            return this.touchCommunicationItemsRepository.Insert(item).Id;
        }

        public TouchCommunicationItem Update(TouchCommunicationItem item)
        {
            return this.touchCommunicationItemsRepository.Update(item);
        }

        public void Delete(TouchCommunicationItem item)
        {
            this.touchCommunicationItemsRepository.Delete(item);
        }

        public IList<TouchCommunicationItem> SelectAll()
        {
            return this.touchCommunicationItemsRepository
                    .Select()
                    .ToList();
        }

        public TouchCommunicationItem GetById(int id)
        {
            return this.touchCommunicationItemsRepository
                    .Select()
                    .ById(id)
                    .FirstOrDefault();
        }

        public IList<TouchCommunicationItem> GetByParentId(int parentId)
        {
            return this.touchCommunicationItemsRepository
                    .Select()
                    .ByParentId(parentId)
                    .ToList();
        }
    }
}
