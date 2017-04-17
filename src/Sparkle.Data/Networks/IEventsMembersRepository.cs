
namespace Sparkle.Data.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sparkle.Entities.Networks;

    [Repository]
    public interface IEventsMembersRepository : IBaseNetworkRepository<EventMember>
    {
        IQueryable<EventMember> SelectRegistered(IList<string> options);
        IQueryable<EventMember> NewQuery(EventMemberOptions options);

        IQueryable<User> GetGuestList(int eventId, EventMemberState state);
        IQueryable<User> GetGuestList(int eventId);

        IQueryable<User> GetRegisteredList(int eventId, bool accepted);

        ////int InsertRegistered(EventMember item);

        EventMember UpdateRegistered(EventMember item);

        void DeleteRegistered(EventMember item);

        IList<EventMember> GetById(int[] userIds, int eventId);
        EventMember GetById(int userId, int eventId);

        IList<EventMember> GetByEventId(int id, EventMemberOptions options);

        IDictionary<int, IList<EventMember>> GetByEventId(int[] eventIds, EventMemberOptions options);
        IDictionary<int, IList<EventMember>> GetByEventId(int[] eventIds, EventMemberOptions options, bool comingOnly);

        EventMember GetMembership(int eventId, int userId);

        int CountAll(int networkId);
        int CountActualMembers(int networkId);
        int CountInvitedMembers(int networkId);
    }
}