
namespace Sparkle.Services.Networks
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Events;

    public interface IEventsService
    {
        IList<string> OptionsList { get; set; }
        void DeleteEvent(Event item);
        IList<Event> GetWeekEvents(DateTime start);
        IList<Event> Get2NextWeekEvents();
        IList<Event> GetThisWeekEvents();
        int Insert(Event item);
        IList<Event> Search(string request);
        Event GetById(int id, EventOptions options);
        IList<Event> GetEventsForMainUserCalendar(int visibility);
        Event SelectFutureEventById(int Id);
        IList<Event> SelectFutureMettings(int companyId);
        IList<Event> GetUsersAcceptedEvents(int userId);
        IList<Event> SelectPrivateEvents();
        IList<Event> SelectPrivateEventsWithUserId(int userId);
        IList<Event> SelectPublicEvents();
        IList<Event> SelectPublicEventsWithUserId(int userId);

        [Obsolete]
        Event UpdateEvent(Event item);

        int CountEventByCreator(int userId);

        IList<Event> SelectNextEvents(int p, int count);

        IList<Event> SelectPastEvents(int p);

        IList<Event> GetByGroupId(int groupId);

        int Count();

        int CountCompaniesWithAtLeastOneEvent();

        IList<Event> GetDayAndFuture(EventVisibility eventScope, EventOptions options);

        IList<Event> SelectPublicAndNetworkEvents(EventOptions options);
        IList<Event> GetByPlaceId(int placeId);

        IList<TimeStatItem> GetStatsPerMonth();

        IList<Event> GetEvents(DateTime from, DateTime to, EventOptions options = EventOptions.None);

        Events.InviteToEventResult Invite(Events.InviteToEventRequest request);

        EventModel GetByModelId(int eventId, EventOptions options);

        IList<EventMemberModel> GetEventMembers(int id);
        IDictionary<int, IList<EventMemberModel>> GetEventMembers(int[] eventIds, EventMemberState[] withStates);

        DeleteEventResult Delete(DeleteEventRequest request);

        EditEventRequest GetCreateRequest(int? copyFromEventId, int userId, int? groupId, EditEventRequest request);

        EditEventResult Create(EditEventRequest model);

        IList<Event> GetGroupEventsWithUserId(int userId);

        EditEventRequest GetEditRequest(int eventId, int userId, EditEventRequest editEventModel);

        IList<Event> GetAllEventsRelatedToUser(int userId, DateTime? start = null, DateTime? end = null);
        IList<EventModel> GetAllEventsRelatedToUserWithModel(int userId, DateTime? start = null, DateTime? end = null);
        IList<EventModel> GetByGroupIdWithModel(int groupId, int userId);

        bool CanAdministrate(int eventId, int userId);

        string GetProfileUrl(Event item, UriKind uriKind);

        void SetupUserRights(EventModel model, int p);

        IDictionary<int, EventModel> GetModelById(int[] eventIds, EventOptions options);

        IList<EventModel> GetTopFutureEvents(int userId, int count, EventOptions options);
        IList<EventModel> GetTopFutureGroupEvents(int userId, int count, int groupId, EventOptions options);

        EditEventResult UpdateEvent(EditEventRequest model);

        int CountAtPlace(int placeId);

        IList<EventModel> GetRecentEventsAtPlace(int placeId, int userId, EventOptions options);

        int CountCreatedByUser(int userId);

        ValidateUserCalendarTokenResult ValidateUserCalendarToken(ValidateUserCalendarTokenRequest request);

        string GetUserCalendarToken(UserModel user);

        string GetUserCalendarToken(int userId);
    }
}
