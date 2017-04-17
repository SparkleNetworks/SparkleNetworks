
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;

    public interface IEventPublicMembersService
    {
        void Add(Entities.Networks.EventPublicMember member);

        Entities.Networks.EventPublicMember GetByEmail(string emailAddress);

        IList<Entities.Networks.EventPublicMember> GetByEventId(int eventId);

        int Count();
    }
}
