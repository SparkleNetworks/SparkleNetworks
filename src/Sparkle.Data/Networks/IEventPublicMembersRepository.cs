
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IEventPublicMembersRepository : IBaseNetworkRepository<EventPublicMember, int>
    {
        /// <summary>
        /// Gets the event public member by email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        EventPublicMember GetByEmail(string emailAddress);

        /// <summary>
        /// Gets events members by event id.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns></returns>
        IList<EventPublicMember> GetByEventId(int eventId);

        IDictionary<int, IList<EventPublicMember>> GetByEventId(int[] eventIds);

        IList<EventPublicMember> GetAll();

        int CountComing(int networkId);
    }
}
