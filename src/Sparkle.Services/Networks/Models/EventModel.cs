
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Common.DataAnnotations;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Attributes;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class EventModel
    {
        private EventAccessModel eventAccessModel = new EventAccessModel();
        private const string StoreEventDatesTimezone = "Romance Standard Time";
   
        public EventModel()
        {
        }

        public EventModel(Event item)
        {
            this.Set(item);
        }

        public EventModel(Event item, IList<EventMemberModel> members)
        {
            this.Set(item);
            this.Set(members);
        }

        public EventModel(int id)
        {
            this.Id = id;
        }

        private void Set(Event item)
        {
            // Infos de base
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;
            this.CreateDate = item.CreateDate;
            this.DateEvent = item.DateEvent ?? DateTime.Now;
            this.DateEndEvent = item.DateEndEvent ?? DateTime.Now.AddHours(2);
            this.PlaceId = item.PlaceId;
            this.CategoryId = item.EventCategoryId;

            this.DateEventUtc = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezone).ConvertToUtc(this.DateEvent);
            this.DateEndEventUtc = TimeZoneInfo.FindSystemTimeZoneById(StoreEventDatesTimezone).ConvertToUtc(this.DateEndEvent);

            this.CompanyId = item.CompanyId;
            if (this.CompanyId != null && item.CompanyReference.IsLoaded)
            {
                this.CompanyName = item.Company.Name;
            }

            this.GroupId = item.GroupId;
            if (this.GroupId != null && item.GroupReference.IsLoaded)
            {
                this.Group = new GroupModel(item.Group);
            }

            if (item.EventCategoryReference.IsLoaded && item.EventCategory != null)
            {
                this.CategoryName = item.EventCategory.Name;
            }

            this.VisibilityId = item.Visibility;

            this.Website = item.Website;
            this.TicketWebsite = item.TicketsWebsite;
            this.NetworkId = item.NetworkId;
            ////if (item.NetworkReference.IsLoaded)
            ////{
            ////    this.Network = new NetworkModel(item.Network, null);
            ////}
            ////else
            {
                this.Network = new NetworkModel(item.NetworkId);
            }

            this.CreatedByUserId = item.CreatedByUserId;
            if (item.UserReference.IsLoaded)
            {
                User creator = item.User;
                this.CreatedBy = new UserModel(creator);
            }

            ////this.NewModel = new EventModel(item);
            this.DeleteDateUtc = item.DeleteDateUtc;
            
            
            // Date and time
            this.DurationHours = (int)Math.Ceiling((this.DateEndEventUtc - this.DateEventUtc).TotalHours);

            // place
            if (item.PlaceId.HasValue && item.PlaceReference.IsLoaded)
            {
                // TODO: LAZY LOADING TAKES PLACE HERE. This is something we want to change.
                Place place = item.Place;
                if (place != null)
                {
                    this.Place = new PlaceModel(place);

                    if (place.ParentId.HasValue)
                    {
                        Place parent = place.Parent;
                        if (parent != null)
                        {
                            if (parent.ParentId.HasValue)
                            {
                                Place parent2 = parent.Parent;
                                if (parent2 != null)
                                {
                                    this.Place.Address = parent2.Address;
                                    this.Place.ZipCode = parent2.ZipCode;
                                    this.Place.City = parent2.City;
                                    this.Place.Latitude = parent2.lat;
                                    this.Place.Longitude = parent2.lon;
                                }
                            }
                            else
                            {
                                this.Place.Address = parent.Address;
                                this.Place.ZipCode = parent.ZipCode;
                                this.Place.City = parent.City;
                                this.Place.Latitude = parent.lat;
                                this.Place.Longitude = parent.lon;
                            }
                        }
                    }
                }
            }
        }

        private void Set(IList<EventMemberModel> members)
        {
            if (members != null)
            {
            ////    if (members != null && members.Count > 0)
            ////    {
            ////        this.NewModel.SetMembers(members);
            ////        this.CurrentMember = this.NewModel.Members.Where(o => o.UserId.HasValue && o.UserId.Value == this.CurrentUserId).SingleOrDefault();
            ////    }
                this.Members = members
                    .Where(m => m.EventId == this.Id)
                    .ToList();
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime DateEvent { get; set; }

        public DateTime DateEndEvent { get; set; }

        public int? PlaceId { get; set; }

        public int CategoryId { get; set; }

        public PlaceModel Place { get; set; }

        public int? CompanyId { get; set; }

        public string CategoryName { get; set; }

        public int VisibilityId { get; set; }

        public EventVisibility Visibility
        {
            get { return (EventVisibility)this.VisibilityId; }
            set { this.VisibilityId = (short)value; }
        }

        public string VisibilityName
        {
            get
            {
                var key = "EventVisibility_" + this.Visibility;
                switch (this.Visibility)
                {
                    case EventVisibility.Public:
                        return string.Format(
                            NetworksEnumMessages.EventVisibility_Public,
                            this.Network != null ? this.Network.DisplayName : this.NetworkId.ToString());

                    case EventVisibility.Company:
                        return string.Format(
                            NetworksEnumMessages.EventVisibility_Company,
                            this.Group != null ? this.Group.Name : 
                            this.CompanyId != null ? this.CompanyName : 
                            this.Network != null ? this.Network.DisplayName : 
                            this.NetworkId.ToString());

                    case EventVisibility.External:
                        return NetworksEnumMessages.EventVisibility_External;

                    case EventVisibility.Personal:
                        return NetworksEnumMessages.EventVisibility_Personal;
                    
                    case EventVisibility.CompanyPrivate:
                        return NetworksEnumMessages.EventVisibility_CompanyPrivate;
                }

                var value = NetworksEnumMessages.ResourceManager.GetString(key)
                         ?? NetworksEnumMessages.EventVisibility_Unknown;
                return value;
            }
        }

        public NetworkModel Network { get; set; }

        public int NetworkId { get; set; }

        public bool IsPast
        {
            get { return this.DateEvent < DateTime.Now; }
        }

        public DateTime DateEventUtc { get; set; }

        public DateTime DateEndEventUtc { get; set; }

        public int CreatedByUserId { get; set; }

        public void SetMembers(IList<EventMemberModel> model1)
        {
            this.Members = model1;
        }

        public IEnumerable<EventMemberModel> InvitedMembers { get { return this.EnumerateMembers(EventMemberState.IsInvited); } }
        public IEnumerable<EventMemberModel> GoingMembers   { get { return this.EnumerateMembers(EventMemberState.HasAccepted); } }
        public IEnumerable<EventMemberModel> MaybeMembers   { get { return this.EnumerateMembers(EventMemberState.MaybeJoin); } }
        public IEnumerable<EventMemberModel> AwayMembers    { get { return this.EnumerateMembers(EventMemberState.WontCome); } }
        public IEnumerable<EventMemberModel> WantMembers    { get { return this.EnumerateMembers(EventMemberState.WantJoin); } }

        public IEnumerable<EventMemberModel> ComingMembers { get { return this.Members.Where(m => m.State == EventMemberState.MaybeJoin || m.State == EventMemberState.HasAccepted); } }
        
        public int ComingMemberCount
        {
            get { return this.ComingMembers.Count(); }
        }

        private IEnumerable<EventMemberModel> EnumerateMembers(EventMemberState state)
        {
            return this.Members.Where(m => m.State == state);
        }

        private IList<EventMemberModel> members = null;
        public IList<EventMemberModel> Members
        {
            get
            {
                if (this.members == null)
                    this.members = new List<EventMemberModel>();

                return this.members;
            }
            set { this.members = value; }
        }

        public string Website { get; set; }

        public string TicketWebsite { get; set; }

        public int DurationHours { get; set; }

        public UserModel CreatedBy { get; set; }

        public bool IsPublic
        {
            get { return this.VisibilityId == (int)EventVisibility.Public || this.VisibilityId == (int)EventVisibility.External; }
        }

        public string Picture { get; set; }

        public DateTime? NeedAnswerBeforeDate { get; set; }

        public TimelineModel EventTimeline { get; set; }

        public EventMemberModel CurrentMember { get; set; }

        public string CompanyName { get; set; }

        public int? GroupId { get; set; }

        public object DeleteDateUtc { get; set; }

        public GroupModel Group { get; set; }

        public EventAccessModel AccessModel
        {
            get { return this.eventAccessModel; }
            set { this.eventAccessModel = value; }
        }

        public EventType Type
        {
            get
            {
                if (this.GroupId != null)
                    return EventType.Group;

                if (this.CompanyId != null)
                    return EventType.Company;

                return EventType.Network;
            }
        }

        public void SetupUserRights(UserModel user, EventMemberModel eventMember, GroupMemberModel groupMember)
        {
            this.eventAccessModel = this.GetUserRights(user, eventMember, groupMember);
        }

        public EventAccessModel GetUserRights(UserModel user, EventMemberModel eventMember, GroupMemberModel groupMember)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var item = new EventAccessModel();

            var networkAccessLevels = new NetworkAccessLevel[]
            {
                NetworkAccessLevel.ModerateNetwork,
                NetworkAccessLevel.NetworkAdmin,
                NetworkAccessLevel.SparkleStaff,
            };
            if (user.NetworkAccessLevel.Value.HasAnyFlag(networkAccessLevels))
            {
                item.SetIsAdmin();
            }

            if (this.Type == EventType.Network)
            {
                if (this.CreatedByUserId == user.Id)
                {
                    item.SetIsOwner();
                    return item;
                }

                if (eventMember != null && eventMember.Rights == EventMemberRight.Admin)
                {
                    item.SetIsAdmin();
                    return item;
                }

                ////EventMemberState[] states;
                switch (this.Visibility)
                {
                    case EventVisibility.Devices:
                    case EventVisibility.External:
                    case EventVisibility.Public:
                        item.IsVisible = true;
                        item.CanInvite = true;
                        break;

                    case EventVisibility.Company:
                        item.IsVisible = user.CompanyId == this.CompanyId.Value;

                        if (eventMember != null && eventMember.State != EventMemberState.None)
                        {
                            item.IsVisible = true;
                            item.CanInvite = true;
                        }

                        break;

                    case EventVisibility.Personal:
                        item.IsVisible = eventMember != null && eventMember.State != EventMemberState.None;
                        break;

                    case EventVisibility.CompanyPrivate:
                        item.IsVisible = user.CompanyId == this.CompanyId.Value;
                        item.CanInvite = user.CompanyId == this.CompanyId.Value;
                        break;

                    default:
                        throw new InvalidOperationException("EventVisibility " + this.Visibility + " is not supported");
                }
            }
            else if (this.Type == EventType.Group)
            {
                var authenticatedVisibilities = new EventVisibility[]
                {
                    EventVisibility.External,
                    EventVisibility.Public,
                };
                if (authenticatedVisibilities.Contains(this.Visibility))
                {
                    item.IsVisible = true;
                    item.CanInvite = true;
                }

                if (groupMember != null && groupMember.Status == GroupMemberStatus.Accepted)
                {
                    if (this.CreatedByUserId == user.Id)
                    {
                        item.SetIsOwner();
                        return item;
                    }

                    if (eventMember != null && eventMember.Rights == EventMemberRight.Admin)
                    {
                        item.SetIsAdmin();
                    }

                    switch (groupMember.RightStatus)
                    {
                        case GroupMemberRight.Basic:
                            item.IsVisible = true;
                            item.CanInvite = true;
                            break;

                        case GroupMemberRight.Admin:
                            item.SetIsAdmin();
                            break;

                        default:
                            throw new InvalidOperationException("GroupMemberRight " + groupMember.RightStatus + " is not supported");
                    }
                }
            }
            else if (this.Type == EventType.Company)
            {
                var authenticatedVisibilities = new EventVisibility[]
                {
                    EventVisibility.External,
                    EventVisibility.Public,
                    EventVisibility.Company,
                };
                if (authenticatedVisibilities.Contains(this.Visibility))
                {
                    item.IsVisible = true;
                    item.CanInvite = true;
                }

                if (user.CompanyId == this.CompanyId.Value)
                {
                    item.IsVisible = true;
                    item.CanInvite = true;
                }

                var companyRightsForAdmin = new CompanyAccessLevel[]
                {
                    CompanyAccessLevel.Administrator,
                    CompanyAccessLevel.CommunityManager,
                };
                if (companyRightsForAdmin.Contains(user.CompanyAccessLevel.Value))
                {
                    item.SetIsAdmin();
                }
            }
            else
            {
                throw new InvalidOperationException("EventType " + this.Type + " is not supported");
            }

            return item;
        }

        public void SetupAnonymousRights()
        {
            this.AccessModel = this.GetAnonymousRights();
        }

        public EventAccessModel GetAnonymousRights()
        {
            return new EventAccessModel
            {
                IsVisible = this.IsPublic,
            };
        }
    }

    public class EventsModel
    {
        public int GroupId { get; set; }

        public bool AllowControls { get; set; }

        public bool Authenticated { get; set; }

        public IList<EventModel> PublicEvents { get; set; }

        public int? MinVisibility { get; set; }

        public int? MaxVisibility { get; set; }

        public string MyCalendarUrl { get; set; }

        public string MyCalendarUrlError { get; set; }
    }

    public class JoinEventModel
    {
        public int EventId { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string LastName { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = "StringLength")]
        public string CompanyName { get; set; }

        [Required]
        public string Job { get; set; }

        [Required]
        [SrkToolkit.DataAnnotations.PhoneNumber, PhoneUIHint]
        public string PhoneNumber { get; set; }

        public string EventPicture { get; set; }

        public short ResultId { get; set; }

        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public PlaceModel Place { get; set; }
    }

    public class JsonEventModel
    {
        public JsonEventModel()
        { }

        public JsonEventModel(Event item)
        {
            this.Name = item.Name;
            this.Date = item.DateEvent.Value;
            this.Description = item.Description;
            this.DateEndEvent = item.DateEndEvent;
            if (item.EventCategory != null)
                this.EventCategory = item.EventCategory.Name;
            this.GroupId = item.GroupId;
            this.NeedAnswerBefore = item.NeedAnswerBefore;
            this.PlaceId = item.PlaceId;
            this.Website = item.Website;
            this.Id = item.Id;
        }

        public JsonEventModel(EventModel item)
        {
            this.Name = item.Name;
            this.Date = item.DateEvent;
            this.Description = item.Description;
            this.DateEndEvent = item.DateEndEvent;
            this.EventCategory = item.CategoryName;
            this.GroupId = item.GroupId;
            this.NeedAnswerBefore = item.NeedAnswerBeforeDate;
            this.PlaceId = item.PlaceId;
            this.Website = item.Website;
            this.Id = item.Id;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public DateTime? DateEndEvent { get; set; }

        public string EventCategory { get; set; }

        public int? GroupId { get; set; }

        public DateTime? NeedAnswerBefore { get; set; }

        public int? PlaceId { get; set; }

        public string Website { get; set; }

        public int Id { get; set; }
    }

    public class Recurrence
    {
        public Recurrence()
        {
            this.UseRecurrence = 0;
            this.Pattern = RecurrencePattern.Weekly;
            this.DailyEvery = 1;
            this.DailyEveryWeek = 0;

            this.WeeklyEvery = 1;
            this.WeeklyMonday = 0;
            this.WeeklyThuesday = 0;
            this.WeeklyWednesday = 0;
            this.WeeklyThursday = 0;
            this.WeeklyFriday = 0;
            this.WeeklySaturday = 0;
            this.WeeklySunday = 0;

            this.MonthlyChoice = 1;
            this.MonthlyDay = DateTime.Now.Day;
            this.MonthlyEvery = 1;
            this.MonthlyDay2 = 0;


            this.EndAfter = 10;
        }

        public int UseRecurrence { get; set; }
        public RecurrencePattern Pattern { get; set; }

        public int DailyEvery { get; set; }
        public int DailyEveryWeek { get; set; }

        public int WeeklyEvery { get; set; }
        public int WeeklyMonday { get; set; }
        public int WeeklyThuesday { get; set; }
        public int WeeklyWednesday { get; set; }
        public int WeeklyThursday { get; set; }
        public int WeeklyFriday { get; set; }
        public int WeeklySaturday { get; set; }
        public int WeeklySunday { get; set; }

        public int MonthlyChoice { get; set; }
        public int MonthlyDay { get; set; }
        public int MonthlyEvery { get; set; }

        public int MonthlyThe { get; set; }
        public int MonthlyDay2 { get; set; }
        public int MonthlyEvery2 { get; set; }

        public int YearlyEvery { get; set; }
        public int YearlyThe { get; set; }
        public int YearlyDay { get; set; }
        public int YearlyMonth { get; set; }

        public bool NoEnd { get; set; }
        public int EndAfter { get; set; }
        public DateTime EndDate { get; set; }
    }

    public enum RecurrencePattern
    {
        Daily,
        Weekly,
        Monthly,
        Yearly,
    }

    public class EventAccessModel
    {
        public bool IsVisible { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsOwner { get; set; }

        public bool CanInvite { get; set; }

        internal void SetIsOwner()
        {
            this.CanInvite = this.IsOwner = this.IsAdministrator = this.IsVisible = true;
        }

        internal void SetIsAdmin()
        {
            this.CanInvite = this.IsAdministrator = this.IsVisible = true;
        }
    }
}
