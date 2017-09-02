
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data;
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Options;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class EventsMembersService : ServiceBase, IEventsMembersService
    {
        [DebuggerStepThrough]
        internal EventsMembersService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IEventsMembersRepository EventsMembersRepo
        {
            get { return this.Repo.EventsMembers; }
        }

        /// <summary>
        /// Gets all state.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <returns></returns>
        public IList<User> GetAllState(int EventId)
        {
            return this.EventsMembersRepo.GetGuestList(EventId).ToList();
        }

        /// <summary>
        /// Gets the guest list.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public IList<User> GetGuestList(int EventId, EventMemberState state)
        {
            return this.EventsMembersRepo.GetGuestList(EventId, state).ToList();
        }

        /// <summary>
        /// Determines whether the specified event id is invited.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <param name="PersonId">The person id.</param>
        /// <returns>
        ///   <c>true</c> if the specified event id is invited; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInvited(int EventId, Guid PersonId)
        {
            IQueryable<User> ps = this.EventsMembersRepo.GetGuestList(EventId);
            if (ps != null && ps.Count() > 0)
            {
                return ps.Any(o => o.UserId == PersonId);
            }
            return false;
        }

        /// <summary>
        /// Gets the registered list.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <param name="Accepted">if set to <c>true</c> [accepted].</param>
        /// <returns></returns>
        public IList<User> GetRegisteredList(int EventId, bool Accepted)
        {
            return this.EventsMembersRepo.GetRegisteredList(EventId, Accepted)
                .ToList();
        }

        public bool IsInvited(int eventId, int userId)
        {
            IQueryable<User> ps = this.EventsMembersRepo.GetGuestList(eventId);
            if (ps != null && ps.Count() > 0)
            {
                return ps.Any(o => o.Id == userId);
            }
            return false;
        }

        /// <summary>
        /// Adds the event member.
        /// </summary>
        /// <param name="me">Me</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int AddEventMember(User me, EventMember item)
        {
            var entity = this.EventsMembersRepo.Insert(item);
            // notification par email si c'est une invitation
            if (item.StateValue == EventMemberState.IsInvited)
            {
                // Si l'utilisateur accepte les notifications par email
                Notification n = this.Services.Notifications.SelectNotifications(item.UserId);
                if (n.EventInvitation)
                {
                    User contact = this.Services.People.SelectWithId(item.UserId);
                    if (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(contact))
                    {
                        Event @event = this.Services.Events.GetById(item.EventId, EventOptions.Category | EventOptions.Place);
                        this.Services.Email.SendNotification(me, contact, @event);
                    }
                }

            }

            return entity.EventId;
        }

        /// <exception cref="SparkleServicesException"></exception>
        public EventMember Invite(int eventId, int userId, int inviterUserId)
        {
            var evt = this.Repo.Events.GetById(eventId);
            var notif = this.Services.Notifications.SelectNotifications(userId);
            var item = this.Repo.EventsMembers.GetById(userId, eventId);

            if (item == null)
            {
                item = new EventMember
                {
                EventId = evt.Id,
                UserId = userId,
                StateValue = EventMemberState.IsInvited,
                Notifications = notif.EventInvitation ? 1 : 0,
                Rights = 0,
                Comment = "",
            };
                this.Repo.EventsMembers.Insert(item);
            }

            if (notif.EventInvitation)
            {
                var invitee = this.Repo.People.GetById(userId);
                var isUserSubscribed = !this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(invitee);
                if (isUserSubscribed)
                {
                    var inviter = this.Repo.People.GetById(inviterUserId);
                    this.Services.Email.SendNotification(inviter, invitee, evt);
                }
            }

            return item;
        }

        /// <summary>
        /// Updates the registered.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public EventMember UpdateRegistered(EventMember item)
        {
            return this.EventsMembersRepo.UpdateRegistered(item);
        }

        /// <summary>
        /// Deletes the registered.
        /// </summary>
        /// <param name="item">The item.</param>
        public void DeleteRegistered(EventMember item)
        {
            this.EventsMembersRepo.DeleteRegistered(item);
        }

        /// <summary>
        /// Selects the registered by event id.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <returns></returns>
        public IList<EventMember> SelectRegisteredByEventId(int EventId)
        {
            return this.EventsMembersRepo.SelectRegistered(OptionsList)
                                .WithEventId(EventId)
                                .ToList();
        }

        /// <summary>
        /// Selects the registered by user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<EventMember> SelectRegisteredByUserId(int userId)
        {
            return this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .WithUserId(userId)
                .ToList();
        }

        /// <summary>
        /// Selects my events for events service.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<EventMember> SelectMyEventsForEventsService(int userId)
        {
            return this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .Where(p => p.Event.NetworkId == this.Services.NetworkId)
                .Participation(userId)
                .ToList();
        }

        /// <summary>
        /// Selects the event members.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <returns></returns>
        public IList<EventMember> SelectEventMembers(int EventId)
        {
            return this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .WithEventId(EventId)
                .ToList();
        }

        /// <summary>
        /// Selects the event members.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <returns></returns>
        public IList<EventMember> SelectEventMembers(int EventId, EventMemberOptions options)
        {
            return this.EventsMembersRepo
                .NewQuery(options)
                .WithEventId(EventId)
                .ToList();
        }

        /// <summary>
        /// Selects the event member by event id and user id.
        /// </summary>
        /// <param name="EventId">The event id.</param>
        /// <param name="UserId">The user id.</param>
        /// <returns></returns>
        public EventMember SelectEventMemberByEventIdAndUserId(int EventId, int userId)
        {
            return this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .WithEventIdAndUserId(EventId, userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Gets the user state of the event.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public EventMember GetEventState(int eventId, int userId)
        {
            return this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .WithEventIdAndUserId(eventId, userId)
                .SingleOrDefault();
        }

        public bool IsAdmin(int userId, int eventId)
        {
            EventMember member = this.EventsMembersRepo
                .NewQuery(EventMemberOptions.Event | EventMemberOptions.User)
                .WithUserId(userId)
                .WithEventId(eventId)
                .FirstOrDefault();
            if (member != null && member.RightsValue == EventMemberRight.Admin)
                return true;

            var user = this.Services.People.GetActiveById(userId, Data.Options.PersonOptions.None);
            var _event = this.Services.Events.GetById(eventId, EventOptions.None);
            if (_event.CompanyId.HasValue && _event.CompanyId.Value == user.CompanyId && user.CompanyAccessLevel.HasValue && user.CompanyAccessLevel.Value.HasAnyFlag(CompanyAccessLevel.Administrator | CompanyAccessLevel.CommunityManager))
                return true;

            return false;
        }

        public bool IsEventMember(int userId, int eventId)
        {
            EventMember member = this.EventsMembersRepo
                .SelectRegistered(this.OptionsList)
                .WithUserId(userId)
                .WithEventId(eventId)
                .FirstOrDefault();
            if (member != null && member.StateValue == EventMemberState.HasAccepted)
                return true;
            return false;
        }

        public int Count()
        {
            return this.EventsMembersRepo.CountAll(this.Services.NetworkId);
        }

        public int CountActualMembers()
        {
            return this.EventsMembersRepo.CountActualMembers(this.Services.NetworkId);
        }

        public int CountInvitedMembers()
        {
            return this.EventsMembersRepo.CountInvitedMembers(this.Services.NetworkId);
        }

        public IDictionary<int, EventMemberModel> GetMyMembershipForEvents(int[] eventIds, int userId)
        {
            return this.Repo.EventsMembers.Select()
                .Where(e => eventIds.Contains(e.EventId) && e.UserId == userId)
                .ToDictionary(e => e.EventId, e => new EventMemberModel(e, null, null));
        }

        public EventMemberModel GetMembership(int eventId, int userId)
        {
            var item = this.Repo.EventsMembers.GetMembership(eventId, userId);
            if (item == null)
            {
                return new EventMemberModel(eventId, userId, EventMemberState.None);
            }
            else
            {
                return new EventMemberModel(item, new EventModel(eventId), new UserModel(userId));
            }
        }

        public void ClearNotifications(int eventId, int userId)
        {
            var item = this.Repo.EventsMembers.GetMembership(eventId, userId);
            item.Notifications = 0;
            this.Repo.EventsMembers.UpdateRegistered(item);
        }
    }
}
