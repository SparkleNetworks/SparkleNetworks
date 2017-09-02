
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Events;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Cryptography;

    public class EventsService : ServiceBase, IEventsService
    {
        private const string StoreEventDatesTimezone = "Romance Standard Time";

        [DebuggerStepThrough]
        internal EventsService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IEventsRepository eventsRepository
        {
            get { return this.Repo.Events; }
        }

        public Event UpdateEvent(Event item)
        {
            this.VerifyNetwork(item);

            return this.eventsRepository.Update(item);
        }

        public int CountEventByCreator(int userId)
        {
            return this.eventsRepository.Select()
                .ByNetwork(this.Services.NetworkId)
                .Active()
           .ByCreator(userId)
           .Count();
        }

        public int Insert(Event item)
        {
            this.SetNetwork(item);

            return this.eventsRepository.Insert(item).Id;
        }

        public void DeleteEvent(Event item)
        {
            this.VerifyNetwork(item);

            var wallItems  = this.Services.Wall.SelectFromEvent(item.Id);
            foreach (var it in wallItems)
            {
                this.Services.Wall.Delete(it.Id, WallItemDeleteReason.LinkedEntityDelete, item.DeletedByUserId ?? 1);
            }

            this.eventsRepository.Update(item);
        }

        public DeleteEventResult Delete(DeleteEventRequest request)
        {
            var result = new DeleteEventResult(request);

            var item = this.Repo.Events.GetById(request.EventId);
            if (item == null || item.NetworkId != this.Services.NetworkId)
            {
                result.Errors.Add(DeleteEventError.NoSuchItem, NetworksEnumMessages.ResourceManager);
                return result;
            }

            this.VerifyNetwork(item);

            var user = this.Repo.People.GetActiveById(request.UserId, PersonOptions.None);
            if (user == null)
            {
                result.Errors.Add(DeleteEventError.NoSuchUserOrInactive, NetworksEnumMessages.ResourceManager);
                return result;
            }

            if (item.CreatedByUserId == user.Id ||
                user.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff) ||
                (item.GroupId.HasValue && this.Services.GroupsMembers.IsGroupMember(user.Id, item.GroupId.Value)) ||
                item.CompanyId != null && user.CompanyID == item.CompanyId && (user.CompanyAccess == CompanyAccessLevel.CommunityManager || user.CompanyAccess == CompanyAccessLevel.Administrator))
            { }
            else
            {
                result.Errors.Add(DeleteEventError.NotAuthorized, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
                return result;

            item.DeleteDateUtc = DateTime.UtcNow;
            item.DeletedByUserId = request.UserId;
            item.DeleteReasonValue = request.Reason;
            
            var wallItems = this.Services.Wall.SelectFromEvent(item.Id);
            foreach (var it in wallItems)
            {
                this.Services.Wall.Delete(it.Id, WallItemDeleteReason.LinkedEntityDelete, request.UserId);
            }

            this.eventsRepository.Update(item);

            result.Succeed = true;
            return result;
        }

        public Event GetById(int id, EventOptions options)
        {
            return this.eventsRepository
                .CreateQuery(options)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .WithId(id)
                .SingleOrDefault();
        }

        public Event SelectFutureEventById(int Id)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                        .TodayAndFuture(now)
                        .WithId(Id)
                        .SingleOrDefault();
        }

        public IList<Event> SelectFutureMettings(int companyId)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                        .TodayAndFuture(now)
                        .MeetingFromCompany(companyId)
                        .ToList();
        }

        public IList<Event> GetEventsForMainUserCalendar(int visibility)
        {
            var now = DateTime.Now.Date.AddMonths(-1);
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .TodayAndFuture(now)
                .WithVisibility(visibility)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> SelectNextEvents(int visibility, int count)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .WithVisibilityEqualOrLess(visibility)
                .OrderBy(o => o.DateEvent)
                .Take(count)
                .ToList();
        }

        public IList<Event> SelectPastEvents(int visibility)
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .Past()
                .WithVisibility(visibility)
                .OrderByDescending(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetByGroupId(int groupId)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .ByGroupId(groupId)
                .TodayAndFuture(now)
                .ToList();
        }

        public int Count()
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                        .Count();
        }

        public int CountCompaniesWithAtLeastOneEvent()
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .GroupBy(e => e.CompanyId)
                .Count();
        }

        public IList<Event> SelectPublicEvents()
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .WithVisibility(-1)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> SelectPrivateEvents()
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository
                .Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .ToList();
        }

        public IList<Event> SelectPublicEventsWithUserId(int UserId)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .PostedBy(UserId)
                .ToList();
        }

        public IList<Event> GetUsersAcceptedEvents(int userId)
        {
            // TODO: this query is writen with a high cost (many items transfered from SQL to the app). Re-write it or make a stored procedure or a view.
            IList<EventMember> eventsMembers = this.Services.EventsMembers.SelectMyEventsForEventsService(userId);

#warning performance hit here
            IList<Event> allEvents = eventsMembers
                .Where(e => e.State == (int)EventMemberState.HasAccepted)
                .Select(eventMember => GetById(eventMember.EventId, EventOptions.None))
                .Where(e => e != null)
                .Active()
                .OrderBy(o => o.DateEvent)
                .ToList();
            return allEvents
                .Where(e => e.DateEvent > DateTime.Now || e.DateEvent < DateTime.Now && DateTime.UtcNow < e.DateEndEvent)
                .ToList();
        }

        public IList<Event> SelectPrivateEventsWithUserId(int userId)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .PostedBy(userId)
                .ToList();
        }

        public IList<Event> Search(string request)
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                            .Contain(request)
                            .Take(5)
                            .ToList();
        }

        public IList<Event> Get2NextWeekEvents()
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .AfterNextWeek()
                .WithVisibility(0)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetThisWeekEvents()
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .ThisWeek()
                .WithVisibility(0)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetWeekEvents(DateTime start)
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .NextWeek(start)
                .Where(e => e.Visibility <= 0)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetDayAndFuture(EventVisibility eventScope, EventOptions options)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository
                .CreateQuery(options)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .WithVisibility(eventScope)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> SelectPublicAndNetworkEvents(EventOptions options)
        {
            var now = DateTime.Now.Date;
            return this.eventsRepository.CreateQuery(options)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .TodayAndFuture(now)
                .Where(e => e.Visibility == -1 || e.Visibility == 0)
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetByPlaceId(int placeId)
        {
            return this.eventsRepository.Select(OptionsList)
                .ByNetwork(this.Services.NetworkId)
                .Active()
                .ByPlaceId(placeId)
                .ToList();
        }

        public IList<TimeStatItem> GetStatsPerMonth()
        {
            return this.Repo.Events.GetStatsPerMonth(this.Services.NetworkId);
        }

        public IList<Event> GetEvents(DateTime from, DateTime to, EventOptions options = EventOptions.None)
        {
            return this.eventsRepository
                .CreateQuery(options)
                .Active()
                .Where(e => e.NetworkId == this.Services.NetworkId
                        && (from < e.DateEvent && e.DateEvent < to
                         || from < e.DateEndEvent && e.DateEndEvent < to))
                .OrderBy(e => e.DateEvent)
                ////.Select(e => new EventModel(e))
                .ToList();
        }

        public InviteToEventResult Invite(InviteToEventRequest request)
        {
            const string methodName = "Invite";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new InviteToEventResult(request);

            var inviter = this.Services.People.GetActiveLiteById(new int[] { request.FromUserId, }, u => new UserPoco
            {
                Id = u.Id,
                Login = u.Login,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CompanyID = u.CompanyID,
            }).SingleOrDefault();

            if (inviter == null)
                result.Errors.Add(InviteToEventCode.NoSuchInviter, NetworksEnumMessages.AddGroupTagError_AlreadyAdded);

            var evt = this.Services.Events.GetByModelId(request.EventId, EventOptions.Category);
            if (evt == null)
            {
                result.Errors.Add(InviteToEventCode.NoSuchEvent, NetworksEnumMessages.ResourceManager);
            }

            int? companyId = null;
            if (evt != null && !this.Services.EventsMembers.IsAdmin(request.FromUserId, evt.Id))
            {
                switch (evt.Visibility)
                {
                    case EventVisibility.External:
                    case EventVisibility.Public:
                    case EventVisibility.Company:
                        break;

                    case EventVisibility.CompanyPrivate:
                        companyId = evt.CompanyId.Value;
                        if (inviter.CompanyID != companyId)
                            result.Errors.Add(InviteToEventCode.InviterIsNotAuthorized, NetworksEnumMessages.ResourceManager);
                        break;

                    case EventVisibility.Personal:
                        if (inviter.Id != evt.CreatedByUserId)
                            result.Errors.Add(InviteToEventCode.InviterIsNotAuthorized, NetworksEnumMessages.ResourceManager);
                        break;

                    default:
                        result.Errors.Add(InviteToEventCode.UnknownEventVisibility, NetworksEnumMessages.ResourceManager);
                        break;
                }
            }

            if (evt != null && evt.IsPast)
            {
                result.Errors.Add(InviteToEventCode.EventIsInPast, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, methodName, "ActingUserId:" + request.FromUserId);
            }

            var activeUsers = this.Repo.People.GetActiveLiteById(request.UserIds.ToArray(), this.Services.NetworkId)
                .ToDictionary(u => u.Id, u => u);
            ////var members = this.Repo.EventsMembers.GetById(activeUsers.Keys.ToArray(), request.EventId)
            ////    .ToDictionary(m => m.UserId, m => m);

            foreach (var userId in request.UserIds)
            {
                var userResult = new Services.Networks.Events.InviteToEventResult.UserResult();
                if (activeUsers.ContainsKey(userId))
                {
                    bool isOk = true;
                    userResult.User = new UserModel(activeUsers[userId]);
                    var member = this.Repo.EventsMembers.GetById(userId, evt.Id);

                    if (member != null)
                    {
                        userResult.PreviousState = member.StateValue;
                        userResult.Error = new ResultError<InviteToEventResult.UserCode>(InviteToEventResult.UserCode.AlreadyMember, NetworksEnumMessages.ResourceManager);
                    }
                    else
                    {
                        if (companyId != null)
                        {
                            if (activeUsers[userId].CompanyId != companyId.Value)
                            {
                                userResult.Error = new ResultError<InviteToEventResult.UserCode>(InviteToEventResult.UserCode.InviteeIsNotAuthorized, NetworksEnumMessages.ResourceManager);
                                isOk = false;
                            }
                        }

                        if (isOk)
                        {
                            try
                            {
                                var eventMember = this.Services.EventsMembers.Invite(evt.Id, userId, inviter.Id);
                                userResult.Succeed = true;
                            }
                            catch (SparkleServicesException ex)
                            {
                                userResult.Succeed = false;
                                userResult.Error = new ResultError<InviteToEventResult.UserCode>(
                                    InviteToEventResult.UserCode.Error,
                                    ex.DisplayMessage ?? SrkToolkit.EnumTools.GetDescription<InviteToEventResult.UserCode>(InviteToEventResult.UserCode.Error, NetworksEnumMessages.ResourceManager));
                                userResult.Error.Detail = ex.Message;
                            }
                        }
                    }

                    if (isOk)
                    {
                        var message = "Vous avez été invité à " + evt.Name + " par " + inviter.FirstName + " " + inviter.LastName;
                        this.Services.Activities.Create(userId, ActivityType.EventInvitation, message: message, profileId: inviter.Id, eventId: evt.Id);
                    }
                }
                else
                {
                    userResult.User = new UserModel(userId);
                    userResult.Error = new ResultError<InviteToEventResult.UserCode>(InviteToEventResult.UserCode.NoSuchUser, NetworksEnumMessages.ResourceManager);
                }

                result.Results.Add(userResult);
            }

            if (result.Results.Any(r => r.Succeed))
            {
                result.Succeed = true;

                if (result.Results.Any(r => !r.Succeed))
                {
                    // TODO: display errors to user
                }
            }
            else if (result.Results.Count == 0)
            {
                result.Errors.Add(InviteToEventCode.NobodyToInvite, NetworksEnumMessages.ResourceManager);
            }
            else
            {
                var errors = result.Results
                    .Where(x => x.Error != null)
                    .Select(x => (x.User.DisplayName ?? x.User.Id.ToString()) + " " + x.Error.DisplayMessage ?? x.Error.Code.ToString())
                    .ToArray();
                var message = SrkToolkit.EnumTools.GetDescription<InviteToEventCode>(InviteToEventCode.MultipleErrors, NetworksEnumMessages.ResourceManager) 
                    + " " + Environment.NewLine
                    + string.Join(" " + Environment.NewLine, errors);
                result.Errors.Add(InviteToEventCode.MultipleErrors, message);
            }

            return this.LogResult(result, methodName, "ActingUserId:" + request.FromUserId);
        }

        public EventModel GetByModelId(int eventId, EventOptions options)
        {
            var item = this.Repo.Events.CreateQuery(options)
                .SingleOrDefault(e => e.Id == eventId);

            if (item == null)
                return null;

            return new EventModel(item);
        }

        private DateTime GetNextDateByDayOfWeek(DateTime date, DayOfWeek day)
        {
            while (date.DayOfWeek != day)
            {
                date = date.AddDays(1D);
            }

            return date;
        }

        public EditEventRequest GetCreateRequest(int? copyFromEventId, int userId, int? groupId, EditEventRequest request)
        {
            bool isNew = request == null;
            var model = request;

            if (model == null)
            {
                model = request = new EditEventRequest();
                model.Timezone = this.Services.People.GetTimezone(userId).Id;
            }

            var tz = TimeZoneInfo.FindSystemTimeZoneById(model.Timezone);
            var now = tz.ConvertFromUtc(DateTime.UtcNow);

            Group group = null;
            if (groupId.HasValue)
                group = this.Services.Groups.SelectGroupById(groupId.Value);

            model.GroupId = groupId;
            model.Group = group;
            model.Title = group == null ? this.Services.Lang.T("Nouvel évènement") : this.Services.Lang.T("Nouvel évènement dans {0}", group.Name);

            if (copyFromEventId != null)
            {
                var copyFrom = this.GetById(copyFromEventId.Value, EventOptions.None);
                if (copyFrom != null)
                {
                    model.Id = copyFrom.Id;
                    model.Name = copyFrom.Name;
                    model.Category = copyFrom.EventCategoryId;
                    if (copyFrom.DateEvent != null && copyFrom.DateEndEvent != null)
                    {
                        var from = copyFrom.DateEvent.Value;
                        var to = copyFrom.DateEndEvent.Value;
                        if (from < now)
                        {
                            from = this.GetNextDateByDayOfWeek(new DateTime(now.Year, now.Month, now.Day, from.Hour, from.Minute, 0), from.DayOfWeek);
                            to = this.GetNextDateByDayOfWeek(new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0), to.DayOfWeek);
                        }
                        else
                        {
                            from = from.AddDays(1D);
                            to = to.AddDays(1D);
                        }

                        model.DateEvent = from;
                        model.DateEndEvent = to;
                    }

                    model.Description = copyFrom.Description;
                    model.GroupId = copyFrom.GroupId ?? 0;
                    model.PlaceId = copyFrom.PlaceId ?? 0;
                    model.Visibility = copyFrom.VisibilityValue;
                    model.Website = copyFrom.Website;
                    model.TicketsWebsite = copyFrom.TicketsWebsite;
                }
            }
            else if (isNew)
            {
                var tomorrow = now.AddDays(1D).Date;
                model.DateEvent = tomorrow.AddHours(9D);
                model.DateEndEvent = tomorrow.AddHours(18D);
            }

            this.FillEditEventModel(model, userId);

            return model;
        }

        private void FillEditEventModel(EditEventRequest model, int userId)
        {
            if (model.AvailableCategories == null)
            {
                model.AvailableCategories = this.Services.EventsCategories.GetAll();
            }

            if (model.AvailableVisibilities == null)
            {
                var user = this.Services.People.SelectWithId(userId);
                model.AvailableVisibilities = this.GetVisibilities(model.Group, user.Company);
            }

            if (model.AvailablePlaces == null)
            {
                var places = this.Services.Places.SelectAll();
                model.AvailablePlaces = new List<PlaceModel>(places.Count);
                foreach (var place in places.Where(p => p.ParentId == null))
                {
                    var placeModel = new PlaceModel(place);
                    var children = places
                        .Where(p => p.ParentId.HasValue && p.ParentId.Value == place.Id)
                        .Select(p => new PlaceModel(p))
                        .OrderBy(c => c.Name)
                        .ToList();
                    foreach (var child in children)
                    {
                        IList<Place> children2 = places
                            .Where(p => p.ParentId.HasValue && p.ParentId.Value == child.Id)
                            .ToList();
                        child.Children = children2
                            .Select(p => new PlaceModel(p))
                            .OrderBy(c => c.Name)
                            .ToList();
                    }

                    placeModel.Children = children;
                    model.AvailablePlaces.Add(placeModel);
                }

                model.AvailablePlaces = model.AvailablePlaces.OrderBy(c => c.Name).ToList();
            }

            if (model.AvailableGroups == null && userId != null)
            {
                var groups = this.Services.GroupsMembers.SelectMyGroupsForGroupsService(userId);
                model.AvailableGroups = groups
                    .Select(g => new GroupModel(g.Group, g))
                    .OrderBy(g => g.Name)
                    .ToList();
            }
        }

        public EditEventResult Create(EditEventRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new EditEventResult(request);

            if (!request.IsValid)
                return result;

            User user = this.Services.People.SelectWithId(request.UserId);
            if (user == null)
            {
                result.Errors.Add(EditEventError.NoSuchUser, NetworksEnumMessages.ResourceManager);
                request.AddValidationError("UserId", result.Errors.Last().DisplayMessage);
            }

            Group group = null;
            if (request.GroupId != null && request.GroupId != 0)
            {
                group = this.Services.Groups.SelectGroupById(request.GroupId.Value);
                if (!this.Services.GroupsMembers.IsGroupMember(user.Id, group.Id))
                {
                    result.Errors.Add(EditEventError.UserIsNotGroupMember, NetworksEnumMessages.ResourceManager);
                    request.AddValidationError("GroupId", result.Errors.Last().DisplayMessage);
                }
            }

            var eventCategories = this.Services.EventsCategories.GetAll();
            if (!eventCategories.Any(c => c.Id == request.Category))
            {
                result.Errors.Add(EditEventError.NoSuchCategory, NetworksEnumMessages.ResourceManager);
                request.AddValidationError("Category", result.Errors.Last().DisplayMessage);
            }

            var visibilities = this.GetVisibilities(group, user.Company);
            if (!visibilities.Any(v => v.Value == request.Visibility))
            {
                result.Errors.Add(EditEventError.NoSuchVisibility, NetworksEnumMessages.ResourceManager);
                request.AddValidationError("Visibility", result.Errors.Last().DisplayMessage);
            }

            Place place = null;
            if (request.PlaceId != null)
            {
                place = this.Services.Places.SelectPlaceById(request.PlaceId.Value);
                if (place == null)
                {
                    result.Errors.Add(EditEventError.NoSuchPlace, NetworksEnumMessages.ResourceManager);
                    request.AddValidationError("PlaceId", result.Errors.Last().DisplayMessage);
                }
            }

            if (result.Errors.Count > 0)
            {
                return result;
            }

            var inputTz = TimeZoneInfo.FindSystemTimeZoneById(request.Timezone);
            var storeTz = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezone);
            var dateBegin = storeTz.ConvertFromUtc(inputTz.ConvertToUtc(request.DateEvent));
            var dateEnd = storeTz.ConvertFromUtc(inputTz.ConvertToUtc(request.DateEndEvent));
            DateTime? dateAnswer = null;
            if (request.NeedAnswerBefore != null)
                dateAnswer = storeTz.ConvertFromUtc(inputTz.ConvertToUtc(request.NeedAnswerBefore.Value));

            var item = new Event
            {
                Name = request.Name,
                Description = request.Description,
                Website = request.Website.NullIfEmpty(),
                TicketsWebsite = request.TicketsWebsite.NullIfEmpty(),

                CreateDate = DateTime.Now,
                CreatedByUserId = user.Id,
                CompanyId = (request.GroupId == null) && (request.Visibility == EventVisibility.Company || request.Visibility == EventVisibility.CompanyPrivate)
                          ? user.CompanyID
                          : default(int?),
                PlaceId = place != null ? place.Id : default(int?),
                GroupId = group != null ? group.Id : default(int?),
                NetworkId = this.Services.NetworkId,
                
                Visibility = (int)request.Visibility,
                EventCategoryId = request.Category,

                DateEvent = dateBegin,
                DateEndEvent = dateEnd,
                NeedAnswerBefore = dateAnswer,
            };
            item = this.Repo.Events.Insert(item);

            var member = new EventMember
            {
                EventId = item.Id,
                UserId = user.Id,
                StateValue = EventMemberState.HasAccepted,
                Notifications = 0,
                Rights = 1,
                Comment = "",
            };
            member = this.Repo.EventsMembers.Insert(member);
            member = this.Services.EventsMembers.SelectEventMemberByEventIdAndUserId(item.Id, member.UserId);

            item = this.GetById(item.Id, EventOptions.None);
            result.Succeed = true;
            result.EventEntity = item;
            result.EventMemberEntity = member;
            result.Event = new EventModel(item);
            var fields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(member.UserId);
            result.EventMember = new EventMemberModel(item, member, fields);

            return result;
        }

        public IList<EnumValueText<EventVisibility>> GetVisibilities(Group group, Company currentUsersCompany)
        {
            var publicArg = this.Services.Network != null ? this.Services.Network.DisplayName : this.Services.NetworkId.ToString();
            var companyArg = group != null ? group.Name : this.Services.AppConfiguration.Tree.Features.EnableCompanies ? currentUsersCompany.Name : "";

            var list = new List<EnumValueText<EventVisibility>>(group == null ? 4 : group.IsPrivate ? 1 : 2);
            if (group != null)
            {
                if (group.IsPrivate)
                {
                    list.Add(new EnumValueText<EventVisibility>(EventVisibility.Company, NetworksEnumMessages.ResourceManager, companyArg));
                }
                else
                {
                    list.Add(new EnumValueText<EventVisibility>(EventVisibility.Public, NetworksEnumMessages.ResourceManager, publicArg));
                    list.Add(new EnumValueText<EventVisibility>(EventVisibility.Company, NetworksEnumMessages.ResourceManager, companyArg));
                }
            }
            else
            {
                list.Add(new EnumValueText<EventVisibility>(EventVisibility.External, NetworksEnumMessages.ResourceManager));
                list.Add(new EnumValueText<EventVisibility>(EventVisibility.Public, NetworksEnumMessages.ResourceManager, publicArg));
                if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
                {
                    list.Add(new EnumValueText<EventVisibility>(EventVisibility.Company, NetworksEnumMessages.ResourceManager, companyArg));
                }

                list.Add(new EnumValueText<EventVisibility>(EventVisibility.Personal, NetworksEnumMessages.ResourceManager));
            }

            return list;
        }

        public IList<EventMemberModel> GetEventMembers(int id)
        {
            var subject = this.Repo.Events.GetById(id);
            var model = new EventModel(subject);

            ////var userMembers = this.Repo.EventsMembers.GetByEventId(id, EventMemberOptions.None);
            ////var users = this.Repo.People.GetActiveLiteById(userMembers.Select(m => m.UserId).ToArray(), this.Services.NetworkId);
            ////var publicMembers = this.Repo.EventPublicMembers.GetByEventId(id);

            ////var items = new List<EventMemberModel>(users.Count + publicMembers.Count);
            ////items.AddRange(users.Select(m =>
            ////    {
            ////        var userMember = userMembers.SingleOrDefault(u => u.UserId == m.Id);
            ////        return new EventMemberModel(userMember, model, userMember != null ? new UserModel(m) : new UserModel(m.Id));
            ////    }));
            ////items.AddRange(publicMembers.Select(m => new EventMemberModel(m, model)));

            var userMembers = this.Repo.EventsMembers.GetByEventId(id, EventMemberOptions.None);
            var users = this.Repo.People.GetUsersViewById(userMembers.Select(m => m.UserId).ToArray(), this.Services.NetworkId);
            var userIds = users.Select(o => o.Key).ToArray();
            
            var phones = this.Services.Repositories.UserProfileFields.GetByUserIdAndFieldType(userIds, new ProfileFieldType[] { ProfileFieldType.Phone, });
            var subs = this.Services.Subscriptions.GetUsersAppliedSubscriptions(userIds, model.DateEventUtc);
           
            var publicMembers = this.Repo.EventPublicMembers.GetByEventId(id);

            var items = new List<EventMemberModel>(userMembers.Count + publicMembers.Count);
            items.AddRange(userMembers.Select(m =>
            {
                var user = users.ContainsKey(m.UserId) ? users[m.UserId] : null;
                var phone = user != null && phones.ContainsKey(user.Id) ? phones[user.Id].Phone() : null;
                var userSubscriptions = user != null && subs.ContainsKey(user.Id) ? subs[user.Id] : null;
                var member = new EventMemberModel(m, model, user != null ? new UserModel(user) : new UserModel(m.UserId));

                member.User.Phone = phone;
                member.User.Subscriptions = userSubscriptions;
                return member;
            }));
            items.AddRange(publicMembers.Select(m => new EventMemberModel(m, model)));

            return items;
        }

        public IDictionary<int, IList<EventMemberModel>> GetEventMembers(int[] eventIds, EventMemberState[] withStates)
        {
            var subject = this.Repo.Events.GetById(eventIds, EventOptions.Place);
            var models = subject
                .Select(e => new EventModel(e))
                .ToDictionary(m => m.Id, m => m);
            var result = subject.ToDictionary(m => m.Id, m => default(IList<EventMemberModel>));

            var userMembers = this.Repo.EventsMembers.GetByEventId(eventIds, EventMemberOptions.None);
            var userIds = userMembers.SelectMany(m => m.Value.Select(n => n.UserId)).ToArray();
            var users = this.Repo.People.GetUsersViewById(userIds, this.Services.NetworkId);
            var publicMembers = this.Repo.EventPublicMembers.GetByEventId(eventIds);

            foreach (var key in result.Keys.ToArray())
            {
                result[key] = new List<EventMemberModel>();
                if (userMembers.ContainsKey(key))
                {
                    result[key].AddRange(userMembers[key].Select(m =>
                    {
                        var user = users.SingleOrDefault(u => u.Key == m.UserId);
                        return new EventMemberModel(m, models[key], user.Value != null ? new UserModel(user.Value) : new UserModel(m.UserId));
                    }));
                }

                if (publicMembers.ContainsKey(key))
                {
                    result[key].AddRange(publicMembers[key].Select(m => new EventMemberModel(m, models[key])));
                }
            }

            return result;
        }

        public IList<Event> GetGroupEventsWithUserId(int userId)
        {
            var groupIds = this.Services.People
                .SelectWithId(userId)
                .Groups
                .Select(o => o.Id)
                .ToArray();

            return this.Services.Events
                .GetEvents(DateTime.Now, DateTime.MaxValue)
                .ByGroupsIds(groupIds)
                .ToList();
        }

        public EditEventRequest GetEditRequest(int eventId, int userId, EditEventRequest model)
        {
            if (model == null)
            {
                var userTz = this.Services.People.GetTimezone(userId);
                var storeTz = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezone);

                var item = this.GetById(eventId, EventOptions.None);
                model = new EditEventRequest()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Category = item.EventCategoryId,
                    Visibility = item.VisibilityValue,
                    Website = item.Website,
                    TicketsWebsite = item.TicketsWebsite,
                    DateEvent = item.DateEvent.HasValue ? userTz.ConvertFromUtc(storeTz.ConvertToUtc(item.DateEvent.Value)) : DateTime.Now,
                    DateEndEvent = item.DateEndEvent.HasValue ? userTz.ConvertFromUtc(storeTz.ConvertToUtc(item.DateEndEvent.Value)) : DateTime.Now.AddHours(2),
                    PlaceId = item.PlaceId,
                    Group = item.Group,
                    Timezone = userTz.Id,
                };
            }

            this.FillEditEventModel(model, userId);

            return model;
        }

        public EditEventResult UpdateEvent(EditEventRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request", "This value cannot be null");

            const string logPath = "EventsService.UpdateEvent";
            var result = new EditEventResult(request);

            var _event = this.GetById(request.Id, EventOptions.Category | EventOptions.Place);
            if (_event == null)
            {
                result.Errors.Add(EditEventError.NoSuchEvent, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            if (!this.CanAdministrate(_event.Id, request.UserId))
            {
                result.Errors.Add(EditEventError.CannotAdministrateEvent, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            PlaceModel place = null;
            if (request.PlaceId.HasValue && (place = this.Services.Places.GetById(request.PlaceId.Value, PlaceOptions.None)) == null)
            {
                result.Errors.Add(EditEventError.NoSuchPlace, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, logPath);
            }

            var inputTz = TimeZoneInfo.FindSystemTimeZoneById(request.Timezone);
            var storeTz = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezone);

            _event.Name = request.Name;
            _event.Name = request.Name;
            _event.Description = request.Description;
            _event.EventCategoryId = request.Category;
            _event.Visibility = (int)request.Visibility;
            _event.DateEvent = storeTz.ConvertFromUtc(inputTz.ConvertToUtc(request.DateEvent));
            _event.DateEndEvent = storeTz.ConvertFromUtc(inputTz.ConvertToUtc(request.DateEndEvent));
            _event.Website = request.Website;
            _event.TicketsWebsite = request.TicketsWebsite;
            _event.PlaceId = place != null ? place.Id : default(int?);

            this.Repo.Events.Update(_event);

            result.Succeed = true;
            return this.LogResult(result, logPath);
        }

        public IList<EventModel> GetAllEventsRelatedToUserWithModel(int userId, DateTime? start = null, DateTime? end = null)
        {
            var dateStart = start.HasValue ? start.Value : DateTime.Now;
            var dateEnd = end.HasValue ? end.Value : DateTime.MaxValue;

            // Get current user
            var user = this.Services.People.GetByIdFromAnyNetwork(userId, PersonOptions.None);
            var userTimezone = this.Services.People.GetTimezone(user);
            
            // Get public events
            var memberState = new EventMemberState[] { EventMemberState.HasAccepted, EventMemberState.MaybeJoin, };
            var events = this.SelectPublicAndNetworkEvents(EventOptions.Owner)
                .Where(o => o.DateEvent >= dateStart && o.DateEndEvent <= dateEnd)
                .ToList();
            var eventIds = events.Select(o => o.Id).ToArray();
            var memberEvents = this.GetEventMembers(eventIds, memberState);
            var eventsModels = events
                .Select(o => new EventModel(o, memberEvents[o.Id]))
                .ToList();

            // Get specifics events of user
            // Groups events
            ////var groups = this.Services.Groups.SelectMyGroups(userId).Select(o => o.Id).ToList();
            var groups = this.Services.GroupsMembers.GetUsersGroupMemberships(userId, GroupMemberOptions.Group);
            foreach (var group in groups)
            {
                var groupEvents = this.GetByGroupIdWithModel(group.Key, userId)
                    .Where(o => o.DateEvent >= dateStart && o.DateEndEvent <= dateEnd)
                    .Where(o => !eventsModels.Select(i => i.Id).ToArray().Contains(o.Id))
                    .ToList();
                eventsModels.AddRange(groupEvents);
            }

            return eventsModels
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public IList<Event> GetAllEventsRelatedToUser(int userId, DateTime? start = null, DateTime? end = null)
        {
            var dateStart = start.HasValue ? start.Value : DateTime.Now;
            var dateEnd = end.HasValue ? end.Value : DateTime.MaxValue;

            // Get public events
            var memberState = new EventMemberState[] { EventMemberState.HasAccepted, EventMemberState.MaybeJoin, };
            var events = this.SelectPublicAndNetworkEvents(EventOptions.None)
                .Where(o => o.DateEvent >= dateStart && o.DateEndEvent <= dateEnd)
                .ToList();
            var memberEvents = this.GetEventMembers(events.Select(o => o.Id).ToArray(), memberState);

            // Get specifics events of user
            // Groups events
            ////var groups = this.Services.Groups.SelectMyGroups(userId).Select(o => o.Id).ToList();
            var groups = this.Services.GroupsMembers.GetUsersGroupEntities(userId);
            foreach (var group in groups)
            {
                var groupEvents = this.GetByGroupId(group.Id)
                    .Where(o => o.DateEvent >= dateStart && o.DateEndEvent <= dateEnd)
                    .Where(o => !events.Select(i => i.Id).ToArray().Contains(o.Id))
                    .ToList();
                events.AddRange(groupEvents);
            }

            return events
                .OrderBy(o => o.DateEvent)
                .ToList();
        }
        
        public IList<EventModel> GetByGroupIdWithModel(int groupId, int userId)
        {
            var current = this.Services.People.SelectWithId(userId);
            var userTimezone = this.Services.People.GetTimezone(current);

            var events = this.GetByGroupId(groupId);
            var eventMembers = this.GetEventMembers(events.Select(o => o.Id).ToArray(), new EventMemberState[] { EventMemberState.HasAccepted, EventMemberState.MaybeJoin });
            return events
                .Select(o => new EventModel(o, eventMembers[o.Id]))
                .OrderBy(o => o.DateEvent)
                .ToList();
        }

        public bool CanAdministrate(int eventId, int userId)
        {
            var ev = this.GetById(eventId, EventOptions.None);
            var user = this.Services.People.GetByIdFromAnyNetwork(userId, PersonOptions.None);
            if (ev == null || user == null)
                return false;

            if (ev.CreatedByUserId == user.Id)
                return true;

            if (user.NetworkAccessLevel.Value.HasAnyFlag(NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork))
                return true;

            if (ev.GroupId.HasValue && this.Services.GroupsMembers.IsAdmin(user.Id, ev.GroupId.Value))
                return true;

            return false;
        }

        public string GetProfileUrl(Event item, UriKind uriKind)
        {
            var path = "Events/Event/" + item.Id;
            var uri = new Uri(this.Services.GetUrl(path), UriKind.Absolute);
            return uriKind == UriKind.Relative ? uri.PathAndQuery : uri.AbsoluteUri;
        }

        public void SetupUserRights(EventModel model, int userId)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var userModel = this.Services.People.GetByIdFromAnyNetwork(userId, PersonOptions.None);
            var eventMember = this.Services.EventsMembers.GetMembership(model.Id, userModel.Id);
            var groupMember = model.GroupId != null ? this.Services.GroupsMembers.GetMembershipModel(model.GroupId.Value, userId) : null;
            model.SetupUserRights(userModel, eventMember, groupMember);
        }

        public IDictionary<int, EventModel> GetModelById(int[] eventIds, EventOptions options)
        {
            var items = this.Repo.Events.GetById(eventIds, this.Services.NetworkId, options);
            return items.ToDictionary(e => e.Id, e => new EventModel(e));
        }

        public IList<EventModel> GetTopFutureEvents(int userId, int count, EventOptions options)
        {
            var minDate = DateTime.UtcNow;
            var user = this.Services.Cache.GetUser(userId);
            var userTimezone = this.Services.People.GetTimezone(user);

            var groupIds = this.Repo.GroupsMembers.GetActualMembershipsByUser(userId, GroupMemberStatus.Accepted)
                .Select(m => m.GroupId)
                .ToArray();
            var companyIds = new int[] { user.CompanyId, };
            var items = this.Repo.Events.GetFutureItems(this.Services.NetworkId, minDate, count, true, groupIds, companyIds, userId);

            var models = items.ToDictionary(e => e.Id, e => new EventModel(e));
            this.FillItems(options, models);

            return new List<EventModel>(models.Values);
        }

        public IList<EventModel> GetTopFutureGroupEvents(int userId, int count, int groupId, EventOptions options)
        {
            var minDate = DateTime.UtcNow;
            var user = this.Services.Cache.GetUser(userId);
            var userTimezone = this.Services.People.GetTimezone(user);

            var items = this.Repo.Events.GetFutureItems(this.Services.NetworkId, minDate, count, false, new int[] { groupId, }, null, null);

            var models = items.ToDictionary(e => e.Id, e => new EventModel(e));
            this.FillItems(options, models);

            return new List<EventModel>(models.Values);
        }

        private void FillItems(EventOptions options, Dictionary<int, EventModel> models)
        {
            var ids = models.Keys.ToArray();

            var categoryIds = models.Values.GroupBy(e => e.CategoryId).Select(g => g.Key).ToArray();
            var eventCategories = this.Services.Cache.GetEventCategories(categoryIds);
            foreach (var model in models.Values)
            {
                model.CategoryName = eventCategories[model.CategoryId].Name;
            }

            if ((options & EventOptions.EventsMembersPeople) == EventOptions.EventsMembersPeople)
            {
                var members = this.Repo.EventsMembers.GetByEventId(ids, EventMemberOptions.None, true);
                var userIds = members.Values.SelectMany(g => g.Select(e => e.UserId)).ToArray();
                var users = this.Services.Cache.GetUsers(userIds);

                foreach (var model in models.Values)
                {
                    var eventModel = new EventModel(model.Id);
                    if (members.ContainsKey(model.Id))
                    {
                        var memberList = members[model.Id];
                        if (model.Members == null)
                            model.Members = new List<EventMemberModel>(memberList.Count);
                        model.Members.AddRange(memberList
                            .Where(m => users[m.UserId].IsActive)
                            .Select(m => new EventMemberModel(m, eventModel, users[m.UserId])));
                    }
                }
            }

            if ((options & EventOptions.Place) == EventOptions.Place)
            {
                var placeIds = models.Values.Where(e => e.PlaceId != null).Select(e => e.PlaceId.Value).ToArray();
                //var places = this.Repo.Places.GetById(placeIds, PlaceOptions.None);
                var places = this.Services.Cache.GetPlaces(placeIds);

                foreach (var model in models.Values)
                {
                    if (model.PlaceId != null && places.ContainsKey(model.PlaceId.Value))
                    {
                        model.Place = places[model.PlaceId.Value];
                    }
                }
            }
        }

        public int CountAtPlace(int placeId)
        {
            return this.Repo.Events.CountAtPlace(placeId);
        }

        public IList<EventModel> GetRecentEventsAtPlace(int placeId, int userId, EventOptions options)
        {
            var user = this.Services.Cache.GetUser(userId);
            var groupIds = this.Repo.GroupsMembers.GetActualMembershipsByUser(userId, GroupMemberStatus.Accepted)
                .Select(m => m.GroupId)
                .ToArray();
            var companyIds = new int[] { user.CompanyId, };

            var minDate = DateTime.UtcNow;
            var maxDate = minDate.AddYears(4);
            var items = this.Repo.Events.GetAtPlace(this.Services.NetworkId, placeId, minDate, maxDate, EventOptions.None, true, groupIds, companyIds);
            var eventIds = items.Select(e => e.Id).ToArray();
            var models = items.Select(e => new EventModel(e)).ToList();

            if ((options & EventOptions.EventsMembers) == EventOptions.EventsMembers)
            {
                var memberships = this.Repo.EventsMembers.GetByEventId(eventIds, EventMemberOptions.None, false);

                foreach (var item in models)
                {
                    item.SetMembers(memberships[item.Id].Select(m => new EventMemberModel(m.EventId, m.UserId, m.StateValue)).ToList());
                }
            }

            return models;
        }

        public int CountCreatedByUser(int userId)
        {
            return this.eventsRepository.CountCreatedByUser(userId, this.Services.NetworkId);
        }

        public ValidateUserCalendarTokenResult ValidateUserCalendarToken(ValidateUserCalendarTokenRequest request)
        {
            const string path = "ValidateUserCalendarToken";
            var result = new ValidateUserCalendarTokenResult(request);

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.InvalidToken, "The given token is an empty string. ", NetworksEnumMessages.ResourceManager);
            }

            if (request.ActingUserId == null)
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.NoSuchUser, "The acting user id is not set. ", NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            var user = this.Services.People.GetActiveById(request.ActingUserId.Value, PersonOptions.Company);
            if (user == null)
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.NoSuchUser, "No such user by id " + request.ActingUserId + ". ", NetworksEnumMessages.ResourceManager);
            }
            else if (!user.IsActive)
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.NotAuthorized, "User " + request.ActingUserId + " is not authorized. ", NetworksEnumMessages.ResourceManager);
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.InvalidToken, "The given token is an empty string. ", NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, path);
            }

            byte[] providedToken;
            try
            {
                providedToken = request.Token.FromUrlBase64();
            }
            catch (FormatException ex)
            {
                result.Errors.AddDetail(ValidateUserCalendarTokenError.InvalidToken, ex.Message, NetworksEnumMessages.ResourceManager);
                return this.LogResult(result, path);
            }

            string cryptoPurpose = KnownCryptoPurposes.PrivateCalendarUserToken;
            byte[] userToken = this.Services.Crypto.ComputeStaticUserHash(cryptoPurpose, user.Id, user.CreateDateUtc ?? DateTime.MinValue);

            if (userToken.ArraySequenceEquals(providedToken))
            {
                result.Succeed = true;
            }
            else
            {
                result.Errors.Add(ValidateUserCalendarTokenError.WrongToken, NetworksEnumMessages.ResourceManager);
            }

            return this.LogResult(result, path);
        }

        public string GetUserCalendarToken(UserModel user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            string cryptoPurpose = KnownCryptoPurposes.PrivateCalendarUserToken;
            byte[] userToken = this.Services.Crypto.ComputeStaticUserHash(cryptoPurpose, user.Id, user.CreateDateUtc ?? DateTime.MinValue);

            return userToken.ToUrlBase64();
        }

        public string GetUserCalendarToken(int userId)
        {
            var user = this.Services.People.GetActiveById(userId, PersonOptions.Company);
            if (user == null)
            {
                throw new ArgumentException("userId", "No such user. ");
            }

            return this.GetUserCalendarToken(user);
        }
    }
}
