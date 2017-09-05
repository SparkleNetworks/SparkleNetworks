
namespace Sparkle.Controllers
{
    using Sparkle.Data.Networks.Options;
    using Sparkle.Data.Options;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Models;
    using Sparkle.Models.Box;
    using Sparkle.Resources;
    using Sparkle.Services.Authentication;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Places;
    using Sparkle.Services.Networks.Team;
    using Sparkle.Services.Networks.Timelines;
    using Sparkle.Services.Networks.Users;
    using Sparkle.UI;
    using Sparkle.WebBase;
    using SrkToolkit;
    using SrkToolkit.Common.Validation;
    using SrkToolkit.Web;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using DomainModels = Sparkle.Services.Networks.Models.Profile;
    using Neutral = Sparkle.Entities.Networks.Neutral;

    /// <summary>
    /// This controller contains many popups.
    /// In the future, it would be better to put popups in a controller directly related to a feature.
    /// </summary>
    public class BoxController : LocalSparkleController
    {
        public ActionResult BoxContactCompany(int option)
        {
            return this.ResultService.NotFound();
        }

        [AuthorizeUser]
        public ActionResult BoxNumbers()
        {
            BoxNumbersModel model = new BoxNumbersModel();
            IList<Number> numbers = this.Services.Numbers.SelectAll();
            model.Numbers = numbers.Select(o => new NumberModel(o)).ToList();
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxDeviceEdit(string option)
        {
            BoxDeviceEditModel model = new BoxDeviceEditModel();

            Guid deviceId;
            if (!Guid.TryParse(option, out deviceId))
                return View(model);

            Device device = this.Services.Devices.Get(deviceId);
            if (device == null)
                return View(model);

            model.Success = true;
            model.Name = !string.IsNullOrEmpty(device.Name) ? device.Name : device.DeviceId.ToString();

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxDevicePlanning()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxPermissions(string option)
        {
            var me = this.SessionService.User;
            var contact = this.Services.People.SelectWithLogin(option);
            BoxPermissionsModel model = new BoxPermissionsModel();
            model.Person = new PeopleModel(this.Services, contact);
            model.Login = option;
            model.CompanyName = me.Company.Name;
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxSearch()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxLostItem()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxNotificationCenter()
        {
            BoxNotificationCenterModel model = new BoxNotificationCenterModel();

            // Demandes de contacts
            model.Contacts = this.Services.SeekFriends.SelectSeekFriendsByTargetId(this.UserId.Value).Select(o => new PeopleModel(this.Services, o, false)).ToList();

            // Messages
            Message lastMessage = this.Services.PrivateMessage.SelectLastReceivedMessage(this.UserId.Value);
            if (lastMessage != null)
            {
                model.HasLastMessage = true;
                MessageModel msg = new MessageModel(this.Services, lastMessage);
                if (!string.IsNullOrEmpty(msg.Subject))
                {
                    msg.Text = msg.Subject;
                }

                if (!string.IsNullOrEmpty(msg.Text))
                {
                    if (msg.Text.Length > 60)
                    {
                        msg.Text = msg.Text.Substring(0, 60) + "...";
                    }
                }

                model.LastMessage = msg;
            }

            IList<Activity> activities = this.Services.Activities.SelectFiveRecentActivitiesByUserId(this.UserId.Value);

            // 7 : invitation a un evement
            model.Events = activities.Where(o => o.Type == (int)DomainModels.ActivityType.EventInvitation)
                .Select(i => new NotificationCenterItemModel(this.Services, i, this.SessionService.User))
                .ToList();

            // 8 : invitation a un groupe
            model.Groups = activities.Where(o => o.Type == (int)DomainModels.ActivityType.NewGroupInvitation)
                .Select(i => new NotificationCenterItemModel(this.Services, i, this.SessionService.User))
                .ToList();

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxChooseContacts(string option)
        {
            BoxChooseContactsModel model = new BoxChooseContactsModel();
            if (string.IsNullOrEmpty(option)) return View(model);

            var me = this.SessionService.User;

            string type = option.Substring(0, 1).ToUpperInvariant();
            int id;
            if (int.TryParse(option.Substring(1), out id))
            {
                model.Valide = true;

                // fetch DATA!
                IList<User> people = this.Services.People.QueryActivePeople(PersonOptions.None)
                    .OrderByDescending(p => p.FirstName != null)
                    .ThenBy(p => p.LastName)
                    .ToList();

                foreach (var item in people)
                {
                    ////item.Picture = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = item.Login, size = "Small", });
                    item.Picture = this.Services.People.GetProfilePictureUrl(item, UserProfilePictureSize.Small, UriKind.Relative);
                }

                List<BoxChooseContactModel> Contacts = this.Services.Friend.GetContactsByUserId(me.Id).Select(o => new BoxChooseContactModel(o)).ToList();
                foreach (var contact in Contacts)
                {
                    foreach (var user in people)
                    {
                        if (contact.Login == user.Login)
                        {
                            user.IsFriendWithCurrentId = true;
                            break;
                        }
                    }
                }

                switch (type)
                {
                    case "G":
                        model.Title = "Inviter des " + Lang.T("PeopleLabel").ToLower() + " à rejoindre ce groupe";
                        // contact présent dans le groupe
                        var groupMembers = this.Services.GroupsMembers.SelectGroupMembers(id, GroupMemberStatus.None);
                        foreach (GroupMember groupMember in groupMembers)
                        {
                            foreach (User person in people)
                            {
                                if (groupMember.UserId == person.Id)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    break;
                                }
                            }
                        }
                        break;

                    case "E":
                        model.Title = "Inviter des " + Lang.T("PeopleLabel").ToLower() + " à l'événement";
                        // contact non présent dans l'evenement
                        var eventMembers = this.Services.EventsMembers.SelectEventMembers(id);
                        foreach (var eventMember in eventMembers)
                        {
                            foreach (User person in people)
                            {
                                if (eventMember.UserId == person.Id)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    break;
                                }
                            }
                        }
                        break;

                    case "L":
                        model.Title = "Inviter des contacts à déjeuner ici";
                        // Lunch propostion
                        break;

                    case "C":
                        model.Title = "Ajouter des " + Lang.T("PeopleLabel").ToLower() + " à vos contacts";
                        //var People = this.Services.People.SelectAll();
                        //Contacts = People.Select(o => new BoxChooseContactModel(o)).ToList();
                        List<BoxChooseContactModel> myContacts = this.Services.Friend.GetContactsByUserId(me.Id).Select(o => new BoxChooseContactModel(o)).ToList();
                        foreach (BoxChooseContactModel myContact in myContacts)
                        {
                            foreach (User person in people)
                            {
                                if (myContact.UserId == person.Id)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    break;
                                }
                            }
                        }
                        break;
                }
                ////model.Contacts = Contacts;
                model.People = people.Select(p => new BoxChooseContactModel(p)).ToList();
            }

            model.Type = option;
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxInvitePeoples(string option)
        {
            BoxChooseContactsModel model = new BoxChooseContactsModel();
            if (string.IsNullOrEmpty(option))
                return this.View(model);

            var me = this.SessionService.User;
            this.ViewBag.MayInviteAll = false;

            string type = option.Substring(0, 1).ToUpperInvariant();
            int id;
            if (int.TryParse(option.Substring(1), out id))
            {
                model.Valide = true;

                // fetch DATA!
                IList<User> people = this.Services.People.QueryActivePeople(PersonOptions.None)
                    .Where(o => o.Id != me.Id)
                    .OrderByDescending(p => p.FirstName != null)
                    .ThenBy(p => p.LastName)
                    .ToList();

                foreach (var item in people)
                {
                    ////item.Picture = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = item.Login, size = "Small", });
                    item.Picture = this.Services.People.GetProfilePictureUrl(item, UserProfilePictureSize.Small, UriKind.Relative);
                }

                List<BoxChooseContactModel> Contacts = this.Services.Friend.GetContactsByUserId(me.Id).Select(o => new BoxChooseContactModel(o)).ToList();
                foreach (var contact in Contacts)
                {
                    foreach (var user in people)
                    {
                        if (contact.Login == user.Login)
                        {
                            user.IsFriendWithCurrentId = true;
                            break;
                        }
                    }
                }

                Dictionary<int, string> reasonNotDisplay = new Dictionary<int, string>();
                var forceInviteAgainAccess = new NetworkAccessLevel[] { NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.SparkleStaff, };
                switch (type)
                {
                    case "G":
                        #region Groups
                        model.Title = "Inviter des " + Lang.T("PeopleLabel").ToLower() + " à rejoindre ce groupe";
                        // contact présent dans le groupe
                        var groupMembers = this.Services.GroupsMembers.SelectAllMembersFromGroup(id);
                        foreach (var groupMember in groupMembers)
                        {
                            foreach (var person in people)
                            {
                                if (me.Id == groupMember.UserId)
                                    break;
                                if (groupMember.UserId == person.Id)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    switch (groupMember.AcceptedStatus)
                                    {
                                        case GroupMemberStatus.JoinRequest:
                                            reasonNotDisplay[person.Id] = this.Services.GroupsMembers.IsAdmin(me.Id, groupMember.GroupId) ? " vous a envoyé une demande" : " attend une réponse de l'administrateur";
                                            break;
                                        case GroupMemberStatus.Invited:
                                            reasonNotDisplay[person.Id] = " demande en attente";
                                            break;
                                        case GroupMemberStatus.Accepted:
                                            person.IsFriendWithCurrentId = false;
                                            person.IsDisplayWithCurrentId = false;
                                            break;
                                        case GroupMemberStatus.JoinRejected:
                                            reasonNotDisplay[person.Id] = " n'a pas été accepté";
                                            break;
                                        case GroupMemberStatus.Kicked:
                                            reasonNotDisplay[person.Id] = " a été expulsé du groupe";
                                            break;
                                        case GroupMemberStatus.InvitationRejected:
                                            reasonNotDisplay[person.Id] = " a refusé l'invitation";
                                            break;
                                        case GroupMemberStatus.Left:
                                            reasonNotDisplay[person.Id] = " a quitté le groupe";
                                            if (me.NetworkAccess.HasAnyFlag(forceInviteAgainAccess))
                                            {
                                                person.IsDisplayWithCurrentId = false;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                        #endregion

                    case "E":
                        #region Events
                        this.ViewBag.MayInviteAll = this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff)/* || this.Services.EventsMembers.IsAdmin(this.UserId.Value, id)*/;

                        model.Title = "Inviter des " + Lang.T("PeopleLabel").ToLower() + " à l'événement";
                        // contact non présent dans l'evenement
                        var eventMembers = this.Services.EventsMembers.SelectEventMembers(id);
                        model.NonInvitedPeopleCount = this.Services.People.CountActive() - eventMembers.Count;
                        model.NonInvitedContactsCount = this.Services.Friend.GetActiveContactsCountExcept(this.UserId.Value, eventMembers.Select(o => o.UserId).ToArray());

                        var e = this.Services.Events.GetById(id, EventOptions.None);
                        groupMembers = null;
                        if (e.GroupId.HasValue)
                            groupMembers = this.Services.GroupsMembers.SelectAllMembersFromGroup(e.GroupId.Value);
                        foreach (var eventMember in eventMembers)
                        {
                            foreach (var person in people)
                            {
                                if (me.Id == eventMember.UserId)
                                    break;
                                if (e.Group != null && e.VisibilityValue == EventVisibility.Company && !groupMembers.Any(o => o.UserId == person.Id))
                                {
                                    person.IsDisplayWithCurrentId = false;
                                    person.IsFriendWithCurrentId = false;
                                }
                                else
                                    person.IsFriendWithCurrentId = true;
                                if (eventMember.UserId == person.Id)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    switch (eventMember.StateValue)
                                    {
                                        case EventMemberState.IsInvited:
                                            reasonNotDisplay[person.Id] = " en attente d'une réponse";
                                            break;
                                        case EventMemberState.HasAccepted:
                                            reasonNotDisplay[person.Id] = " a répondu : présent" + (person.GenderValue == NetworkUserGender.Female ? "e" : "");
                                            break;
                                        case EventMemberState.MaybeJoin:
                                            reasonNotDisplay[person.Id] = " a répondu : peut-être";
                                            break;
                                        case EventMemberState.WontCome:
                                            reasonNotDisplay[person.Id] = " a répondu : absent" + (person.GenderValue == NetworkUserGender.Female ? "e" : "");
                                            break;
                                        case EventMemberState.WantJoin:
                                            reasonNotDisplay[person.Id] = this.Services.EventsMembers.IsAdmin(me.Id, eventMember.EventId) ? " vous a envoyé une demande" : " attend une réponse de l'administrateur";
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                        #endregion

                    case "L":
                        model.Title = "Inviter des contacts à déjeuner ici";
                        // Lunch propostion
                        break;

                    case "C":
                        #region Contacts
                        model.Title = "Ajouter des " + Lang.T("PeopleLabel").ToLower() + " à vos contacts";
                        var mySeekFriends = this.Services.SeekFriends.SelectAllSeekFriendsRelativeToId(me.Id);
                        foreach (var item in mySeekFriends)
                        {
                            foreach (var person in people)
                            {
                                if (person.Id == item.TargetId && item.HasAccepted == false)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    reasonNotDisplay[person.Id] = " a ignoré votre demande";
                                }
                                else if (person.Id == item.TargetId && item.HasAccepted == null)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    reasonNotDisplay[person.Id] = " en attente d'une réponse";
                                }
                                else if (person.Id == item.SeekerId)
                                {
                                    person.IsDisplayWithCurrentId = true;
                                    reasonNotDisplay[person.Id] = " vous a envoyé une demande";
                                }
                                break;
                            }
                        }
                        break;
                        #endregion
                }
                
                var peopleQuery = people
                    .Select(p => new BoxChooseContactModel(p)
                    {
                        AddedReason = reasonNotDisplay.Where(o => o.Key == p.Id).SingleOrDefault().Value,
                    });
                if (type == "C")
                {
                    peopleQuery = peopleQuery.Where(p => !p.IsContact);
                }
                if (type == "E")
                {
                    peopleQuery = peopleQuery.Where(p => p.IsContact);
                }

                model.People = peopleQuery
                        .OrderBy(p => p.AddedReason)
                        .OrderByDescending(p => p.IsAdded)
                        .ToList();
            }

            model.Type = option;
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxPersonPicker(string option)
        {
            BoxChooseContactsModel model = new BoxChooseContactsModel();
            var me = this.SessionService.User;

            IList<User> people = this.Services.People.QueryActivePeople(PersonOptions.None)
                    .Where(o => o.Id != me.Id)
                    .OrderByDescending(p => p.FirstName != null)
                    .ThenBy(p => p.LastName)
                    .ToList();

            foreach (var item in people)
            {
                ////item.Picture = this.GetSimpleUrl("Data", "PersonPicture", "DataPicture", new { id = item.Login, size = "Small", });
                item.Picture = this.Services.People.GetProfilePictureUrl(item, UserProfilePictureSize.Small, UriKind.Relative);
            }

            var peopleQuery = people
                    .Select(p => new BoxChooseContactModel(p));

            model.People = peopleQuery
                    .OrderBy(p => p.AddedReason)
                    .OrderByDescending(p => p.IsAdded)
                    .ToList();

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxWelcome()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxMessage(string option)
        {
            MessageModel model = new MessageModel(this.Services);

            if (!string.IsNullOrEmpty(option))
            {
                var contact = this.Services.People.SelectWithLogin(option);
                if (contact != null)
                {
                    model.ToUserId = contact.Id;
                    model.ToUserLogin = contact.Login;
                    model.FirstName = contact.FirstName;
                    model.LastName = contact.LastName;
                }
            }

            return View(model);
        }

        [AuthorizeByNetworkAccess(NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff)]
        public ActionResult BoxAdminProcedures(string option)
        {
            var user = this.Services.People.SelectWithLogin(option);
            this.ViewBag.User = user;

            // emial change model
            var model = new AdminProceduresRequest();
            var changeEmailModel = this.Services.UserEmailChangeRequest.GetAdminProceduresRequestFromLogin(option);
            if (changeEmailModel != null)
            {
                this.ViewBag.CanChangeEmail = true;
                model = changeEmailModel;
            }
            else
            {
                this.ViewBag.CanChangeEmail = false;
            }

            // Revovery password link
            if (this.Services.People.IsActive(user))
            {
                this.ViewBag.CanDisplayRecoverLink = true;
                var result = this.Services.People.SendPasswordRecoveryEmail(user.Id, sendEmail: false);
                if (result.Succeed)
                {
                    this.ViewBag.PasswordRecoveryLink = result.PasswordResetLink;
                }

                var userMembership = this.Services.People.GetMembershipUser(user.UserId);
                this.ViewBag.PasswordIsLockedOut = userMembership.IsLockedOut;
                this.ViewBag.LastPasswordChangeDate = userMembership.LastPasswordChangedDate.AsUtc();
            }
            else
            {
                this.ViewBag.CanDisplayRecoverLink = false;
            }

            // Change company
            if (this.Services.AppConfiguration.Tree.Features.EnableCompanies)
            {
                this.ViewBag.CanChangeCompany = true;
                var userCompany = this.Services.Company.GetById(user.CompanyID);
                model.ActualCompany = userCompany.Name;
                model.Companies = this.Services.Company
                    .GetAllLight()
                    .Where(o => o.ID != user.CompanyID)
                    .OrderBy(o => o.Name)
                    .ToDictionary(o => o.ID, o => o.Name + (!o.IsApproved || !o.IsEnabled ? " (" + this.Services.Lang.T("Entreprise désactivée") + ")" : ""));
                model.CompaniesRights = Enum
                    .GetValues(typeof(CompanyAccessLevel))
                    .Cast<CompanyAccessLevel>()
                    .ToDictionary(o => o, o => EnumTools.GetDescription<CompanyAccessLevel>((CompanyAccessLevel)o, NetworksEnumMessages.ResourceManager));
            }
            else
            {
                this.ViewBag.CanChangeCompany = false;
            }

            return this.View(model);
        }

        [HttpPost]
        [AuthorizeByNetworkAccess(NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff)]
        public ActionResult BoxAdminProcedures(AdminProceduresRequest request)
        {
            if (this.IsAdminProceduresRequestValid(request.PostAction))
            {
                switch (request.PostAction)
                {
                    case AdminProceduresPostAction.ChangeEmail:
                        var result = this.Services.UserEmailChangeRequest.AddUserEmailChangeRequest(request, this.UserId.Value);
                        if (!result.Succeed)
                        {
                            var error = result.Errors.First();
                            if (error != null)
                                return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
                            else
                                return this.ResultService.JsonError("UnknwonError");
                        }

                        var userMembership = new AccountMembershipService().GetUser(request.Login);
                        var key = Keys.ComputeForAccount(userMembership.ProviderUserKey, userMembership.LastLoginDate);
                        var user = this.Services.People.SelectWithLogin(request.Login);
                        var pending = this.Services.UserEmailChangeRequest.SelectPendingRequestFromUserId(user.Id);
                        var emailToSend = new EmailAddress(pending.NewEmailAccountPart, pending.NewEmailTagPart, pending.NewEmailDomainPart);
                        this.Services.Email.SendEmailChangeRequest(user, emailToSend.Value, UrlTools.Compose(Request) + Url.RouteUrl("EmailChange", new { id = pending.Id, key = key }));

                        return this.ResultService.JsonSuccess(new { FirstName = user.FirstName });
                    case AdminProceduresPostAction.ChangeCompany:
                        var resultCompany = this.Services.People.ChangeUserCompany(request, this.UserId.Value);
                        if (!this.ValidateResult(resultCompany, MessageDisplayMode.None))
                        {
                            var error = resultCompany.Errors.First();
                            return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
                        }

                        user = this.Services.People.SelectWithLogin(request.Login);
                        return this.ResultService.JsonSuccess(new { FirstName = user.FirstName });
                    case AdminProceduresPostAction.None:
                    default:
                        throw new InvalidOperationException("Invalid PostAction type: " + request.PostAction);
                }
            }

            return View(request);
        }

        private bool IsAdminProceduresRequestValid(AdminProceduresPostAction action)
        {
            bool isValid = true;
            string[] modelKeys = new string[0];

            switch (action)
            {
                case AdminProceduresPostAction.ChangeEmail:
                    modelKeys = new string[] { "Email", "Remark" };
                    break;
                case AdminProceduresPostAction.ChangeCompany:
                    modelKeys = new string[0];
                    break;
                case AdminProceduresPostAction.None:
                default:
                    throw new InvalidOperationException("Invalid PostAction type: " + action);
            }

            foreach (var key in modelKeys)
            {
                if (this.ModelState.ContainsKey(key) && this.ModelState[key].Errors.Count > 0)
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }

        [AuthorizeUser]
        public ActionResult BoxRecurrence()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxExchange()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxExchangeEdit()
        {
            return View();
        }

        [AuthorizeUser]
        public ActionResult BoxCoffee(string option)
        {
            MessageModel model = new MessageModel(this.Services);

            if (!string.IsNullOrEmpty(option))
            {
                var contact = this.Services.People.SelectWithLogin(option);
                if (contact != null)
                {
                    model.ToUserId = contact.Id;
                    model.ToUserLogin = contact.Login;
                    model.FirstName = contact.FirstName;
                    model.LastName = contact.LastName;
                }
            }
            return View(model);
        }

        [AuthorizeUser]
        public ActionResult BoxProposals(string option)
        {
            if (string.IsNullOrEmpty(option) || option.Length < 2)
            {
                this.Response.StatusCode = 404;
                return this.View("NotFound");
            }

            string type = option.Substring(0, 1).ToUpperInvariant();
            string login = option.Substring(1).ToLowerInvariant();

            BoxProposalsModel model = new BoxProposalsModel();
            model.Type = type;
            model.CurrentDate = DateTime.Now.ToShortDateString();
            model.Hour = (short)DateTime.Now.Hour;
            model.Minute = (short)DateTime.Now.Minute;

            int[] mins = new int[] { 00, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };

            foreach (var min in mins)
            {
                if (mins.Contains(model.Minute))
                {
                    break;
                }
                else
                {
                    if (model.Minute == 59)
                    {
                        model.Hour++;
                        model.Minute = 0;
                    }
                    else
                    {
                        model.Minute++;
                    }
                }
            }

            switch (type)
            {
                case "C":
                    model.Title = "Proposer un café";
                    break;
                case "L":
                    model.Title = "Proposer un déjeuner";
                    if (DateTime.Now.Hour < 12)
                    {
                        model.Hour = 12;
                        model.Minute = 10;
                    }
                    else if (DateTime.Now.Hour < 13)
                    {
                        model.Hour = 13;
                        model.Minute = 0;
                    }
                    else
                    {
                        model.Hour = 19;
                        model.Minute = 30;
                    }
                    break;
                case "B":
                    model.Title = "Proposer une bière";
                    if (DateTime.Now.Hour < 19)
                    {
                        model.Hour = 19;
                        model.Minute = 0;
                    }
                    break;
                case "O":
                    model.Title = "Proposer autre chose...";
                    break;
            }

            MessageModel msg = new MessageModel(this.Services);

            if (!string.IsNullOrEmpty(login))
            {
                var contact = this.Services.People.SelectWithLogin(login);
                if (contact != null)
                {
                    msg.ToUserId = contact.Id;
                    msg.ToUserLogin = contact.Login;
                    msg.FirstName = contact.FirstName;
                    msg.LastName = contact.LastName;
                }
                model.Message = msg;
            }

            return View(model);
        }

        [AuthorizeUser]
        public ActionResult ModeratePublication(int option)
        {
            int id = option;
            if (!this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.ModerateNetwork, NetworkAccessLevel.SparkleStaff))
            {
                this.Response.StatusCode = 403;
                return this.View("Forbidden");
            }

            var item = this.Services.Wall.SelectWallItemById(id);
            if (item == null)
            {
                this.Response.StatusCode = 404;
                return this.View("NotFound");
            }

            var items = this.Services.Wall.NewGetByContext((byte)0, 0, this.UserId.Value, DateTime.MaxValue, itemOrList: id);
            var itemModel = WallModel.CreateTimelineListFromConveyor(this.Services, items, this.SessionService.User).First();

            var model = new ModerateWallModel(itemModel);

            if (item.ExtraTypeValue == TimelineItemExtraType.GoogleGroupImportedMessage && !string.IsNullOrEmpty(item.Extra))
            {
                var data = new Dictionary<string, string>();
                data.FromHttpHeaderText(item.Extra);
                model.IsImportedWithoutUser = data.ContainsKey("Owner") ? data["Owner"] == "undefined" : false;
                if (model.IsImportedWithoutUser)
                {
                    model.AvailableNewOwners = this.Services.People.GetAllLite();
                    model.OwnerChange = this.Services.Wall.GetOwnerChangePreview(item.Id);
                }
            }
/*
 * Type:GoogleGroupsImportedMessage  Owner:undefined  TopicUrl:https://groups.google.com/forum/#!topic/le-camping-brotherhood/vMsI_C2emmk  TopicId:vMsI_C2emmk  Subject:Infinit: Fundraising, Public Launch on Mac and Beta on Windows  
 * */

            return this.View(model);
        }

        [AuthorizeUser]
        public ActionResult ModerateTag(string option)
        {
            // 0: tagId, 1: groupId, 2: category
            char[] tmp = { '|' };
            var opt = option.Split(tmp);

            var group = this.Services.Groups.SelectGroupById(int.Parse(opt[1]));
            if (group == null)
            {
                this.Response.StatusCode = 404;
                return this.View("NotFound");
            }

            int error = 0;
            switch (opt[2])
            {
                case "S":
                    if (this.Services.Skills.GetById(int.Parse(opt[0])) == null ||
                        group.Skills.Where(o => o.SkillId == int.Parse(opt[0])).SingleOrDefault() == null)
                        error++;
                    break;
                case "I":
                    if (this.Services.Interests.GetById(int.Parse(opt[0])) == null ||
                        group.Interests.Where(o => o.InterestId == int.Parse(opt[0])).SingleOrDefault() == null)
                        error++;
                    break;
                case "R":
                    if (this.Services.Recreations.GetById(int.Parse(opt[0])) == null ||
                        group.Recreations.Where(o => o.RecreationId == int.Parse(opt[0])).SingleOrDefault() == null)
                        error++;
                    break;
            }
            if (error > 0)
            {
                this.Response.StatusCode = 404;
                return this.View("NotFound");
            }

            var model = new ModerateTagModel()
            {
                TagId = int.Parse(opt[0]),
                GroupId = int.Parse(opt[1]),
                Category = opt[2],
            };

            return this.View(model);
        }

        [AuthorizeByNetworkAccess(NetworkAccessLevel.ModerateNetwork | NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff)]
        public ActionResult ManageTag(string option)
        {
            var splitted = option.Split('-');
            int id;
            if (!int.TryParse(splitted[1], out id))
                return this.ResultService.NotFound();

            Skill skill = null;
            Recreation recreation = null;
            Interest interest = null;
            var model = new ManageTagModel();

            switch (splitted[0].ToLowerInvariant())
            {
                case "skill":
                    skill = this.Services.Skills.GetById(id);
                    if (skill == null)
                        return this.ResultService.NotFound();
                    model.Tag = new Sparkle.Services.Networks.Models.Tags.TagModel(skill);
                    model.Tag.Numbers = new Dictionary<string, int>();
                    model.Tag.Numbers["CompanyProfiles"] = this.Services.Skills.CountCompanyProfiles(id, false);
                    model.Tag.Numbers["ExtCompanyProfiles"] = this.Services.Skills.CountCompanyProfiles(id, true) - model.Tag.Numbers["CompanyProfiles"];
                    model.Tag.Numbers["UserProfiles"] = this.Services.Skills.CountUserProfiles(id, false);
                    model.Tag.Numbers["ExtUserProfiles"] = this.Services.Skills.CountUserProfiles(id, true) - model.Tag.Numbers["UserProfiles"];
                    model.Tag.Numbers["TimelineItems"] = this.Services.Skills.CountTimelineItems(id, false);
                    model.Tag.Numbers["ExtTimelineItems"] = this.Services.Skills.CountTimelineItems(id, true) - model.Tag.Numbers["TimelineItems"];
                    model.Tag.Numbers["Groups"] = this.Services.Skills.CountGroups(id, false);
                    model.Tag.Numbers["ExtGroups"] = this.Services.Skills.CountGroups(id, true) - model.Tag.Numbers["Groups"];
                    break;

                case "interest":
                    interest = this.Services.Interests.GetById(id);
                    if (interest == null)
                        return this.ResultService.NotFound();
                    model.Tag = new Sparkle.Services.Networks.Models.Tags.TagModel(interest);
                    break;

                case "recreation":
                    recreation = this.Services.Recreations.GetById(id);
                    if (recreation == null)
                        return this.ResultService.NotFound();
                    model.Tag = new Sparkle.Services.Networks.Models.Tags.TagModel(recreation);
                    break;

                default:
                    break;
            }

            model.Rename = new Services.Networks.Tags.RenameTagRequest
            {
                Type = model.Tag.Type,
                Id = model.Tag.Id,
                NewName = model.Tag.Name,
                MayApplyToAllNetworks = this.SessionService.User.NetworkAccess.HasAnyFlag(NetworkAccessLevel.NetworkAdmin, NetworkAccessLevel.SparkleStaff),
            };

            model.Merge = new Services.Networks.Tags.MergeTagRequest
            {
                Type = model.Tag.Type,
                Id = model.Tag.Id,
            };

            switch (splitted[0].ToLowerInvariant())
            {
                case "skill":
                    model.Merge.AvailableTags = this.Services.Skills.GetAll();
                    break;

                case "interest":
                    break;

                case "recreation":
                    break;

                default:
                    break;
            }

            return this.PartialView(model);
        }

        [AuthorizeByNetworkAccess(NetworkAccessLevel.ManageCompany | NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff)]
        public ActionResult BoxToggleCompany(string option)
        {
            var alias = option;
            var model = this.Services.Company.GetToggleCompanyRequest(alias);

            return this.View(model);
        }

        [HttpPost, AuthorizeByNetworkAccess(NetworkAccessLevel.ManageCompany | NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff)]
        public ActionResult BoxToggleCompany(ToggleCompanyRequest request)
        {
            request.CurrentUserId = this.UserId.Value;
   
            if (ModelState.IsValid)
            {
                var result = this.Services.Company.ToggleCompany(request);
                if (result.Succeed)
                    return this.ResultService.JsonSuccess();

                var error = result.Errors.FirstOrDefault();
                if (error != null)
                    return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
            }
            request = this.Services.Company.GetToggleCompanyRequest(request.CompanyAlias);

            return this.View(request);
        }

        [AuthorizeUser]
        public ActionResult BoxPlacePicker()
        {
            var model = new PlacePickerModel
            {
                UserRemoteAddress = this.Request.UserHostAddress,
            };
            model = this.Services.Places.GetPlacePickerModel(model);

            return this.View(model);
        }

        [AuthorizeByNetworkAccess(NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.ContentManager)]
        public ActionResult BoxNetworkRole(string option)
        {
            var login = option;
            var model = this.Services.People.GetEditNetworkRoleRequest(login);
            if (model == null)
                return this.ResultService.NotFound();

            return this.View(model);
        }

        [HttpPost, AuthorizeByNetworkAccess(NetworkAccessLevel.NetworkAdmin | NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.ContentManager)]
        public ActionResult BoxNetworkRole(EditNetworkRoleRequest request)
        {
            var result = this.Services.People.UpdateNetworkRole(request);
            if (!this.ValidateResult(result, MessageDisplayMode.None))
            {
                var error = result.Errors.First();
                return this.ResultService.JsonError(error.Code.ToString(), error.DisplayMessage);
            }

            return this.ResultService.JsonSuccess();
        }
    }
}
