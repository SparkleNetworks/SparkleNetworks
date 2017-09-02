using System;
using System.Collections.Generic;
using System.Linq;
using Sparkle.Data.Filters;
using Sparkle.Data.Networks;
using Sparkle.Entities.Networks;
using Sparkle.Services.Networks;

namespace Sparkle.Services.Main.Networks
{
    public class TouchCommunicationsService : ServiceBase, ITouchCommunicationsService
    {
        internal TouchCommunicationsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected ITouchCommunicationsRepository touchCommunicationsRepository
        {
            get { return this.Repo.TouchCommunications; }
        }

        public int Insert(TouchCommunication item)
        {
            this.SetNetwork(item);

            return this.touchCommunicationsRepository.Insert(item).Id;
        }

        public TouchCommunication Update(TouchCommunication item)
        {
            this.VerifyNetwork(item);

            return this.touchCommunicationsRepository.Update(item);
        }

        public void Delete(TouchCommunication item)
        {
            this.VerifyNetwork(item);

            this.touchCommunicationsRepository.Delete(item);
        }

        public IList<TouchCommunication> SelectAll()
        {
            return this.touchCommunicationsRepository
                    .Select()
                .ByNetwork(this.Services.NetworkId)
                    .ToList();
        }

        public TouchCommunication GetById(int id)
        {
            return this.touchCommunicationsRepository
                    .Select()
                .ByNetwork(this.Services.NetworkId)
                    .ById(id)
                    .FirstOrDefault();
        }

        public TouchCommunication GetByDay(DateTime dateTime)
        {
            return this.touchCommunicationsRepository
                    .Select()
                .ByNetwork(this.Services.NetworkId)
                    .Where(o => o.Date == dateTime)
                    .FirstOrDefault();
        }
    }
}
