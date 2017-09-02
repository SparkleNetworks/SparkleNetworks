
namespace Sparkle.Services.Main.Networks
{
    using Newtonsoft.Json;
    using Sparkle.Common;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.EmailUtility;
    using Sparkle.Entities.Networks;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure;
    using Sparkle.Services.EmailModels;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Main.EmailModels;
    using Sparkle.Services.Main.Internal;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Companies;
    using Sparkle.Services.Networks.EmailModels;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Tags;
    using Sparkle.Services.Networks.Objects;
    using Sparkle.Services.Networks.PrivateMessages;
    using Sparkle.Services.Networks.Subscriptions;
    using Sparkle.Services.Networks.Texts;
    using Sparkle.Services.Networks.Timelines;
    using Sparkle.Services.Objects;
    using Sparkle.UI;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Web;
    using Neutral = Sparkle.Entities.Networks.Neutral;
    using Regexes = System.Text.RegularExpressions;

    public class EmailService : ServiceBase, IEmailService, IDisposable
    {
        private const string FROMFIRSTNAME = "[FIRSTNAME]";
        private const string FROMLASTNAME = "[LASTNAME]";
        private const string EMAIL = "[EMAIL]";
        private const string EVENTNAME = "[EVENTNAME]";
        private const string EVENTDATE = "[DATE]";
        private const string MESSAGETEXT = "[TEXT]";
        private const string LINKURL = "[LIEN]";
        private const string REPLYTODOMAIN = "@sparklenetworks.net";
        private const string EMAILDOMAIN = "@sparklenetworks.net";

        private static readonly Regexes.Regex regexItemId = new Regexes.Regex("<a[^<>]*name=\"?[a-zA-Z0-9_]*Item([0-9]+)\"?[^<>]*>", Regexes.RegexOptions.Compiled);
        private static readonly Regexes.Regex regexPrivateMessageId = new Regexes.Regex("<a[^<>]*name=\"?[a-zA-Z0-9_]*PrivateMessageId([0-9]+)\"?[^<>]*>", Regexes.RegexOptions.Compiled);

        private static readonly object uidLock = new object();
        private static long uid;

        private bool disposed;

        private IEmailProvider classAProvider;
        private IEmailProvider classBProvider;
        private IEmailProvider classCProvider;

        [DebuggerStepThrough]
        internal EmailService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        /// <summary>
        /// Gets the email provider for emails of kind registration, password, invitation emails...
        /// </summary>
        public IEmailProvider ClassAProvider
        {
            get { return this.classAProvider ?? (this.classAProvider = this.CreateProviderByClass("Providers.Emails.ClassA")); }
        }

        /// <summary>
        /// Gets the email provider for emails of kind user notifications (new comment, messages...).
        /// </summary>
        public IEmailProvider ClassBProvider
        {
            get { return this.classBProvider ?? (this.classBProvider = this.CreateProviderByClass("Providers.Emails.ClassB")); }
        }

        /// <summary>
        /// Gets the email provider for emails of kind newsletters.
        /// </summary>
        public IEmailProvider ClassCProvider
        {
            get { return this.classCProvider ?? (this.classCProvider = this.CreateProviderByClass("Providers.Emails.ClassC")); }
        }
        /*
        [Obsolete("Use ClassASender, ClassBSender, ClassCSender")]
        private EmailContact NotificationSender
        {
            get { return new EmailContact("notification" + EMAILDOMAIN, this.Services.Lang.T("AppName")); }
        }

        [Obsolete("Use ClassASender, ClassBSender, ClassCSender")]
        private EmailContact InvitationSender
        {
            get { return new EmailContact("invitation" + EMAILDOMAIN, this.Services.Lang.T("AppName")); }
        }
        */
        private EmailContact ClassASender
        {
            get
            {
                var address = this.Services.AppConfiguration.Tree.Features.Emails.ClassA.SenderAddress;
                var name = this.Services.AppConfiguration.Tree.Features.Emails.ClassA.SenderName.NullIfEmpty() ?? this.Services.Lang.T("AppName");
                return new EmailContact(address, name);
            }
        }

        private EmailContact ClassBSender
        {
            get
            {
                var address = this.Services.AppConfiguration.Tree.Features.Emails.ClassB.SenderAddress;
                var name = this.Services.AppConfiguration.Tree.Features.Emails.ClassB.SenderName.NullIfEmpty() ?? this.Services.Lang.T("AppName");
                return new EmailContact(address, name);
            }
        }

        private EmailContact ClassCSender
        {
            get
            {
                var address = this.Services.AppConfiguration.Tree.Features.Emails.ClassC.SenderAddress;
                var name = this.Services.AppConfiguration.Tree.Features.Emails.ClassC.SenderName.NullIfEmpty() ?? this.Services.Lang.T("AppName");
                return new EmailContact(address, name);
            }
        }

        private EmailContact ReplyToAdress
        {
            get
            {
                if (this.Services.AppConfiguration.Tree.Features.Timeline.EmailReply.IsEnabled)
                {
                    if (this.Services.AppConfiguration.Tree.Features.EmailReply.InboundEmailAddress != null)
                    {
                        var name = this.Services.AppConfiguration.Tree.Features.Emails.ClassB.SenderName.NullIfEmpty()
                            ?? this.Services.Lang.T("AppName");
                        var address = this.Services.AppConfiguration.Tree.Features.EmailReply.InboundEmailAddress;
                        return new EmailContact(address, name);
                    }
                    else
                    {
                        return this.ClassBSender;
                    }
                }
                else
                {
                    return this.ClassBSender;
                }
            }
        }

        [Obsolete]
        public static string GetBody(IServiceFactory services, string template, Dictionary<string, string> dictionnary)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<title></title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style='margin:0; font-family:Arial'>");
            sb.AppendLine("<div style='margin-bottom:20px; padding:5px 10px; background:#3f3f3f;'><a href='" + Lang.T("AppDomain") + "' style='color:#fff; text-decoration:none'>" + services.Lang.T("AppName") + "</a></div>");
            sb.AppendLine("<div style='font-size:13px'>");

            switch (template)
            {
                case EmailTemplates.ProposeEat:
                    sb.AppendLine("Bonjour [CONTACTFIRSTNAME],<br />");
                    sb.AppendLine("<br />");
                    sb.AppendLine("<a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='[PROFILE]'>[FIRSTNAME] [LASTNAME]</a> vous propose de déjeuner à <a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='[PLACEURL]'>[PLACENAME]</a>.<br />");
                    sb.AppendLine("<br />");
                    sb.AppendLine("Pour accepter ou proposer un autre lieu <a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='" + services.Lang.T("AppDomain") + "#lunch'>cliquez ici</a>.<br />");
                    sb.AppendLine("<br />");
                    sb.AppendLine("[OTHERS]<br />");
                    sb.AppendLine("<br />");
                    sb.AppendLine("Bonne journ�e,<br />");
                    sb.AppendLine(services.Lang.T("AppName"));
                    break;
                case EmailTemplates.Feedback:
                    sb.AppendLine("Bonjour,<br /><br />");
                    sb.AppendLine("Vous avez reçu un nouveau commentaire de <a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='[CONTACTURL]'>[CONTACTFIRSTNAME] [CONTACTLASTNAME]</a> concernant l'utilisation du site " + services.Lang.T("AppName") + ".<br /><br />");
                    sb.AppendLine("[CONTACTFIRSTNAME] a écrit : [COMMENT]<br /><br />");
                    sb.AppendLine("Merci,<br />");
                    sb.AppendLine(services.Lang.T("AppName"));
                    break;
                case EmailTemplates.RegisterRequest:
                    sb.AppendLine("Bonjour Kevin,<br /><br />");
                    sb.AppendLine("[EMAIL] demande à rejoindre " + services.Lang.T("AppName") + ".<br /><br />");
                    sb.AppendLine("Merci,<br />");
                    sb.AppendLine(services.Lang.T("AppName"));
                    break;
                case EmailTemplates.Weside:
                    sb.AppendLine("Bonjour,<br /><br />");
                    sb.AppendLine("<a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='[CONTACTURL]'>[CONTACTFIRSTNAME] [CONTACTLASTNAME]</a> vient de s'inscrire sur " + services.Lang.T("AppName") + ".<br /><br />");
                    sb.AppendLine("Merci,<br />");
                    sb.AppendLine(services.Lang.T("AppName"));
                    break;
                case EmailTemplates.Contact:
                    sb.AppendLine("Bonjour,<br /><br />");
                    sb.AppendLine("Nouveau message via le formulaire de contact<br /><br />");
                    sb.AppendLine("De : [NAME]<br />");
                    sb.AppendLine("Email : [EMAIL]<br />");
                    sb.AppendLine("Message : [MESSAGE]<br />");
                    sb.AppendLine("<br />");
                    sb.AppendLine("Merci,<br />");
                    sb.AppendLine(services.Lang.T("AppName"));
                    break;
                case EmailTemplates.MailChimpReport:
                    sb.AppendLine("Bonjour,<br /><br />");
                    sb.AppendLine("[REPORT]");
                    sb.AppendLine("Merci,<br />");
                    sb.AppendLine("L'équipe.<br />");
                    break;

                default:
                    throw new NotSupportedException("This method does not support sending the email template '"+template+"'");
            }

            sb.AppendLine("</div>");
            sb.AppendLine("<div style='font-size:11px; color:#bbb'><br><br>");
            sb.AppendLine("Le message a été envoyé à <span style='color:" + services.Lang.T("AccentColor") + ";'>[EMAIL]</span>. Si vous ne souhaitez plus recevoir ces messages via votre adresse électronique, vous pouvez modifier <a style='text-decoration:none; color:" + services.Lang.T("AccentColor") + ";' href='" + services.Lang.T("AppDomain") + "Account/Choices'>vos préférences</a>.");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            if (dictionnary != null)
            {
                foreach (KeyValuePair<string, string> s in dictionnary)
                {
                    sb.Replace(s.Key, s.Value);
                }
            }

            return sb.ToString();
        }

        public void SendInvitation(User inviter, Invited invited)
        {
            string invitationKey = invited.Code.ToString();
            string email = invited.Email;
            
            int i = email.IndexOf("@");
            string mailDomain = email.Substring(i + 1);


            Guid code;
            if (Guid.TryParse(invitationKey, out code))
            {
            }

            Invited invite = this.Services.Invited.GetByInvitationKey(code);
            IList<User> coworkers = new List<User>();

            if (invite != null)
            {
                coworkers = this.Services.People.SelectFromCompany(invite.CompanyId);
            }

            var model = new InviteEmailModel(email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = "Rejoindre " + this.Services.Lang.T("AppName");
            model.Contact = inviter;
            ////model.Contact.Picture = this.Services.Lang.T("AppDomain") + "/Data/PersonPicture/" + model.Contact.Username;
            model.Contact.Picture = this.Services.People.GetProfilePictureUrl(model.Contact, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute);
            model.InvitationUrl = this.Services.Lang.T("AppDomain") + "Account/Register/" + invitationKey;
            model.UnregisterUrl = this.Services.Lang.T("AppDomain") + "Account/Unregister/" + invitationKey;

            int peopleCount = this.Services.People.CountAll();
            int companiesCount = this.Services.Company.Count();

            model.Infos = ""; // this.Services.Lang.T("AppName") + " compte " + peopleCount + " personnes parmis " + companiesCount + " entreprises.";
            model.Network = this.Services.Network;

            if (coworkers.Count > 2)
            {
                model.Infos = coworkers.Count + " collègues sont présents. " + model.Infos;
            }

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("Invite", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(invited), inviter.FirstName + " " + inviter.LastName + " vous invite à rejoindre " + this.Services.Lang.T("AppName"), body);

            this.SendMail(this.ClassAProvider, "SendInvitation", message);
        }

        public void SendRegistred(User me, string message = null)
        {
            var model = new RegisterEmailModel(me.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = "Bienvenue";
            model.FirstName = me.FirstName;
            model.Login = me.Login;
            model.Message = message;

            this.SendRegistered(model);
        }

        public void SendRegistred(Sparkle.Services.Networks.Models.UserModel me, string message = null)
        {
            var model = new RegisterEmailModel(me.Email.OriginalString, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = "Bienvenue";
            model.FirstName = me.FirstName;
            model.Login = me.Username;
            model.Message = message;

            this.SendRegistered(model);
        }

        public void SendMailChimpReport(string report)
        {
            Dictionary<string, string> dictionnary = new Dictionary<string, string>
            {
                {"[REPORT]", report.Replace("\n", "<br />\n")}
            };

            string body = GetBody(this.Services, EmailTemplates.MailChimpReport, dictionnary);
            var message = this.CreateMessage(this.Services, this.ClassBSender, new EmailContact("beta@sparkleproject.net"), "Rapport d'envoi mailchip", body);

            this.SendMail(this.ClassAProvider, "SendMailChimpReport", message);
        }
        /*
         * See work item 504
         * 
        public void SendTimelineItem(int recipientUserId, int timelineItemId, int? timelineItemCommentId)
        {
            var model = this.Services.Wall.GetModelById(timelineItemId);
        }
        */
        public void SendNotification(User postedBy, User contact, TimelineItem item, Group group = null, Event @event = null, TimelineItemComment comment = null, NotificationType? notifType = null)
        {
            string title, subject, intro, linkName, linkUrl;

            if (!this.Services.People.IsActive(contact))
                return;

            var culture = this.Services.People.GetCulture(contact);
            if (group != null)
            {
                title = group.Name;
                if (comment == null)
                    ////intro = subject = " vient de poster un message dans le groupe " + group.Name;
                    intro = subject = this.Services.Lang.T(culture, "{0} vient de poster un message dans le groupe {1}", postedBy.FullName, group.Name);
                else
                    ////intro = subject = " vient de commenter un message dans le groupe " + group.Name;
                    intro = subject = this.Services.Lang.T(culture, "{0} vient de commenter un message dans le groupe {1}", postedBy.FullName, group.Name);
                linkUrl = "Group/" + group.Id;
            }
            else if (@event != null)
            {
                title = @event.Name;
                if (comment == null)
                    ////intro = subject = " vient de poster un message dans l'évènement " + @event.Name;
                    intro = subject = this.Services.Lang.T(culture, "{0} vient de poster un message dans l'évènement {1}", postedBy.FullName, @event.Name);
                else
                    ////intro = subject = " vient de commenter un message dans l'évènement " + @event.Name;
                    intro = subject = this.Services.Lang.T(culture, "{0} vient de commenter un message dans l'évènement {1}", postedBy.FullName, @event.Name);
                linkUrl = "Event/" + @event.Id;
            }
            else
            {
                title = "";
                subject = " : " + item.Text.TrimTextRight(32);
                if (comment == null)
                    ////intro = " vient de publier dans le fil d'actualité";
                    intro = this.Services.Lang.T(culture, "{0} vient de publier dans le fil d'actualité", postedBy.FullName);
                else
                    ////intro = " vient de commenter dans le fil d'actualité";
                    intro = this.Services.Lang.T(culture, "{0} vient de commenter dans le fil d'actualité", postedBy.FullName);
                linkUrl = "Ajax/Item/" + item.Id;
            }

            subject = postedBy.FullName + " : " + item.Text.TrimTextRight(32);
            linkName = this.Services.Lang.T(culture, "Afficher la conversation");

            var itemModel = new BasicTimelineItemModel();
            itemModel.Fill(this.Services, item, item.Comments, evt: @event, group: group);
            itemModel.IsUserAuthorized = this.Services.Wall.IsVisible(item, contact);

            if (comment == null)
            {
                itemModel.IsHighlighted = true;
            }
            else if (itemModel.Items != null)
            {
                foreach (var child in itemModel.Items)
                {
                    if (child.Id == comment.Id)
                    {
                        child.IsHighlighted = true;
                    }
                }
            }
            
            var model = new PublicationEmailModel(contact.Email, itemModel.ForegroundColor ?? this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = this.Services.Lang.T(culture, "Nouvelle publication"),
                TimelineItem = itemModel,
                LinkName = linkName,
                LinkUrl = this.Services.Lang.T(culture, "AppDomain") + linkUrl,
                IsReplyToEmailEnabled = this.Services.AppConfiguration.Tree.Features.Timeline.EmailReply.IsEnabled,
            };

            model.NotificationType = notifType;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, notifType);

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("Publication", null, culture, this.Services.People.GetTimezone(contact), model);
            
            var message = this.CreateMessage(this.Services, this.ReplyToAdress, EmailContact.Create(contact), subject, body);
            if (this.Services.AppConfiguration.Tree.Features.Timeline.EmailReply.IsEnabled)
            {
                message.ReplyToList.Add(this.ReplyToAdress.ToMailAddress());
            }

            this.SendMail(this.ClassBProvider, "SendNotification.NewPublication", message);
        }

        /// <summary>
        /// Sends contact request notification.
        /// </summary>
        /// <param name="currentUserId">The current user id.</param>
        /// <param name="item">The contact.</param>
        public void SendContactRequest(int currentUserId, User item)
        {
            this.CheckUserIsActive(item);

            this.Services.People.OptionsList = new List<string> { "Job", "Company" };
            User me = this.Services.People.SelectWithId(currentUserId);
            var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(me.Id);

            var model = new ContactRequestEmailModel(item.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = null;
            model.FirstName = item.FirstName;
            model.Contact = new UserModel(me, profileFields);
            model.ContactJob = me.Job;
            model.ContactCompany = me.Company;
            model.Contact.Picture = this.Services.People.GetProfilePictureUrl(me, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute);

            model.NotificationType = NotificationType.ContactRequest;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(item.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(item.Id, NotificationType.ContactRequest);

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("ContactRequest", null, this.Services.People.GetCulture(item), this.Services.People.GetTimezone(item), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(item), me.FirstName + " " + me.LastName + " souhaite vous ajouter à ses contacts", body);

            this.SendMail(this.ClassBProvider, "SendNotification.SeekFriends", message);
        }

        public void SendContactRequestAccepted(int userId, User item)
        {
            this.Services.People.OptionsList = new List<string> { "Job", "Company", };
            User me = this.Services.People.SelectWithId(userId);
            var profileFields = this.Services.ProfileFields.GetUserProfileFieldsByUserId(me.Id);

            var model = new ContactRequestEmailModel(item.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = null;
            model.FirstName = item.FirstName;
            model.Contact = new UserModel(me, profileFields);
            model.ContactJob = me.Job;
            model.ContactCompany = me.Company;

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("ContactRequestAccepted", null, this.Services.People.GetCulture(item), this.Services.People.GetTimezone(item), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(item), me.FirstName + " " + me.LastName + " à accepté votre demande", body);

            this.SendMail(this.ClassBProvider, "SendNotification.SeekFriends", message);
        }

        /// <summary>
        /// Sends invitation to event notification.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <param name="contact">The contact.</param>
        /// <param name="event">The @event.</param>
        public void SendNotification(User me, User contact, Event @event)
        {
            this.CheckUserIsActive(contact);
            var timezone = this.Services.People.GetTimezone(contact);
            var eventModel = new EventModel(@event);

            var model = new EventEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Title = null;
            model.FirstName = contact.FirstName;
            model.ContactFirstName = me.FirstName;
            model.ContactLastName = me.LastName;
            model.ProfileUrl = this.Services.Lang.T("AppDomain") + "Person/" + me.Login;
            model.EventName = @event.Name;
            model.EventDate = timezone.ConvertFromUtc(eventModel.DateEventUtc);
            model.EventDescription = @event.Description;
            model.EventUrl = this.Services.Lang.T("AppDomain") + "Event/" + @event.Id;
            model.EventReponseYesUrl = this.Services.Lang.T("AppDomain") + "Events/EventResponse/" + @event.Id + "?response=1";
            model.EventReponseMaybeUrl = this.Services.Lang.T("AppDomain") + "Events/EventResponse/" + @event.Id + "?response=2";
            model.EventReponseNoUrl = this.Services.Lang.T("AppDomain") + "Events/EventResponse/" + @event.Id + "?response=3";

            model.NotificationType = NotificationType.EventInvitation;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, NotificationType.EventInvitation);

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("Event", null, this.Services.People.GetCulture(contact), timezone, model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(contact), me.FirstName + " vous invite à l'évènement \"" + @event.Name + "\"", body);

            this.SendMail(this.ClassBProvider, "SendNotification.InvitedToJoinEvent", message);
        }

        /// <summary>
        /// Sends the group email notification.
        /// </summary>
        /// <param name="inviter">Me.</param>
        /// <param name="invited">The contact.</param>
        /// <param name="group">The group.</param>
        public void SendNotification(User inviter, User invited, Group group)
        {
            this.CheckUserIsActive(invited);

            var model = new GroupEmailModel(invited.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = null,
                FirstName = invited.FirstName,
                ContactFirstName = inviter.FirstName,
                ContactLastName = inviter.LastName,
                ProfileUrl = this.Services.Lang.T("AppDomain") + "Person/" + inviter.Login,
                GroupName = group.Name,
                GroupDescription = group.Description,
                GroupPicture = this.Services.Groups.GetProfilePictureUrl(group, CompanyPictureSize.Medium, UriKind.Absolute),
                GroupUrl = this.Services.Lang.T("AppDomain") + "Group/" + group.Id
            };

            model.NotificationType = NotificationType.EventInvitation;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(invited.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(invited.Id, NotificationType.EventInvitation);

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("Group", null, this.Services.People.GetCulture(invited), this.Services.People.GetTimezone(invited), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(invited), inviter.FirstName + " vous invite à rejoindre le groupe \"" + group.Name + "\"", body);

            this.SendMail(this.ClassBProvider, "SendNotification.InvitedToJoinGroup", message);
        }

        public void SendProposeALunch(User me, User contact, Place place)
        {
            var model = new LunchEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.FirstName = contact.FirstName;
            model.ContactFirstName = me.FirstName;
            model.ContactLastName = me.LastName;
            model.ProfileUrl = this.Services.Lang.T("AppDomain") + "Person/" + me.Login;

            model.PlaceName = place.Name;
            model.PlaceUrl = this.Services.Lang.T("AppDomain") + "Place/" + place.Alias;
            model.LunchReponseYesUrl = this.Services.Lang.T("AppDomain") + "Events/LunchResponse/" + place.Id + "?response=1";
            model.LunchReponseOtherUrl = this.Services.Lang.T("AppDomain") + "Events/LunchResponse/" + place.Id + "?response=2";

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("Event", null, this.Services.People.GetCulture(contact), this.Services.People.GetTimezone(contact), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(contact), me.FirstName + " " + me.LastName + " vous propose de déjeuner à " + place.Name, body);

            this.SendMail(this.ClassBProvider, "SendNotification.InvitedToJoinEvent", message);
        }

        public void SendNewsletter(WeeklyMailSubscriber contact, DateTime start, NotificationFrequencyType frequency)
        {
            var model = new NewsletterEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);
            model.Initialize(this.Services);

            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.UserId, null);
            if (frequency == NotificationFrequencyType.Daily)
            {
                model.NotificationType = NotificationType.DailyNewsletter;
                model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.UserId, NotificationType.DailyNewsletter);
            }
            else if (frequency == NotificationFrequencyType.Weekly)
            {
                model.NotificationType = NotificationType.Newsletter;
                model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.UserId, NotificationType.Newsletter);
            }
            else
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");

            var body = this.MakeNewsletter(contact, start, frequency, model);

            if (body != null)
            {
                var message = this.CreateMessage(this.Services, this.ClassCSender, new EmailContact(contact.Email, contact.FirstName + " " + contact.LastName), model.Title, body);
                // no logging for this one because Sparkle.Commands already logs stuff
                this.SendMail(this.ClassCProvider, "SendNewsletter", message, logging: false);

                // Tracking Send
                Neutral.StatsCounter counter;
                if (frequency == NotificationFrequencyType.Daily)
                    counter = Neutral.StatsCounter.WellKnown.DailyNewsletter.Send;
                else if (frequency == NotificationFrequencyType.Weekly)
                    counter = Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Send;
                else
                    throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");

                var ct = this.Services.StatsCounters.GetCounter(counter.Category, counter.Name);
                if (ct == null)
                    throw new InvalidOperationException("Missing StatCounter '" + counter.Category + "'/'" + counter.Name + "' for SendNewsletter");
                if (ct.Id < 1)
                    throw new InvalidOperationException("StatCounter '" + counter.Category + "'/'" + counter.Name + "' for SendNewsletter has no ID");

                var hit = new StatsCounterHitLink(ct, start.ToString("yyy-MM-dd"), contact.UserId > 0 ? contact.UserId : default(int?), this.Services.NetworkId);

                try
                {
                    this.Services.StatsCounters.Hit(hit);
                }
                catch (Exception ex)
                {
                    var errorMessage = "StatCounter hit failed with counter '" + counter + "' -> '" + ct + "'. Link is '" + hit + "'. NetworkId is " + this.Services.NetworkId + " and networks are '" + string.Join(", ", this.Repo.Networks.GetAll(NetworkOptions.None)) + "'";
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
            else
            {
                this.Services.Logger.Verbose("EmailService.Newsletter", ErrorLevel.Success, "Newsletter " + frequency + " on " + start + " is empty for " + contact + "");
            }
        }

        public string MakeNewsletter(WeeklyMailSubscriber contact, DateTime start, NotificationFrequencyType frequency, NewsletterEmailModel model)
        {
            if (frequency == NotificationFrequencyType.Weekly && start.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("Parameter start should be a monday. ", "start");
            }
            if (start.Hour > 8)
            {
                throw new ArgumentException("Parameter start should be less than 8AM. ", "start");
            }

            // date ranges
            DateTime past, future, doubleFuture;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                past = start.AddDays(-7D);
                future = start.AddDays(7D);
                doubleFuture = future.AddDays(7D);
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                past = start.AddDays(-1D);
                future = start.AddDays(1D);
                doubleFuture = future.AddDays(1D);
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            int usefullInfos = 0;

            // new companies
            var comps = this.Services.Company.SelectByApprovedDateRange(past, start);
            if (comps.Count > 0)
            {
                usefullInfos++;
            }

            // user culture and timezone
            UserPoco user = null;
            CultureInfo culture = null;
            TimeZoneInfo tz = null;
            if (contact.UserId > 0)
            {
                model.contactRequests = this.Services.SeekFriends.SelectSeekFriendsByTargetId(contact.UserId).Count;
                user = this.Services.People.GetLiteById(contact.UserId, u => new UserPoco
                {
                    Id = u.Id,
                    Culture = u.Culture,
                    Timezone = u.Timezone,
                });
                if (user != null)
                {
                    culture = this.Services.People.GetCulture(user.Culture);
                    tz = this.Services.People.GetTimezone(user.Timezone);
                }
            }

            culture = culture ?? this.Services.DefaultCulture;
            tz = tz ?? this.Services.Context.Timezone;

            #region Derniers inscrits

            bool hasNewReistrants = false;
            hasNewReistrants = true;
            DateTime minRegistrantsDate = past;
            int newRegistrants = this.Services.Wall.CountRegistrantsInDateRange(past, start);
            ////IList<NewRegistrant> lastRegistrants = this.Services.Wall.GetLastRegistrants(4, minRegistrantsDate).Select(u => new NewRegistrant
            IList<NewRegistrant> lastRegistrants = this.Services.Wall
                .GetRegistrantsInDateRange(4, past, start)
                .Select(u => new NewRegistrant
                {
                    Name = u.PostedBy.FirstName + " " + u.PostedBy.LastName,
                    Login = u.PostedBy.Username,
                    JobName = (u.PostedBy.Job != null ? u.PostedBy.Job.Libelle : null),
                    CompanyName = u.PostedBy.Company.Name,
                    ////PictureUrl = this.Services.Lang.T("AppDomain") + "Data/PersonPicture/" + u.PostedBy.Username,
                    PictureUrl = this.Services.People.GetProfilePictureUrl(u.PostedBy.Username, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute),
                })
                .ToList();
            hasNewReistrants = lastRegistrants.Count > 0;
            if (newRegistrants > 0 || lastRegistrants.Count > 0)
            {
                usefullInfos++;
            }

            #endregion

            // Evenement de la semaine a venir
            bool hasEvents = false;
            IList<Event> nEvents;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                nEvents = this.Services.Events.GetAllEventsRelatedToUser(contact.UserId, start, future);
                //nEvents = this.Services.Events.GetWeekEvents(start);
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                nEvents = this.Services.Events.GetAllEventsRelatedToUser(contact.UserId, start, future);
                //nEvents = this.Services.Events.GetEvents(start, start.AddDays(2D));
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            if (nEvents.Count > 0)
            {
                usefullInfos++;
                hasEvents = true;
                foreach (Event nEvent in nEvents)
                {
                    if (nEvent.Description != null && nEvent.Description.Length > 350)
                    {
                        nEvent.Description = nEvent.Description.Substring(0, 350) + "...";
                    }
                }
            }


            #region Autres évènements de la semaine suivante

            bool hasOtherEvents = false;
            IList<Event> nOtherEvents;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                nOtherEvents = this.Services.Events.GetAllEventsRelatedToUser(contact.UserId, future, doubleFuture)
                    .Where(o => !nEvents.Select(i => i.Id).ToArray().Contains(o.Id) && o.DeleteDateUtc == null)
                    .ToList();
                //nOtherEvents = this.Services.Events.GetWeekEvents(start.AddDays(7D));
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                nOtherEvents = this.Services.Events.GetAllEventsRelatedToUser(contact.UserId, future, doubleFuture)
                    .Where(o => !nEvents.Select(i => i.Id).ToArray().Contains(o.Id) && o.DeleteDateUtc == null)
                    .ToList();
                //nOtherEvents = this.Services.Events.GetEvents(start.AddDays(2D), start.AddDays(7D));
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            if (nOtherEvents.Count > 0)
            {
                hasOtherEvents = true;
                foreach (Event nEvent in nOtherEvents)
                {
                    if (nEvent.Description != null && nEvent.Description.Length > 200)
                    {
                        nEvent.Description = nEvent.Description.Substring(0, 200) + "...";
                    }
                }
            }

            #endregion

            #region Publications des entreprises & main

            bool hasPeoplePublications = false;
            bool hasCompaniesPublications = false;
            bool hasPartnersPublications = false;
            IList<TimelineItem> publications;
            IList<TimelineItem> publicationsPeoples = null;
            IList<TimelineItem> publicationsPartners = null;
            var companyTimelineItems = new BasicTimelineItemModel
            {
                IsRootNode = true,
                Items = new List<BasicTimelineItemModel>(),
            };
            var peopleTimelineItems = new BasicTimelineItemModel
            {
                IsRootNode = true,
                Items = new List<BasicTimelineItemModel>(),
            };
            var partnerTimelineItems = new BasicTimelineItemModel
            {
                IsRootNode = true,
                Items = new List<BasicTimelineItemModel>(),
            };

            if (frequency == NotificationFrequencyType.Weekly)
            {
                if (this.Services.AppConfiguration.Tree.Features.Newsletter.ShowMainTimeline)
                    publicationsPeoples = this.Services.Wall.SelectPeoplePublicationsInDateRange(past, start);

                if (this.Services.AppConfiguration.Tree.Features.PartnerResources.IsEnabled)
                    publicationsPartners = this.Services.Wall.SelectPartnersPublicationsInDateRange(past, start);
                
                publications = this.Services.Wall.SelectCompaniesPublicationsInDateRange(past, start);
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                if (this.Services.AppConfiguration.Tree.Features.Newsletter.ShowMainTimeline)
                    publicationsPeoples = this.Services.Wall.SelectPeoplePublicationsInDateRange(past, start);
                
                if (this.Services.AppConfiguration.Tree.Features.PartnerResources.IsEnabled)
                    publicationsPartners = this.Services.Wall.SelectPartnersPublicationsInDateRange(past, start);

                publications = this.Services.Wall.SelectCompaniesPublicationsInDateRange(past, start);
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            if (publicationsPeoples != null &&  publicationsPeoples.Count > 0)
            {
                usefullInfos++;
                hasPeoplePublications = true;
                foreach (TimelineItem publication in publicationsPeoples)
                {
                    if (this.Services.People.IsActive(publication.PostedBy))
                    {
                        var item1 = new BasicTimelineItemModel();
                        item1.Fill(this.Services, publication, null);
                        item1.IsUserAuthorized = true;
                        peopleTimelineItems.Items.Add(item1);
                    }
                }
            }

            if (publications != null && publications.Count > 0)
            {
                usefullInfos++;
                hasCompaniesPublications = true;
                foreach (TimelineItem publication in publications)
                {
                    Company comp;
                    if (publication.CompanyId.HasValue && (comp = this.Services.Company.GetById(publication.CompanyId.Value)) != null && this.Services.Company.IsActive(comp))
                    {
                        var item1 = new BasicTimelineItemModel();
                        item1.Fill(this.Services, publication, null, company: comp);
                        item1.IsUserAuthorized = true;
                        companyTimelineItems.Items.Add(item1);
                    }
                }
            }

            if (publicationsPartners != null && publicationsPartners.Count > 0)
            {
                usefullInfos++;
                hasPartnersPublications = true;
                foreach (var publication in publicationsPartners)
                {
                    PartnerResource partner = null;
                    if (publication.PartnerResourceId.HasValue && (partner = this.Services.Repositories.PartnerResources.GetById(publication.PartnerResourceId.Value)) != null && partner.DateDeletedUtc == null)
                    {
                        var item1 = new BasicTimelineItemModel();
                        item1.Fill(this.Services, publication, null, partnerResource: partner);
                        item1.IsUserAuthorized = true;
                        partnerTimelineItems.Items.Add(item1);
                    }
                }
            }

            #endregion

            #region Nouveaux groupes

            bool hasNewGroups = false;
            var groups = this.Services.Groups.GetCreatedInRange(past, start);
            IList<NewGroup> newGroups = new List<NewGroup>();
            if (groups.Count > 0)
            {
                usefullInfos++;
                hasNewGroups = true;
                foreach (var group in groups)
                {
                    var item = new NewGroup
                    {
                        Id = @group.Id,
                        Name = @group.Name,
                        Description = @group.Description,
                        PictureUrl = this.Services.Groups.GetProfilePictureUrl(@group, CompanyPictureSize.Medium, UriKind.Absolute),
                    };
                    newGroups.Add(item);
                }
            }

            #endregion

            #region Invité par

            contact.InvitedByName = "Kévin";
            if (!contact.Registered)
            {
                var invitedby = this.Services.People.SelectWithId(contact.InvitedBy);
                if (invitedby != null)
                {
                    contact.InvitedByName = invitedby.FirstName + " " + invitedby.LastName;
                }
            }

            #endregion

            #region

            var maxAds = 40;
            var ads = this.Services.Ads.GetByDateRange(Ad.Columns.UpdateDateUtc, false, true, 0, maxAds, AdOptions.Owner, past, start);
            var adsCount = this.Services.Ads.CountByDateRange(true, past, start);

            model.Ads = ads;
            model.AdsTotal = adsCount;

            if (ads.Count > 0)
                usefullInfos++;

            #endregion

            if (usefullInfos == 0)
            {
                return null;
            }

            #region Tracking

            var identifier = start.ToString("yyy-MM-dd");
            StatsCounterHitLink trackerDisplay = null;
            StatsCounterHitLink trackerFollow = null;
            Neutral.StatsCounter counterDisplay;
            Neutral.StatsCounter counterFollow;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                counterDisplay = Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Display;
                counterFollow = Neutral.StatsCounter.WellKnown.WeeklyNewsletter.Follow;
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                counterDisplay = Neutral.StatsCounter.WellKnown.DailyNewsletter.Display;
                counterFollow = Neutral.StatsCounter.WellKnown.DailyNewsletter.Follow;
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            if (contact.UserId > 0)
            {
                trackerDisplay = this.Services.StatsCounters.GetTrackingCode(counterDisplay.Category, counterDisplay.Name, identifier, contact.UserId, this.Services.NetworkId);
                trackerFollow = this.Services.StatsCounters.GetTrackingCode(counterFollow.Category, counterFollow.Name, identifier, contact.UserId, this.Services.NetworkId);
            }
            else
            {
                trackerDisplay = this.Services.StatsCounters.GetTrackingCode(counterDisplay.Category, counterDisplay.Name, identifier, contact.UserId, this.Services.NetworkId);
                trackerFollow = this.Services.StatsCounters.GetTrackingCode(counterFollow.Category, counterFollow.Name, identifier, networkId: this.Services.NetworkId);
            }

            #endregion

            string title;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                title = this.Services.Lang.T(culture, "C'est lundi à {0}", this.Services.Lang.T("CurrentPlaceAlone"));
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                title = this.Services.Lang.T(culture, "Quoi de neuf à {0} ?", this.Services.Lang.T("CurrentPlaceAlone"));
            }
            else
            {
                throw new ArgumentException("Frequency '" + frequency + "' is not supported", "frequency");
            }

            model.Title = title;
            if (frequency == NotificationFrequencyType.Weekly)
            {
                model.Subtitle = this.Services.Lang.T(culture, "Newsletter hebdomadaire, édition du {0}.", start.ToString("D", culture));
            }
            else if (frequency == NotificationFrequencyType.Daily)
            {
                model.Subtitle = this.Services.Lang.T(culture, "Newsletter quotidienne, édition du {0}.", start.ToString("D", culture));
            }

            // new people
            model.HasNewRegistrants = hasNewReistrants;
            model.NewRegistrants = lastRegistrants;
            model.CountNewRegistrants = newRegistrants;
            model.OtherCountNewRegistrants = newRegistrants - lastRegistrants.Count;
            ////model.LastNewRegistrants = lastRegistrant;

            // events
            model.HasEvents = hasEvents;
            model.Events = nEvents.Select(o => new EventModel(o)).ToList();
            model.HasOtherEvents = hasOtherEvents;
            model.OtherEvents = nOtherEvents.Select(o => new EventModel(o)).ToList();

            // timelines
            model.HasCompaniesPublications = hasCompaniesPublications;
            model.CompaniesTimeline = companyTimelineItems;
            model.HasPeoplePublications = hasPeoplePublications;
            model.PeopleTimeline = peopleTimelineItems;
            model.HasPartnersPublications = hasPartnersPublications;
            model.PartnersTimeline = partnerTimelineItems;
            
            // groups
            model.HasNewGroups = hasNewGroups;
            model.NewGroups = newGroups;
            model.Subscriber = contact;
            model.Registered = contact.Registered;
            model.RecipientInvitedCode = contact.InvitedCode;
            model.IsCompanyFeatureEnabled = this.Services.AppConfiguration.Tree.Features.EnableCompanies;
            
            // new companies
            model.NewCompanies = comps
                .Select(c =>
                {
                    return new CompanyModel
                    {
                        Id = c.Company.ID,
                        Alias = c.Company.Alias,
                        Name = c.Company.Name,
                        CategoryId = c.Company.CategoryId,
                        Baseline = c.Company.Baseline,
                        ////About = about != null ? about.Value : string.Empty,
                        ProfileUrl = this.Services.Company.GetProfileUrl(c.Company, UriKind.Absolute),
                        PictureUrl = this.Services.Company.GetProfilePictureUrl(c.Company, Sparkle.Services.Networks.Companies.CompanyPictureSize.Large, UriKind.Absolute),
                        Skills = c.Skills.Select(s => new TagModel(s)).ToList(),
                    };
                })
                .ToList();

            var companyIds = model.NewCompanies.Select(c => c.Id).ToArray();
            var companyCategoryIds = model.NewCompanies.Select(c => c.CategoryId).ToArray();
            var companyFields = new ProfileFieldType[] { ProfileFieldType.About, };
            var fields = this.Services.ProfileFields.GetCompanyProfileFieldByCompanyIdAndType(companyIds, companyFields);
            foreach (var company in model.NewCompanies)
            {
                if (fields.ContainsKey(company.Id))
                {
                    company.SetFields(fields[company.Id]);
                }
            }

            model.CompanyCategories = this.Services.Company.GetCategoryById(companyCategoryIds);
            
            // trackers
            model.TrackerDisplay = trackerDisplay.ToUrlParameter();
            model.TrackerFollow = trackerFollow.ToUrlParameter();

            model.AboutNetwork = this.Services.Network.About;

            foreach (var item in model.NewCompanies)
            {
                foreach (var tag in item.Skills)
                {
                    int a = tag.Id;
                }
            }

            var check1 = model.Lang.T("PeopleLabel");

            string body = this.Services.EmailTemplateProvider.Process("WeeklyNewsletter", null, culture, tz, model);
            return body;
        }

        public void SendWeeklyGroupNewsletter(MemberGroupNewsletter person, string subject)
        {
            var user = person.Person;
            CultureInfo culture = null;
            TimeZoneInfo tz = null;
            if (user != null)
            {
                culture = this.Services.People.GetCulture(user.Culture);
                tz = this.Services.People.GetTimezone(user.Timezone);
            }

            culture = culture ?? this.Services.DefaultCulture;
            tz = tz ?? this.Services.Context.Timezone;

            var model = new GroupNewsletterEmailModel(person.Person.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = this.Services.Lang.T(culture, "Vos groupes"),
                Subject = subject,
                Person = person.Person,
                Groups = person.Groups,
                Registered = true,
            };

            if (model.Groups != null)
            {
                foreach (var group in model.Groups)
                {
                    group.Timeline = new BasicTimelineItemModel
                    {
                        IsRootNode = true,
                        Items = new List<BasicTimelineItemModel>(group.Walls.Count),
                    };

                    foreach (var item in group.Walls)
                    {
                        var item1 = new BasicTimelineItemModel();
                        item1.Fill(this.Services, item, item.Comments, group: group.Group);
                        item1.IsUserAuthorized = this.Services.Wall.IsVisible(item, user);
                        group.Timeline.Items.Add(item1);
                    }
                }
            }

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("WeeklyGroupNewsletter", null, culture, tz, model);
            var message = this.CreateMessage(this.Services, this.ClassCSender, EmailContact.Create(person.Person), subject, body);
            this.SendMail(this.ClassCProvider, "SendGroupNewsletter", message);
        }

        /// <summary>
        /// Sends the private message email notification.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SendPrivateMessage(Message message)
        {
            User me = this.Services.People.SelectWithId(message.FromUserId);
            User contact = this.Services.People.SelectWithId(message.ToUserId);

            this.CheckUserIsActive(contact);

            var model = new PrivateMessageEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Nouveau message de " + me.FirstName + " " + me.LastName,
                FirstName = contact.FirstName,
                ContactFirstName = me.FirstName,
                ContactLastName = me.LastName,
                ContactUrl = this.Services.Lang.T("AppDomain") + "Person/" + me.Login,
                Subject = message.Subject,
                Message = message.Text,
                MessageId = message.Id,
                ConversationUrl = this.Services.Lang.T("AppDomain") + "Conversations/" + me.Login,
                IsReplyToEmailEnabled = this.Services.AppConfiguration.Tree.Features.Timeline.EmailReply.IsEnabled,
            };

            model.NotificationType = NotificationType.PrivateMessage;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(contact.Id, NotificationType.PrivateMessage);

            model.Initialize(this.Services);

            string body = this.Services.EmailTemplateProvider.Process("PrivateMessage", null, this.Services.People.GetCulture(contact), this.Services.People.GetTimezone(contact), model);
            var mailmessage = this.CreateMessage(this.Services, this.ReplyToAdress, EmailContact.Create(contact), "Nouveau message de " + me.FirstName + " " + me.LastName, body);
            if (this.Services.AppConfiguration.Tree.Features.Timeline.EmailReply.IsEnabled)
            {
                mailmessage.ReplyToList.Add(this.ReplyToAdress.ToMailAddress());
            }

            this.SendMail(this.ClassBProvider, "SendPrivateMessage", mailmessage);
        }

        public void SendCompanyContact(CompanyContact item)
        {
            // Si c'est un contact d'une entreprise extérieur
            if (item.FromUser != null)
            {
                item.FromUserName = item.FromUser.FirstName + " " + item.FromUser.LastName;
                item.FromUserEmail = item.FromUser.Email;
            }

            if (item.FromCompany != null)
            {
                item.FromCompanyName = item.FromCompany.Name;
            }

            if (item.ToCompany != null && this.Services.Company.IsActive(item.ToCompany))
            {
                // Message à une entreprise du réseau
                var contacts = this.Services.People.SelectCompanyContacts(item.ToCompany.ID);

                foreach (var contact in contacts)
                {
                    if (this.Services.People.IsActive(contact) && (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(contact)))
                    {
                        var model = new CompanyContactEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                        {
                            Title = "Message de la société " + item.FromCompanyName,
                            ToUser = contact,
                            ToCompany = contact.Company,
                            FromUserName = item.FromUserName,
                            FromUserEmail = item.FromUserEmail,
                            FromCompanyName = item.FromCompanyName,
                            Message = item.Message,
                            ConversationUrl = this.Services.Lang.T("AppDomain") + "Companies/Messages"
                        };

                        model.Initialize(this.Services);
                        string body = null;
                        body = this.Services.EmailTemplateProvider.Process("CompanyContact", null, this.Services.People.GetCulture(contact), this.Services.People.GetTimezone(contact), model);
                        var mailmessage = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(contact), "Message de la société " + item.FromCompanyName, body);

                        this.SendMail(this.ClassBProvider, "SendCompanyContact", mailmessage);
                    }
                }
            }
            else
            {
                // Message à une entreprise HORS du réseau
                var model = new ExternalCompanyContactEmailModel(item.ToUserEmail, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                {
                    Title = "Message de la société " + item.FromCompanyName,
                    FromUserName = item.FromUserName,
                    FromUserEmail = item.FromUserEmail,
                    FromCompanyName = item.FromCompanyName,
                    Message = item.Message,
                    ResponseUrl = this.Services.Lang.T("AppDomain") + "Companies/Contact/" + item.FromCompany.Alias
                };

                model.Initialize(this.Services);
                string body = null;
                body = this.Services.EmailTemplateProvider.Process("ExternalCompanyContact", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
                var mailmessage = this.CreateMessage(this.Services, this.ClassBSender, new EmailContact(item.ToUserEmail, item.ToCompany.Name), "Message de la société " + item.FromCompanyName, body);

                this.SendMail(this.ClassBProvider, "SendCompanyContact", mailmessage);
            }

        }

        public void SendAddResumeConfirmation(Resume resume)
        {
            var model = new AddResumeConfirmationEmailModel(resume.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Confirmation de l'enregistrement de votre profil",
                FirstName = resume.FirstName,
                Id = resume.Id,
                Pin = resume.Pin
            };

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("AddResume", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var mailmessage = this.CreateMessage(this.Services, this.ClassBSender, new EmailContact(resume.Email, resume.FirstName + " " + resume.LastName), "Confirmation de l'enregistrement de votre profil", body);

            this.SendMail(this.ClassAProvider, "SendAddResume", mailmessage);
        }

        public void SendProposal(ProposalRequest proposalRequest)
        {
            User me = this.Services.People.SelectWithId(proposalRequest.From);
            User contact = this.Services.People.SelectWithId(proposalRequest.To);

            this.CheckUserIsActive(contact);

            var model = new ProposalEmailModel(contact.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = proposalRequest.EmailTitle,
                EmailText = proposalRequest.EmailText,
                FirstName = contact.FirstName,
                ContactFirstName = me.FirstName,
                ContactLastName = me.LastName,
                ContactUrl = this.Services.Lang.T("AppDomain") + "Person/" + me.Login,
                ////ContactPictureUrl = this.Services.Lang.T("AppDomain") + "Data/PersonPicture/" + me.Login + "/Medium",
                ContactPictureUrl = this.Services.People.GetProfilePictureUrl(me.Login, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute),
                Message = proposalRequest.Message,
                ConversationUrl = this.Services.Lang.T("AppDomain") + "Conversations/" + me.Login,
                Date = proposalRequest.DateTime,
            };

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("Proposal", null, this.Services.People.GetCulture(contact), this.Services.People.GetTimezone(contact), model);
            var mailmessage = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(contact), proposalRequest.Title + " avec " + me.FirstName + " " + me.LastName, body);

            this.SendMail(this.ClassBProvider, "SendProposal", mailmessage);
        }

        public void SendCompleteProfile(User profile, User me)
        {
            this.CheckUserIsActive(profile);

            var model = new CompleteProfileEmailModel(profile.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                {
                    Title = "Suggestion",
                    FirstName = profile.FirstName,
                    ContactFirstName = me.FirstName,
                    ContactLastName = me.LastName,
                    ContactUrl = this.Services.Lang.T("AppDomain") + "Person/" + me.Login,
                    CompleteProfileUrl = this.Services.Lang.T("AppDomain") + "Account/Settings",
                    Comment = me.FirstName + ", et sans doute les " + this.Services.People.CountActive() + " autres inscrits aimeraient en savoir plus sur vous. Pensez à compléter votre présentation et à ajouter vos compétences.",
                };

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("CompleteProfile", null, this.Services.People.GetCulture(profile), this.Services.People.GetTimezone(profile), model);
            var mailmessage = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(profile), me.FirstName + " vous suggère de compléter votre profil", body);

            this.SendMail(this.ClassBProvider, "SendCompleteProfile", mailmessage);
        }

        private void CheckUserIsActive(User profile)
        {
            if (!this.Services.People.IsActive(profile))
                throw new InvalidOperationException("The user " + profile.Id + " is not active.");
        }

        public void SendRecallMail(int invitedId)
        {
            Invited user = this.Services.Invited.SelectById(invitedId);
            if (user != null)
            {
                SendRecallMail(user);
            }
        }

        public void SendRecallMail(Invited invited)
        {
            string subject = "Invitation à rejoindre " + this.Services.Lang.T("AppName");
            var model = new ReminderEmailModel(invited.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang);

            model.InvitedEmail = invited.Email;

            var contact = this.Services.People.SelectWithId(invited.InvitedByUserId);
            model.ContactName = contact.FirstName + " " + contact.LastName;
            model.ContactLogin = contact.Login;
            subject = model.ContactName + " vous invite à rejoindre " + this.Services.Lang.T("AppName");

            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("Reminder", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(invited), subject, body);

            this.SendMail(this.ClassAProvider, "SendRecallMail", message);
        }

        public void SendProposeToEat(int userId, User contact, List<User> others, Place place)
        {
            User me = this.Services.People.SelectWithId(userId);
            string contacts = "";

            int i = 0;
            int max = others.Count;
            foreach (User ctc in others)
            {
                i++;
                string sep = ",";
                if (i == max)
                {
                    sep = ".";
                }
                else if (i == max - 1)
                {
                    sep = " et ";
                }
                else
                {
                    sep = ", ";
                }
                contacts += ctc.FirstName + " " + ctc.LastName + sep;
            }
            string othersString = "";
            if (max > 0)
            {
                othersString = "Cette proposition concerne également " + contacts;
            }

            var dictionnary = new Dictionary<string, string>
            {
                {"[CONTACTFIRSTNAME]", contact.FirstName},
                {"[PROFILE]", this.Services.Lang.T("AppDomain") + "Person/" +me.Login},
                {"[FIRSTNAME]", me.FirstName},
                {"[LASTNAME]", me.LastName},
                {"[PLACENAME]", place.Name},
                {"[PLACEURL]", this.Services.Lang.T("AppDomain") + "Place/" + place.Alias},
                {"[OTHERS]", othersString},
                {EMAIL, contact.Email}
            };
            string body = GetBody(this.Services, EmailTemplates.ProposeEat, dictionnary);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(contact), "Déjeuner '" + place.Name + "' ?", body);

            this.SendMail(this.ClassBProvider, "SendProposeToEat", message);
        }

        /// <summary>
        /// Sends an email message with the specified provider and logging parameters.
        /// NOTICE: ONLY ONE (1) RECIPIENT!
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="logpath"></param>
        /// <param name="message"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
        public EmailSendResult SendMail(IEmailProvider provider, string logpath, MailMessage message, bool logging = true)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (message == null)
                throw new ArgumentNullException("message");
            if (string.IsNullOrEmpty(logpath))
                throw new ArgumentException("The value cannot be empty", "logpath");
            
            try
            {
                var tags = new string[] { "Sn", "Sn-" + this.Services.Network.Name, "SnTpl-" + logpath.TrimToLength(40), };
                var results = provider.SimpleSend(message, tags);
                if (logging)
                    this.LogSendSuccess("EmailService." + logpath, message);

                return results.Single(); // this crashes if you specify zero or multiple recipients.
            }
            catch (InvalidOperationException ex)
            {
                this.LogSendError("EmailService." + logpath, message, ex);
                throw;
            }
        }

        public void SendErrorReport(string content, string recipients)
        {
            var msg = this.CreateMessage(this.Services,
                this.ClassBSender,
                new EmailContact(recipients),
                "error report from " + this.Services.Lang.T("AppDomain"),
                content);
            msg.IsBodyHtml = false;

            this.SendMail(this.ClassAProvider, "SendErrorReport", msg);
        }

        public void SendRecovery(User person, string recoveryLink, string message = null, string subject = null, bool isPasswordReset = true)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (string.IsNullOrEmpty(recoveryLink))
                throw new ArgumentException("The value cannot be empty", "recoveryLink");

            var model = new RecoverPasswordEmailModel(person.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = subject ?? "Réinitialiser votre mot de passe‏",
                FirstName = person.FirstName,
                LastName = person.LastName,
                Login = person.Login,
                Email = person.Email,
                Link = recoveryLink,
                Message = message,
                IsPasswordReset = isPasswordReset,
            };

            this.SendRecovery(person, model);
        }
        
        private void SendRecovery(User email, RecoverPasswordEmailModel model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("RecoverPassword", null, this.Services.People.GetCulture(email), this.Services.People.GetTimezone(email), model);
            var message = this.CreateMessage(this.Services, this.ClassASender, EmailContact.Create(email), model.Title, body);

            this.SendMail(this.ClassAProvider, "SendRecovery", message);
        }

        public void SendEmailChangeRequest(User person, string email, string confirmationLink)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (string.IsNullOrEmpty(confirmationLink))
                throw new ArgumentException("The value cannot be empty", "confirmationLink");

            var model = new EmailChangeEmailModel(email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Confirmer votre changement d'e-mail",
                FirstName = person.FirstName,
                Login = person.Login,
                Email = email,
                Link = confirmationLink,
                CreateDateUtc = this.Services.Context.Timezone.ConvertFromUtc(DateTime.UtcNow).ToShortDateString(),
            };

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("EmailChangeRequest", null, this.Services.People.GetCulture(person), this.Services.People.GetTimezone(person), model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(email, person.FullName), "Changement de votre adresse e-mail‏", body);

            this.SendMail(this.ClassAProvider, "SendEmailChangeRequest", message);
        }

        public void SendPrivateGroupJoinRequest(User person, Group group, int joining)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            if (group == null)
                throw new ArgumentNullException("group");

            this.CheckUserIsActive(person);

            var userJoin = this.Services.People.SelectWithId(joining);
            if (userJoin == null)
                throw new ArgumentException("There is no user with this id", "joining");

            var model = new GroupJoinRequestEmailModel(person.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Un utilisateur demande à rejoindre le groupe " + group.Name,
                AdminFirstName = person.FirstName,
                UserFirstName = userJoin.FirstName,
                UserLastName = userJoin.LastName,
                GroupName = group.Name,
                RequestLink = this.Services.Lang.T("AppDomain") + "Groups/Requests/" + group.Id,
            };

            model.NotificationType = NotificationType.PrivateGroupJoinRequest;
            model.NotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(person.Id, null);
            model.UnsubscribeNotificationUrl = this.Services.Notifications.GetUnsubscribeActionUrl(person.Id, NotificationType.PrivateGroupJoinRequest);

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("PrivateGroupJoinRequest", null, this.Services.People.GetCulture(person), this.Services.People.GetTimezone(person), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(person), "Demande d'ajout à un de vos groupes", body);

            this.SendMail(this.ClassBProvider, "SendPrivateGroupJoinRequest", message);
        }

        public void SendGroupJoinResponse(User user, Group group, bool success)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (group == null)
                throw new ArgumentNullException("group");

            this.CheckUserIsActive(user);

            var groupName = this.Services.Groups.SelectGroupById(group.Id).Name;

            var model = new GroupJoinResponseEmailModel(user.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Demande " + (success ? "acceptée ! :)" : "rejetée ! :("),
                Success = success,
                FirstName = user.FirstName,
                GroupName = groupName,
                GroupLink = this.Services.Lang.T("AppDomain") + "Group/" + group.Id,
                GroupListLink = this.Services.Lang.T("AppDomain") + "Groups#2",
            };

            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("PrivateGroupJoinResponse", null, this.Services.People.GetCulture(user), this.Services.People.GetTimezone(user), model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(user), "Réponse à votre demande d'ajout à " + groupName, body);

            this.SendMail(this.ClassBProvider, "SendGroupJoinResponse", message);
        }

        #region Logs

        #endregion

        public void SendNewCompanyDetailsForApproval(Company item, IList<Sparkle.Entities.Networks.Neutral.Person> recipients)
        {
            var companyEmail = this.Services.ProfileFields.GetCompanyProfileFieldByCompanyIdAndType(item.ID, ProfileFieldType.Email)
                .FirstOrDefault();

            foreach (var recipient in recipients)
            {
                var model = new CompanyEmailModel(recipient.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                {
                    Title = "Demande d'inscription",
                    Company = item,
                    Name = item.Name,
                    Alias = item.Alias,
                    Email = companyEmail != null ? companyEmail.Value : string.Empty,
                    CompanyId = item.ID,
                };
                model.Initialize(this.Services);
                string body = this.Services.EmailTemplateProvider.Process("NewCompanyDetailsForApproval", null, this.Services.People.GetCulture(recipient), this.Services.People.GetTimezone(recipient), model);
                var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(recipient), model.Title, body);
                this.SendMail(this.ClassBProvider, "SendNewCompanyDetailsForApproval", message);
            }
        }

        public void SendNewCompanyDetailsForApproval(CompanyRequest item, IList<Sparkle.Entities.Networks.Neutral.Person> recipients)
        {
            foreach (var recipient in recipients)
            {
                var model = new CompanyEmailModel(recipient.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                {
                    Title = "Demande d'inscription - " + item.Name,
                    CompanyRequest = item,
                    Name = item.Name,
                    Alias = item.Alias,
                    Email = item.Email,
                    RequestId = item.Id,
                    RequestUniqueId = item.UniqueId,
                };
                model.Initialize(this.Services);
                string body = this.Services.EmailTemplateProvider.Process("NewCompanyDetailsForApproval", null, this.Services.People.GetCulture(recipient), this.Services.People.GetTimezone(recipient), model);
                var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(recipient), model.Title, body);
                this.SendMail(this.ClassBProvider, "SendNewCompanyDetailsForApproval", message);
            }
        }
        
        public void SendCompanyRequestConfirmation(CompanyRequest item, string requesterEmail)
        {
            var model = new CompanyEmailModel(requesterEmail, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Etat de votre demande d'inscription",
                CompanyRequest = item,
                Name = item.Name,
                Alias = item.Alias,
                Email = item.Email,
                RequestId = item.Id,
                RequestUniqueId = item.UniqueId,
            };
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("CompanyRequestConfirmation", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(requesterEmail), model.Title, body);
            this.SendMail(this.ClassBProvider, "SendCompanyRequestConfirmation", message);
        }

        public void SendCompanyRequestAccepted(Company item, string requesterEmail, string[] adminEmails, string[] otherEmails)
        {
            var companyEmail = this.Services.ProfileFields.GetCompanyProfileFieldByCompanyIdAndType(item.ID, ProfileFieldType.Email)
                .FirstOrDefault();

            var model = new CompanyEmailModel(requesterEmail, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Demande d'inscription : acceptée",
                Company = item,
                Name = item.Name,
                Alias = item.Alias,
                Email = companyEmail != null ? companyEmail.Value : string.Empty,
                AdminEmails = adminEmails ?? new string[0],
                OtherEmails = otherEmails ?? new string[0],
            };
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("CompanyRequestAccepted", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(requesterEmail), model.Title, body);
            this.SendMail(this.ClassAProvider, "SendCompanyRequestAccepted", message);
        }

        public void SendCompanyRegisteredNotification(Company item, User inviter, string requesterEmail, string[] adminEmails, string[] otherEmails)
        {
            var companyEmail = this.Services.ProfileFields.GetCompanyProfileFieldByCompanyIdAndType(item.ID, ProfileFieldType.Email)
                .FirstOrDefault();

            var model = new CompanyEmailModel(requesterEmail, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Inscription de l'entreprise " + item.Name,
                Sender = inviter,
                SenderPictureUrl = this.Services.People.GetProfilePictureUrl(inviter, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute),
                Company = item,
                Name = item.Name,
                Alias = item.Alias,
                Email = companyEmail != null ? companyEmail.Value : string.Empty,
                AdminEmails = adminEmails ?? new string[0],
                OtherEmails = otherEmails ?? new string[0],
            };
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("CompanyRegisteredNotification", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(requesterEmail), model.Title, body);
            this.SendMail(this.ClassBProvider, "SendCompanyRegisteredNotification", message);
        }

        public void SendCompanyRequestRejected(CompanyRequest item, string requesterEmail, string reason)
        {
            var model = new CompanyEmailModel(requesterEmail, this.Services.Lang.T("AccentColor"), this.Services.Lang)
            {
                Title = "Demande d'inscription : refusée",
                CompanyRequest = item,
                Name = item.Name,
                Alias = item.Alias,
                Email = item.Email,
                Reason = reason,
            };
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("CompanyRequestRejected", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(requesterEmail), model.Title, body);
            this.SendMail(this.ClassBProvider, "SendCompanyRequestRejected", message);
        }

        public void SendNewUserConfirmEmail(Services.Networks.EmailModels.NewUserConfirmEmail model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("NewUserConfirmEmail", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(model.RecipientEmailAddress, model.Recipient.FullName), "Confirmez votre compte " + this.Services.Lang.T("AppName"), body);
            ////message.To.Add(new MailAddress(model.RecipientEmailAddress, model.Recipient.FullName)); // replace here /\
            this.SendMail(this.ClassAProvider, "SendNewUserConfirmEmail", message);
        }

        public void SendPendingUserRegistrations(Services.Networks.EmailModels.PendingUserRegistrationsModel model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("PendingUserRegistrations", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, new EmailContact(model.RecipientContact.EmailAddress, model.RecipientContact.FullName), "Nouveaux utilisateurs inscrits (en attente) sur " + this.Services.Lang.T("AppName"), body);
            ////message.To.Add(new MailAddress(model.RecipientContact.EmailAddress, model.RecipientContact.FullName)); // replaced here /\
            this.SendMail(this.ClassBProvider, "SendPendingUserRegistrations", message);
        }

        public void SendTechnicalMessages(IList<string> messages, NetworkAccessLevel[] networkAccessLevels)
        {
            var users = new List<Neutral.Person>();
            for (int i = 0; i < networkAccessLevels.Length; i++)
            {
                var items = this.Services.People.GetByNetworkAccessLevel(networkAccessLevels[i]);
                for (int j = 0; j < items.Count; j++)
                {
                    if (!users.Any(u => u.Id == items[j].Id))
                        users.Add(items[j]);
                }
            }

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var model = new TechnicalMessagesModel(user.Email, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                {
                    Messages = messages,
                    Recipients = users.ToList(),
                };
                model.Initialize(this.Services);
                string body = this.Services.EmailTemplateProvider.Process("TechnicalMessages", null, this.Services.People.GetCulture(user), this.Services.People.GetTimezone(user), model);
                var message = this.CreateMessage(this.Services, this.ClassBSender, EmailContact.Create(user), "Messages techniques", body);
                this.SendMail(this.ClassAProvider, "TechnicalMessages", message);
            }
        }

        public void SendRegisterRequest(RegisterRequestEmailModel model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("RegisterRequest", null,this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassBSender, new EmailContact(model.RecipientContact.EmailAddress, model.RecipientContact.FullName), "Demande d'inscription en attente sur " + this.Services.Lang.T("AppName"), body);
            ////message.To.Add(new MailAddress(model.RecipientContact.EmailAddress, model.RecipientContact.FullName)); // replaced here /\
            this.SendMail(this.ClassBProvider, "SendRegisterRequest", message);
        }

        public void SendRegisterRequestConfirmation(RegisterRequestEmailModel model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("RegisterRequestConfirmation", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(model.RecipientEmailAddress), "Votre demande d'inscription sur " + this.Services.Lang.T("AppName"), body);
            ////message.To.Add(new MailAddress(model.RecipientEmailAddress)); // replaced here /\
            this.SendMail(this.ClassAProvider, "SendRegisterRequestConfirmation", message);
        }

        public void SendRegisterRequestDenied(RegisterRequestEmailModel model)
        {
            model.Initialize(this.Services);
            string body = this.Services.EmailTemplateProvider.Process("RegisterRequestDenied", null, this.Services.DefaultCulture, this.Services.DefaultTimezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(model.RecipientEmailAddress), "Votre demande d'inscription sur " + this.Services.Lang.T("AppName"), body);
            ////message.To.Add(new MailAddress(model.RecipientEmailAddress)); // replaced here /\
            this.SendMail(this.ClassBProvider, "SendRegisterRequestDenied", message);
        }

        public InboundEmailReport HandleInboundEmail(InboundEmailModel obj)
        {
            const string path = "EmailService.HandleInboundEmail";

            // Initialize report
            var report = new InboundEmailReport();
            bool newWallItem = false;

            report.Message = obj.Msg;

            ////if (obj.Event.ToLowerInvariant() == "inbound")
            if (obj.Event == WebHookEventType.Inbound)
            {
                // match user by email sender
                var postedBy = this.Repo.People.GetByEmail(obj.Msg.FromEmail, this.Services.NetworkId, Data.Options.PersonOptions.None);
                if (postedBy == null)
                {
                    report.Error("User with email " + obj.Msg.FromEmail + " does not exist");
                    this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                    return report;
                }

                report.Log("Sender has been recognized : user " + postedBy.FullName + " with email " + postedBy.Email);

                // find timelineitem id
                var capture = string.Empty;
                int timelineItemId = 0;
                TimelineItem item = null;
                Message itemMessage = null;
                var captureResult = obj.Msg.Html != null ? ExtractItemId(obj.Msg.Html, out capture) : ItemCaptureResult.None;
                if (captureResult == ItemCaptureResult.TimelineItem)
                {
                    if (int.TryParse(capture, out timelineItemId))
                    {
                        item = this.Services.Wall.SelectByPublicationId(timelineItemId);
                        if (item == null)
                        {
                            report.Error("TimelineItem with id " + timelineItemId + " does not exist");
                            this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                            return report;
                        }

                        report.Log("TimelineItem with id " + timelineItemId + " loaded successfuly, entering comment mode");
                    }
                    else
                    {
                        newWallItem = true;
                        report.Log("WallItem info found, but Id is not a valid integer, entering new publication mode");
                    }
                }
                else if (captureResult == ItemCaptureResult.PrivateMessage)
                {
                    if (int.TryParse(capture, out timelineItemId))
                    {
                        itemMessage = this.Services.PrivateMessage.GetPrivateMessageById(timelineItemId);
                        if (itemMessage == null)
                        {
                            report.Error("PrivateMessage with id " + timelineItemId + " does not exist");
                            this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                            return report;
                        }

                        if (itemMessage.From.Email != obj.Msg.FromEmail && itemMessage.To.Email != obj.Msg.FromEmail)
                        {
                            report.Error("Sender: " + obj.Msg.FromEmail + " does not match private message (" + timelineItemId + ") sender nor receiver");
                            this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                            return report;
                        }

                        report.Log("PrivateMessage with id " + timelineItemId + " loaded successfuly and sender/receiver recognized, entering response mode");
                    }
                    else
                    {
                        report.Error("PrivateMessage info found, but Id is not a valid integer, aborting");
                        this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                        return report;
                    }
                }
                else
                {
                    newWallItem = true;
                    captureResult = ItemCaptureResult.TimelineItem;
                    report.Log("No WallItemId nor PrivateMessageId found, entering new publication mode");
                }

                // Determine sender provider to extract message
                var message = obj.Msg.Html ?? obj.Msg.Text;
                try
                {
                    var splitter = new EmailSplitter();
                    var genericRules = new string[] { "class=\"Sparkle-Message\"", "name=\"Sparkle-Message\"", };
                    var messageParts = splitter.GetMessageFromEmailHtml(message, genericRules);
                    message = messageParts.UserMessage;
                    if (!newWallItem && messageParts.ReplyQuote == null)
                    {
                        message = message.Length > 4000 ? message.Substring(0, 3999) : message;
                        Services.Logger.Error(
                            "EmailService.HandleInboundEmail",
                            ErrorLevel.Business,
                            "Received email from '{0}' to '{1}' with subject '{2}' and a wall item id was found within but the reply quote cannot be retrieved, the provider may be unknown",
                            obj.Msg.FromEmail, obj.Msg.Email, obj.Msg.Subject);
                    }
                }
                catch (Exception)
                {
                    this.Services.Logger.Error(
                        "EmailService.HandleIndounbEmail",
                        ErrorLevel.Business,
                        "Received email from '{0}' to '{1}' with subject '{2}' but no provider was found to handle its content",
                        obj.Msg.FromEmail, obj.Msg.Email, obj.Msg.Subject);
                    return report;
                }

                // Email to all subscribers (owner of item + owners of comments + notifications subscribed)
                if (captureResult == ItemCaptureResult.TimelineItem)
                {
                    report.OnSucceedItemType = ItemCaptureResult.TimelineItem;
                    // Finally, publish new publication or comment an existing one
                    if (newWallItem)
                    {
                        if (!string.IsNullOrEmpty(obj.Msg.Subject))
                        {
                            message = "**" + obj.Msg.Subject + " :**\n\n" + message;
                        }

                        var wallItem = this.Services.Wall.Publish(postedBy, null, TimelineItemType.TextPublication, message, TimelineType.Public, 0);

                        report.OnSucceedNewItem = true;
                        report.OnSucceedPublishId = wallItem.Id;

                        this.Services.Logger.Info(
                            "EmailService.HandleInboundEmail",
                            ErrorLevel.Success,
                            "Received email from '{0}' to '{1}' with subject '{2}' and published to the main timeline",
                            obj.Msg.FromEmail, obj.Msg.Email, obj.Msg.Subject);
                        report.Log("new TimelineItem publication successful! ItemId=" + report.OnSucceedPublishId);
                    }
                    else
                    {
                        bool isAble = true;
                        if (item.GroupId != null)
                        {
                            var group = item.Group;
                            isAble = group.Members.Where(o => o.Accepted == (int)GroupMemberStatus.Accepted).Select(o => o.UserId).Contains(postedBy.Id);
                        }

                        if (isAble)
                        {
                            var request = new Sparkle.Services.Networks.Timelines.TimelineCommentRequest
                            {
                                Comment = message,
                                TimelineItemId = item.Id,
                                UserId = postedBy.Id,
                            };
                            var result = this.Services.WallComments.Publish(request);
                            var comment = result.CommentEntity;

                            report.OnSucceedNewItem = false;
                            report.OnSucceedPublishId = comment.Id;
                            this.Services.Logger.Info(
                                "EmailService.HandleInboundEmail",
                                ErrorLevel.Success,
                                "Received email from '{0}' to '{1}' with subject '{2}' and published as comment into the timeline item {3}",
                                obj.Msg.FromEmail, obj.Msg.Email, obj.Msg.Subject, item.Id.ToString());
                            report.Log("new TimelineComment publication successful! ItemId=" + (item != null ? item.Id : 0) + " CommentId=" + report.OnSucceedPublishId);
                        }
                        else
                        {
                            var reason = "";
                            var thirdOption = item.Id.ToString();
                            if (item.GroupId != null)
                            {
                                reason = "Received email from '{0}' to '{1}' with subject '{2}' but user is no longer in the group {3}";
                                thirdOption = item.Group.Name;
                            }
                            this.Services.Logger.Info(
                                "EmailService.HandleInboundEmail",
                                ErrorLevel.Business,
                                reason,
                                obj.Msg.FromEmail, obj.Msg.Email, obj.Msg.Subject, thirdOption);

                            report.Error(reason);
                            report.Succeed = false;
                            return report;
                        }
                    }
                }
                else if (captureResult == ItemCaptureResult.PrivateMessage)
                {
                    int fromId = 0;
                    int toId = 0;
                    // Determine receiver
                    if (itemMessage.From.Email == obj.Msg.FromEmail)
                    {
                        fromId = itemMessage.FromUserId;
                        toId = itemMessage.ToUserId;
                    }
                    else
                    {
                        fromId = itemMessage.ToUserId;
                        toId = itemMessage.FromUserId;
                    }

                    var toSend = new SendPrivateMessageRequest
                    {
                        ActingUserId = fromId,
                        TargetUserId = toId,
                        Message = message,
                        Source = MessageSource.InboundEmail,
                    };
                    var messageResult = this.Services.PrivateMessage.Send(toSend);

                    if (messageResult.Succeed)
                    {
                        report.Log("Private message " + messageResult.Item.Id + " from " + itemMessage.FromUserId + " successfuly sent to " + itemMessage.ToUserId);
                        this.Services.Logger.Info("EmailsServices.HandleInboundEmail",
                                                    ErrorLevel.Success,
                                                    "Private message {0} from {1} successfuly sent to {2}",
                                                    messageResult.Item.Id, itemMessage.FromUserId, itemMessage.ToUserId);
                        report.OnSucceedNewItem = false;
                        report.OnSucceedPublishId = messageResult.Item.Id;
                        report.OnSucceedItemType = ItemCaptureResult.PrivateMessage;
                        report.Log("new PrivateMessage publication successful! MessageId=" + (messageResult.Item != null ? messageResult.Item.Id : 0));
                    }
                    else
                    {
                        report.Log("Private message " + messageResult.Item.Id + " from " + itemMessage.FromUserId + " to " + itemMessage.ToUserId + " failed. ");
                        this.Services.Logger.Error("EmailsServices.HandleInboundEmail",
                                                    ErrorLevel.Business,
                                                    "Private message {0} from {1} to {2} failed",
                                                    messageResult.Item.Id, itemMessage.FromUserId, itemMessage.ToUserId);
                        report.Log("new PrivateMessage publication failed! ");
                    }
                }
                else
                {
                    report.Error("No inbound email handling type was found, this could have ripped the all reality apart, aborting");
                    this.Services.Logger.Error(path, ErrorLevel.Critical, string.Join(Environment.NewLine, report.Errors));
                    return report;
                }
                
                report.Succeed = true;

                return report;
            }
            else
            {
                report.Error("Email is not of type inbound");
                report.Succeed = false;

                this.Services.Logger.Error(path, ErrorLevel.Input, string.Join(Environment.NewLine, report.Errors));
                return report;
            }
        }

        public AdminWorksModel GetAdminWorkModel(
            bool companiesValidation = false,
            bool usersValidation = false,
            bool companiesNoAdmin = false,
            bool adsValidation = false)
        {
            var model = new AdminWorksModel();

            if (companiesValidation)
            {
                model.Items.AddRange(this.Services.Company.GetPendingRequests()
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
            }

            if (usersValidation)
            {
                model.Items.AddRange(this.Services.UserEmailChangeRequest.GetPendingRequests()
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
                model.Items.AddRange(this.Services.RegisterRequests.GetPendingRegisterRequests(RegisterRequestOptions.None)
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
                model.Items.AddRange(this.Services.People.GetPendingApplyRequests()
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
                model.Items.AddRange(this.Services.PartnerResources.GetPendingRequests()
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
            }

            if (companiesNoAdmin)
            {
                model.Items.AddRange(this.Services.Company.GetCompanyAccessLevelIssues());
            }

            if (adsValidation)
            {
                model.Items.AddRange(this.Services.Ads.GetPendingList(AdOptions.None)
                    .Select(r => AdminWorkModel.For(this.Services, r, AdminWorkPriority.Current)));
            }

            return model;
        }

        public void SendAdminWorkEmail(AdminWorksModel model, bool discloseRecipients, params NetworkAccessLevel[] roles)
        {
            var recipients = new Dictionary<int, AdminWorkRecipient>();

            foreach (var role in roles)
            {
                var users = this.Services.People.GetByNetworkAccessLevel(role);
                foreach (var user in users)
                {
                    if (recipients.ContainsKey(user.Id))
                    {
                        recipients[user.Id].NetworkAccessLevels.Add(role);
                    }
                    else
                    {
                        recipients.Add(user.Id, new AdminWorkRecipient
                        {
                            User = new UserModel(user),
                            Contact = EmailContact.Create(user),
                            NetworkAccessLevels = new List<NetworkAccessLevel>()
                            {
                                role,
                            },
                        });
                    }
                }
            }

            model.Recipients = recipients.Values.ToList();
            model.DiscloseRecipients = discloseRecipients;
            this.SendAdminWorkEmail(model);
        }

        public void SendAdminWorkEmail(AdminWorksModel model, bool discloseRecipients, IList<UserModel> tos)
        {
            model.DiscloseRecipients = discloseRecipients;
            model.Recipients = tos
                .Select(r => new AdminWorkRecipient
                {
                    User = r,
                    Contact = r.GetEmailContact(),
                })
                .ToList();
            this.SendAdminWorkEmail(model);
        }

        public void SendApplyRequestConfirmation(Services.Networks.Users.ApplyRequestModel model, UserModel requester)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (requester == null)
                throw new ArgumentNullException("requester");

            var culture = this.Services.People.GetCulture(requester);
            var tz = this.Services.People.GetTimezone(requester);

            var emailModel = new BaseEmailModel<Services.Networks.Users.ApplyRequestModel>(
                requester.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);
            emailModel.Data["RequestLink"] = this.Services.People.GetApplyRequestConfirmUrl(model);
            var title = this.Services.Lang.T(culture, "Votre demande d'inscription");
            
            var body = this.Services.EmailTemplateProvider.Process("ApplyRequestConfirmation", null, culture, tz, emailModel);
            var message = this.CreateMessage(this.Services, this.ClassASender, requester.GetEmailContact(), title, body);
            this.SendMail(this.ClassAProvider, "SendApplyRequestConfirmation", message);
        }

        public void SendApplyRequestAccepted(Sparkle.Services.Networks.Users.ApplyRequestModel model, UserModel accepted)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (accepted == null)
                throw new ArgumentNullException("accepted");

            var culture = this.Services.People.GetCulture(accepted);
            var tz = this.Services.People.GetTimezone(accepted);

            var emailModel = new BaseEmailModel<Sparkle.Services.Networks.Users.ApplyRequestModel>(
                accepted.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);
            emailModel.Data["RequestLink"] = this.Services.People.GetApplyRequestJoinUrl(model);
            var title = this.Services.Lang.T(culture, "Demande acceptée !");

            var body = this.Services.EmailTemplateProvider.Process("ApplyRequestAccepted", null, culture, tz, emailModel);
            var message = this.CreateMessage(this.Services, this.ClassASender, accepted.GetEmailContact(), title, body);
            this.SendMail(this.ClassAProvider, "SendApplyRequestAccepted", message);
        }

        public void SendApplyRequestRefused(Sparkle.Services.Networks.Users.ApplyRequestModel model, UserModel refused)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (refused == null)
                throw new ArgumentNullException("refused");

            var culture = this.Services.People.GetCulture(refused);
            var tz = this.Services.People.GetTimezone(refused);

            var emailModel = new BaseEmailModel<Sparkle.Services.Networks.Users.ApplyRequestModel>(
                refused.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);
            var title = this.Services.Lang.T(culture, "Demande refusée");
            
            var body = this.Services.EmailTemplateProvider.Process("ApplyRequestRefused", null, culture, tz, emailModel);
            var message = this.CreateMessage(this.Services, this.ClassASender, refused.GetEmailContact(), title, body);
            this.SendMail(this.ClassAProvider, "SendApplyRequestRefused", message);
        }

        public void SendPartnerResourceProposalAccepted(Sparkle.Services.Networks.PartnerResources.PartnerResourceModel model, UserModel user)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (user == null)
                throw new ArgumentNullException("user");

            var culture = this.Services.People.GetCulture(user);
            var tz = this.Services.People.GetTimezone(user);

            var emailModel = new BaseEmailModel<Sparkle.Services.Networks.PartnerResources.PartnerResourceModel>(
                user.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);
            emailModel.Data["RequestLink"] = this.Services.PartnerResources.GetProfileUrl(model.Alias, UriKind.Absolute);
            var title = this.Services.Lang.T(culture, "Partenaire acceptée !");

            var body = this.Services.EmailTemplateProvider.Process("PartnerResourceProposalAccepted", null, culture, tz, emailModel);
            var message = this.CreateMessage(this.Services, this.ClassBSender, user.GetEmailContact(), title, body);
            this.SendMail(this.ClassBProvider, "SendPartnerResourceProposalAccepted", message);
        }

        public void SendPartnerResourceProposalRefused(Sparkle.Services.Networks.PartnerResources.PartnerResourceModel model, UserModel user)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (user == null)
                throw new ArgumentNullException("refused");

            var culture = this.Services.People.GetCulture(user);
            var tz = this.Services.People.GetTimezone(user);

            var emailModel = new BaseEmailModel<Sparkle.Services.Networks.PartnerResources.PartnerResourceModel>(
                user.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);
            var title = this.Services.Lang.T(culture, "Partenaire refusée");

            var body = this.Services.EmailTemplateProvider.Process("PartnerResourceProposalRefused", null, culture, tz, emailModel);
            var message = this.CreateMessage(this.Services, this.ClassBSender, user.GetEmailContact(), title, body);
            this.SendMail(this.ClassBProvider, "SendPartnerResourceProposalRefused", message);
        }

        public void SendSubscriptionActivated(SubscriptionModel subscription, UserModel user)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");
            if (user == null)
                throw new ArgumentNullException("user");

            var template = this.Services.SubscriptionTemplates.GetById(subscription.TemplateId);

            var textModel = this.Services.SubscriptionTemplates.GetEditTextRequest(template.Id, this.Services.People.GetCulture(user), "Confirm");
            if (textModel != null)
            {
                var subModel = new SubscriptionEmailModel
                {
                    Subscription = subscription,
                    Template = template,
                    User = user,
                    NetworkName = this.Services.Network.Name,
                    NetworkDomain = this.Services.Lang.T("AppDomain"),
                };
                var rules = this.Services.Subscriptions.GetEmailSubstitutionRules();
                textModel.Title = this.Services.I18N.ReplaceVariables(textModel.Title, subModel, rules);
                textModel.Value = this.Services.I18N.ReplaceVariables(textModel.Value, subModel, rules);

                this.SendUserCustom(this.ClassAProvider, textModel, user);
            }
        }

        public void SendSubscriptionEnded(SubscriptionModel subscription, UserModel user)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");
            if (user == null)
                throw new ArgumentNullException("user");

            var template = this.Services.SubscriptionTemplates.GetById(subscription.TemplateId);

            var extraType = subscription.IsActive ? template.RenewText.Name : template.ExpireText.Name;
            var textModel = this.Services.SubscriptionTemplates.GetEditTextRequest(template.Id, this.Services.People.GetCulture(user), extraType);

            if (textModel != null)
            {
                var subModel = new SubscriptionEmailModel
                {
                    Subscription = subscription,
                    Template = template,
                    User = user,
                    NetworkName = this.Services.Network.Name,
                    NetworkDomain = this.Services.Lang.T("AppDomain"),
                };
                var rules = this.Services.Subscriptions.GetEmailSubstitutionRules();
                textModel.Title = this.Services.I18N.ReplaceVariables(textModel.Title, subModel, rules);
                textModel.Value = this.Services.I18N.ReplaceVariables(textModel.Value, subModel, rules);

                this.SendUserCustom(this.ClassAProvider, textModel, user);
            }
        }

        public void SendInvitationWithApply(UserModel me, UserModel invited, string invitationLink)
        {
            if (me == null)
                throw new ArgumentNullException("me");
            if (invited == null)
                throw new ArgumentNullException("invited");

            var culture = this.Services.People.GetCulture(invited);
            var tz = this.Services.People.GetTimezone(invited);

            me.Picture = this.Services.People.GetProfilePictureUrl(me.Login, me.PictureName, Sparkle.Services.Networks.Users.UserProfilePictureSize.Medium, UriKind.Absolute);
            var emailModel = new BaseEmailModel<UserModel>(
                invited.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                me);
            emailModel.Data["InviteLink"] = invitationLink ?? this.Services.People.GetInviteWithApplyUrl(me.Id, this.Services.NetworkId);
            emailModel.Data["PeopleLink"] = this.Services.People.GetProfileUrl(me, UriKind.Absolute);
            emailModel.Data["AboutNetwork"] = this.Services.Network.About;

            var body = this.Services.EmailTemplateProvider.Process("InvitationWithApply", null, culture, tz, emailModel);
            var emailMessage = this.CreateMessage(this.Services, this.ClassASender, invited.GetEmailContact(), this.Services.Lang.T("{0} vous invite à rejoindre {1}", me.DisplayName, this.Services.Lang.T("AppName")), body);
            this.SendMail(this.ClassAProvider, "SendInvitationWithApply", emailMessage);
        }

        public void SendTests(List<string> usernames)
        {
            var users = this.Services.Repositories.People.GetUsersViewByLogin(usernames.ToArray());
            var userModels = users.Values.Select(x => new UserModel(x)).Where(x => x.IsActive).ToList();
            var persons = userModels
                .Select(u => new Sparkle.Entities.Networks.Neutral.Person
                {
                    Id = u.Id,
                    Email = u.Email.Value,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Login,
                    Culture = u.Culture,
                    Timezone = u.Timezone,
                    CompanyId = u.CompanyId,
                    CompanyName = u.CompanyName,
                })
                .ToList();
            var senders = new List<Tuple<IEmailProvider, EmailContact, string>>(3);
            senders.Add(new Tuple<IEmailProvider, EmailContact, string>(this.ClassAProvider, this.ClassASender, "Class A email send test."));
            senders.Add(new Tuple<IEmailProvider, EmailContact, string>(this.ClassBProvider, this.ClassBSender, "Class B email send test."));
            senders.Add(new Tuple<IEmailProvider, EmailContact, string>(this.ClassCProvider, this.ClassCSender, "Class C email send test."));

            foreach (var user in userModels)
            {
                foreach (var sender in senders)
                {
                    var model = new TechnicalMessagesModel(user.Email.Value, this.Services.Lang.T("AccentColor"), this.Services.Lang)
                    {
                        Messages = new List<string>() { sender.Item3, },
                        Recipients = persons,
                    };
                    model.Initialize(this.Services);
                    string body = this.Services.EmailTemplateProvider.Process("TechnicalMessages", null, this.Services.People.GetCulture(user), this.Services.People.GetTimezone(user), model);
                    var message = this.CreateMessage(this.Services, sender.Item2, new EmailContact(user.Email.Value, user.DisplayName), "Messages techniques", body);
                    this.SendMail(sender.Item1, "TechnicalMessages", message);
                }
            }
        }

        public EmailSendResult TestSend(string providerConfigurationKey, string recipient)
        {
            if (string.IsNullOrEmpty(providerConfigurationKey))
                throw new ArgumentException("The value cannot be empty", "providerConfigurationKey");
            if (string.IsNullOrEmpty(recipient))
                throw new ArgumentException("The value cannot be empty", "recipient");

            if (!providerConfigurationKey.StartsWith("Providers.Emails."))
                throw new ArgumentException("The configuration key does not match the required format", "providerConfigurationKey");
            if (!this.Services.AppConfiguration.Values.ContainsKey(providerConfigurationKey))
                throw new ArgumentException("The value does not match an existing configuration key", "providerConfigurationKey");

            var configurationValue = this.Services.AppConfiguration.Values[providerConfigurationKey];
            using (var provider = this.CreateProvider(providerConfigurationKey))
            {
                var message = new MailMessage();
                message.From = this.ClassASender.ToMailAddress();
                message.To.Add(new MailAddress(recipient));
                message.Subject = "Test email from " + this.Services.Lang.T("AppName");
                message.IsBodyHtml = false;
                message.Body = @"Hello, this is a technical message from " + this.Services.Lang.T("AppName") + @".

An email send test was performed and the result was to be sent to " + recipient + @".

If you receive this email, then the """ + providerConfigurationKey + @""" provider is well configured.
";
                var result = this.SendMail(provider, "TestSend", message);
                return result;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.classAProvider != null)
                    {
                        this.classAProvider.Dispose();
                        this.classAProvider = null;
                    }

                    if (this.classBProvider != null)
                    {
                        this.classBProvider.Dispose();
                        this.classBProvider = null;
                    }

                    if (this.classCProvider != null)
                    {
                        this.classCProvider.Dispose();
                        this.classCProvider = null;
                    }
                }

                this.disposed = true;
            }
        }

        private void SendRegistered(RegisterEmailModel model)
        {
            model.Initialize(this.Services);
            string body = null;
            body = this.Services.EmailTemplateProvider.Process("Register", null, this.Services.DefaultCulture, this.Services.Context.Timezone, model);
            var message = this.CreateMessage(this.Services, this.ClassASender, new EmailContact(model.RecipientEmailAddress, model.FirstName), "Vous avez rejoint " + this.Services.Lang.T("AppName") + " !", body);

            this.SendMail(this.ClassBProvider, "SendRegistred", message);
        }

        private ItemCaptureResult ExtractItemId(string html, out string capture)
        {
            var match = regexItemId.Match(html);
            if (match.Groups[1].Success)
            {
                capture = match.Groups[1].Value;
                return ItemCaptureResult.TimelineItem;
            }

            match = regexPrivateMessageId.Match(html);
            if (match.Groups[1].Success)
            {
                capture = match.Groups[1].Value;
                return ItemCaptureResult.PrivateMessage;
        }

            capture = string.Empty;
            return ItemCaptureResult.None;
        }

        /// <param name="emailClassProviderKey">can be "Providers.Emails.ClassA", "Providers.Emails.ClassB", "Providers.Emails.ClassC"</param>
        private IEmailProvider CreateProviderByClass(string emailClassProviderKey)
        {
            var classConfig = this.Services.AppConfiguration.Values[emailClassProviderKey];
            if (classConfig == null)
                throw new InvalidOperationException("There is no email provider configuration for email class '" + emailClassProviderKey + "'");

            var classConfigValue = classConfig.RawValue ?? classConfig.DefaultRawValue;
            return this.CreateProvider(classConfigValue);
        }

        /// <summary>
        /// Create a provider instance from the specified configuration key.
        /// </summary>
        /// <remarks>
        /// the configuration value should be in the following format: "<.NET type name>, <configuration string>"
        /// the <.NET type name> part should be the fully qualified type name of a class implementing IEmailProvider.
        /// the <configuration string> part is provider dependant.
        /// </remarks>
        /// <param name="emailProviderKey">something like "Providers.Emails.SomeApiProvider"</param>
        /// <returns></returns>
        private IEmailProvider CreateProvider(string emailProviderKey)
        {
            var providerConfig = this.Services.AppConfiguration.Values[emailProviderKey];
            if (providerConfig == null)
                throw new InvalidOperationException("There is no email provider configuration for key '" + emailProviderKey + "'");

            var providerConfigValue = providerConfig.RawValue ?? providerConfig.DefaultRawValue;
            var providerTypeName = providerConfigValue.Substring(0, providerConfigValue.IndexOf(','));
            var providerType = Type.GetType(providerTypeName);
            if (providerType == null)
                throw new InvalidOperationException("Email provider '" + emailProviderKey + "' specifies a provider type '" + providerTypeName + "' that is not loaded");

            var provider = (IEmailProvider)Activator.CreateInstance(providerType);
            provider.Initialize(this.Services);
            var providerConfiguration = providerConfigValue.Substring(providerConfigValue.IndexOf(',') + 1).Trim();
            provider.Configure(providerConfiguration);
            return provider;
        }

        private void SendAdminWorkEmail(AdminWorksModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            foreach (var recipient in model.Recipients)
            {
                var recipientModel = model.For(recipient);
                var culture = this.Services.People.GetCulture(recipientModel.SelfRecipient.User);
                var tz = this.Services.People.GetTimezone(recipientModel.SelfRecipient.User);
                string subject = null;
                using (new ThreadCulture(culture, culture))
                {
                    if (recipientModel.Items != null)
                    {
                        var task = recipientModel.Items
                            .Where(t => t.Priority == AdminWorkPriority.Current)
                            .FirstOrDefault();
                        if (task != null)
                        {
                            subject = task.TaskTitle;
                        }
                    }
                }

                if (subject == null)
                    subject = NetworksLabels.AdminWorksDefaultSubject;
                subject += " (" + this.Services.Lang.T("AppName") + ")";

                var emailModel = new BaseEmailModel<AdminWorksModel>(
                    recipientModel.SelfRecipient.User.GetSimpleContact(),
                    this.Services.Lang.T("NetworkAccentColor"),
                    this.Services.Lang,
                    recipientModel);
                var body = this.Services.EmailTemplateProvider.Process("AdminWorks", null, culture, tz, emailModel);
                var message = this.CreateMessage(this.Services, this.ClassBSender, recipientModel.SelfRecipient.Contact, subject, body);
                this.SendMail(this.ClassBProvider, "SendAdminWorkEmail", message);
            }
        }

        private void SendUserCustom(IEmailProvider provider, EditTextRequest model, UserModel receiver)
        {
            if (model == null || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Value))
                throw new ArgumentNullException("model");
            if (receiver == null)
                throw new ArgumentNullException("receiver");

            var culture = this.Services.People.GetCulture(receiver);
            var tz = this.Services.People.GetTimezone(receiver);

            var emailModel = new BaseEmailModel<EditTextRequest>(
                receiver.GetSimpleContact(),
                this.Services.Lang.T("AccentColor"),
                this.Services.Lang,
                model);

            var body = this.Services.EmailTemplateProvider.Process("UserCustom", null, culture, tz, emailModel);
            var emailMessage = this.CreateMessage(this.Services, this.ClassASender, receiver.GetEmailContact(), model.Title, body);
            this.SendMail(provider, "SendUserCustom", emailMessage);
        }

        private void LogSendSuccess(string path, MailMessage message)
        {
            string msg = "To: {0}\r\nFrom: {1}\r\nSubject: {2}";
            this.Services.Logger.Info(path, ErrorLevel.Success, msg, message.To.ToString(), message.From.ToString(), message.Subject);
        }

        private void LogSendError(string path, MailMessage message, Exception ex)
        {
            string msg = "To: {0}\r\nFrom: {1}\r\nSubject: {2}\r\nException: {3}: {4}";
            this.Services.Logger.Error(path, ErrorLevel.ThirdParty, msg, message.To.ToString(), message.From.ToString(), message.Subject, ex.GetType().Name, ex.Message);
        }

        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public MailMessage CreateMessage(IServiceFactory services, EmailContact from, EmailContact to, string subject, string body)
        {
            MailMessage message = new MailMessage
            {
                IsBodyHtml = true,
            };

            if (from != null)
            {
                if (to.DisplayName != null)
                    message.From = new MailAddress(from.EmailAddress, from.DisplayName);
                else
                    message.From = new MailAddress(from.EmailAddress);
            }
            else
            {
                ////message.From = new MailAddress(from, services.Lang.T("AppName"));
                if (from == null)
                    throw new ArgumentNullException("from");
            }

            if (to != null)
            {
                if (to.DisplayName != null)
                    message.To.Add(new MailAddress(to.EmailAddress, to.DisplayName));
                else
                    message.To.Add(new MailAddress(to.EmailAddress));
            }
            else
            {
                if (to == null)
                    throw new ArgumentNullException("to");
            }

            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            message.Subject = subject.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            lock (uidLock)
            {
                message.Headers.Add("Message-ID", "<" + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) + Environment.TickCount + uid++ + EMAILDOMAIN + ">");
            }

            return message;
        }
    }

    public static class EmailTemplates
    {
        public const string Welcome = "Welcome";
        public const string Event = "EventTemplate";
        public const string Group = "GroupTemplate";
        public const string PrivateMessage = "PrivateMessageTemplate";
        public const string SeekFriend = "SeekFriendTemplate";
        public const string ContactAccepted = "ContactAccepted";
        public const string RegisterInvitation = "InvitationTemplate";
        public const string Comment = "CommentTemplate";
        public const string Feedback = "Feedback";
        public const string Weside = "Weside";
        public const string Contact = "Contact";
        public const string ProposeEat = "ProposeEat";
        public const string ThisWeek = "ThisWeek";
        public const string Recovery = "Recovery";
        public const string MailChimpReport = "MailChimpReport";
        public const string Recall = "Recall";
        public const string RegisterRequest = "RegisterRequest";
    }
}

