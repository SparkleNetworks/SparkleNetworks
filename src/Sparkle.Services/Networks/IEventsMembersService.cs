
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;

    public interface IEventsMembersService
    {
        IList<string> OptionsList { get; set; }

        int AddEventMember(User me, EventMember item);
        EventMember Invite(int eventId, int userId, int inviterUserId);

        void DeleteRegistered(EventMember item);
        IList<User> GetAllState(int EventId);
        EventMember GetEventState(int eventId, int userId);
        IList<User> GetGuestList(int EventId, EventMemberState state);
        IList<User> GetRegisteredList(int EventId, bool Accepted);
        bool IsInvited(int EventId, int PersonId);
        EventMember SelectEventMemberByEventIdAndUserId(int EventId, int userId);
        IList<EventMember> SelectEventMembers(int EventId);
        IList<EventMember> SelectEventMembers(int EventId, EventMemberOptions options);
        IList<EventMember> SelectMyEventsForEventsService(int userId);
        IList<EventMember> SelectRegisteredByEventId(int EventId);
        IList<EventMember> SelectRegisteredByUserId(int userId);
        EventMember UpdateRegistered(EventMember item);

        bool IsAdmin(int userId, int eventId);

        bool IsEventMember(int userId, int eventId);

        int Count();
        int CountActualMembers();
        int CountInvitedMembers();

        IDictionary<int, EventMemberModel> GetMyMembershipForEvents(int[] eventIds, int userId);

        EventMemberModel GetMembership(int eventId, int userId);

        void ClearNotifications(int eventId, int userId);
    }
}
