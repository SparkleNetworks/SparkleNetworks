
namespace Sparkle.Services.Main.Networks
{
    using System;
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class EventPublicMembersService : ServiceBase, IEventPublicMembersService
    {
        [DebuggerStepThrough]
        internal EventPublicMembersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        public void Add(EventPublicMember member)
        {
            this.Repo.EventPublicMembers.Insert(member);
        }

        public EventPublicMember GetByEmail(string emailAddress)
        {
            return this.Repo.EventPublicMembers.GetByEmail(emailAddress);
        }

        public IList<EventPublicMember> GetByEventId(int eventId)
        {
            return this.Repo.EventPublicMembers.GetByEventId(eventId).ToList();
        }

        public int Count()
        {
            return this.Repo.EventPublicMembers.CountComing(this.Services.NetworkId);
        }
    }
}
