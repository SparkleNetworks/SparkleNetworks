
namespace Sparkle.Controllers
{
    using Sparkle.Entities.Networks;
    using Sparkle.Models;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks.Events;
    using Sparkle.UI;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using vCalendar.Mvc;

    public class CalendarController : LocalSparkleController
    {
        private static readonly DateTime maxDateForObsoleteKeyValidation = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// ICAL FILE to synchronize the network's calendar in you favorite calendar app.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="key"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public ActionResult Main(string username, string key, string displayName)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("The value cannot be empty", "username");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The value cannot be empty", "key");

            var me = this.SessionService.User;
            if (string.IsNullOrWhiteSpace(displayName) && me != null)
            {
                return this.RedirectToRoutePermanent("Calendars", new
                {
                    controller = "Calendar", action = "Main",
                    username = me.Login,
                    key = Keys.ComputeForCalendar(me.UserId),
                    displayName = Lang.T("AppName") + ".ics"
                });
            }

            var user = this.Services.People.GetActiveByLogin(username, Data.Options.PersonOptions.None);
            if (user == null)
            {
                return this.ResultService.NotFound("No such user");
            }

            // obsolete key validation method
            bool isAuthorized = false;
            bool isObsoleteKeyValidation = false;
            var now = DateTime.UtcNow;
            if (now < maxDateForObsoleteKeyValidation && key.Length > 20 && key.Length < 30)
            {
                isObsoleteKeyValidation = true;
                if (!Keys.ValidateForCalendar(user.UserId, key))
                {
                    isAuthorized = false;
                }
                else
                {
                    isAuthorized = true;
                }
            }

            // new key validation method
            if (!isAuthorized)
            {
                var validateRequest = new ValidateUserCalendarTokenRequest();
                validateRequest.ActingUserId = this.UserId;
                validateRequest.Token = key;
                var validateResult = this.Services.Events.ValidateUserCalendarToken(validateRequest);
                isAuthorized = validateResult.Succeed;
            }

            if (!isAuthorized)
            {
                return this.ResultService.NotFound("No such user");
            }

            var query = this.Services.Events.GetAllEventsRelatedToUserWithModel(user.Id);
            var events = query.ToList();
            var myevents = this.Services.EventsMembers.GetMyMembershipForEvents(events.Select(e => e.Id).ToArray(), user.Id);

            var vevents = new List<vCalendar.Event>();
            foreach (var e in events)
            {
                if (e.DateEvent == null)
                    continue;

                var vevt = new vCalendar.Event
                {
                    Class = (e.Visibility == EventVisibility.Public || e.Visibility == EventVisibility.External) ? vCalendar.EventClass.Public : vCalendar.EventClass.Private,
                    DateCreatedUtc = e.CreateDate.HasValue ? e.CreateDate.Value.ToUniversalTime() : now,
                    DateUpdatedUtc = e.CreateDate.HasValue ? e.CreateDate.Value.ToUniversalTime() : now,
                    DateStartUtc = e.DateEvent.ToUniversalTime(),
                    DateEndUtc = e.DateEndEvent.ToUniversalTime(),
                    Summary = e.Name,
                    Description = e.Description,
                    Location = e.Place != null ? (e.Place.Address != null ? e.Place.Address.ToString() : string.Empty) : string.Empty,
                    Status = e.DeleteDateUtc != null ? vCalendar.EventStatus.Cancelled : vCalendar.EventStatus.Confirmed,
                    Uid = "event-" + e.Id + "@sparklenetworks.net",
                    Url = new Uri(Lang.T("AppDomain") + "Events/" + e.Id, UriKind.Absolute),
                };
                var my = myevents.ContainsKey(e.Id) ? myevents[e.Id] : null;
                if (my != null)
                {
                    vevt.PartyStatus = ToPartyStatus(my.State);
                }
                else
                {
                    vevt.PartyStatus = vCalendar.PartyStatus.NeedsAction;
                }

                vevents.Add(vevt);
            }

            if (isObsoleteKeyValidation)
            {
                // remind users that they need to change the url
                // in order to use the new key
                var url = Lang.T("AppDomain") + "Help/Page/Changes/2017-04-02-ICalendar-URL.fr.md";
                var specialDate = maxDateForObsoleteKeyValidation.AddHours(-24D + 12D);
                var vevt = new vCalendar.Event
                {
                    Class = vCalendar.EventClass.Private,
                    DateCreatedUtc = now,
                    DateUpdatedUtc = now,
                    DateStartUtc = specialDate,
                    DateEndUtc = specialDate.AddHours(.5D),
                    Summary = "Attention requise. L'URL d'accès au calendrier du réseau " + this.Services.Lang.T("AppName") + " va changer",
                    Description = "L'URL d'accès au calendrier du réseau " + this.Services.Lang.T("AppName") + " va changer. Veuillez supprimer ce calendrier et l'ajouter à nouveau depuis le site web du réseau. Plus d'informations à cette adresse : " + url + "\r\n\r\n",
                    Location = string.Empty,
                    Status = vCalendar.EventStatus.Confirmed,
                    Uid = "event-721D9957-9C9B-4789-913A-E059173FC720@sparklenetworks.net",
                    Url = new Uri(url, UriKind.Absolute),
                };
                vevt.PartyStatus = vCalendar.PartyStatus.NeedsAction;
                vevents.Add(vevt);
            }

            var cal = new vCalendar.Calendar
            {
                Name = Lang.T("AppName"),
                Method = vCalendar.CalendarMethod.Publish,
                Scale = vCalendar.CalendarScale.Gregorian,
                ProductId = "-//SparkleNetworks//NONSGML Agenda Sparkle Networks V1.0//FR",
                OriginalUrl = new Uri(Lang.T("AppDomain") + Request.RawUrl.Substring(1), UriKind.Absolute),
                Events = vevents,
            };

            return new CalendarActionResult
            {
                Data = cal,
            };
        }

        private static vCalendar.PartyStatus ToPartyStatus(EventMemberState status)
        {
            switch (status)
            {
                case EventMemberState.IsInvited:
                    return vCalendar.PartyStatus.NeedsAction;
                case EventMemberState.HasAccepted:
                    return vCalendar.PartyStatus.Accepted;
                case EventMemberState.MaybeJoin:
                    return vCalendar.PartyStatus.Tentative;
                case EventMemberState.WontCome:
                    return vCalendar.PartyStatus.Declined;
                case EventMemberState.WantJoin:
                    return vCalendar.PartyStatus.Tentative;
                default:
                    return vCalendar.PartyStatus.Tentative;
            }
        }
    }
}
