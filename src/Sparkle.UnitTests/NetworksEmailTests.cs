
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Sparkle.EmailTemplates;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.EmailTemplates;
    using Sparkle.Services.Main.EmailModels;
    using Sparkle.Services.Networks.EmailModels;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.PartnerResources;
    using Sparkle.Services.Networks.Timelines;
    using Sparkle.UI;
    using Sparkle.UnitTests.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    public class NetworksEmailTests
    {
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

        private static DefaultEmailTemplateProvider provider = new DefaultEmailTemplateProvider();

        public static void InitializeModel(BaseEmailModel model)
        {
            model.AppConfiguration = new Infrastructure.AppConfiguration(new Infrastructure.Application(), new Dictionary<string, Infrastructure.Data.AppConfigurationEntry>() { 
                { "Features.EnableCompanies", new Infrastructure.Data.AppConfigurationEntry() { RawValue = "true", } },
            });
        }

        [TestClass]
        public class AddResume
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void AddResumeTest()
            {
                var FirstName = "TotoALaPiscine";
                var Email = "toto@titi.tata";
                var Pin = "TotoPin";
                var model = new AddResumeConfirmationEmailModel(Email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Confirmation de l'enregistrement de votre profil",
                    FirstName = FirstName,
                    Id = 42,
                    Pin = Pin,
                };
                var result = provider.Process("AddResume", null, null, tz, model);

                Assert.IsTrue(result.Contains(FirstName));
                Assert.IsTrue(result.Contains(Email));
            }
        }
        /*
        [TestClass]
        public class Comment
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod, Ignore]
            public void CommentTest()
            {
                var title = "nouveau commentaire";
                var firstname = "toto";
                var lastname = "titi";
                var profileurl = Lang.T("AppDomain") + "Person/toto";
                var originalpost = "coucou je suis un commentaire !";
                var comment = "ah non cest moi le comment";
                var model = new CommentEmailModel("toto@titi.tata", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    LastName = lastname,
                    ProfileUrl = profileurl,
                    OriginalPost = originalpost,
                    ContactFirstName = firstname,
                    ContactLastName = lastname,
                    ContactUrl = Lang.T("AppDomain") + "Person/toto",
                    Comment = comment,
                    PublicationUrl = Lang.T("AppDomain") + "Ajax/Item/42",
                    TimelineItem = new Entities.Networks.TimelineItem // Test is ignored because we cannot build this manually
                    {
                        Text = "timeline content",
                    },
                    TimelineItemComments = new List<Sparkle.Entities.Networks.TimelineItemComment>()
                    {
                        new Sparkle.Entities.Networks.TimelineItemComment // Same here
                        {
                            Text = "comment 1",
                            PostedBy = new Entities.Networks.User
                            {
                                Login = "user.comment.1",
                                FirstName = "yufghjk,l",
                                LastName = "inokl,"
                            }
                        },
                    },
                };
                var result = provider.Process("Comment", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(profileurl));
                Assert.IsTrue(result.Contains(originalpost));
                Assert.IsTrue(result.Contains(comment));
                Assert.IsTrue(result.Contains(model.TimelineItem.Text));
                Assert.IsTrue(result.Contains(model.TimelineItemComments[0].Text));
            }
        }
        */
        [TestClass]
        public class Communication
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CommunicationTest()
            {
                var firstname = "toto";

                var culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { Firstname = firstname, EmailAddress = "test@test.com", };
                var title = "nouveau communicatin email";
                var message = "salut je suis une nouvelle communication.. mais quoi ?";
                ////var model = new CommunicationEmailModel("toto@titi.tata", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                ////{
                ////    FirstName = firstname,
                ////    Title = title,
                ////    Message = message,
                ////};
                var parentModel = new BaseEmailModel<string>(recipient, Lang.T("AccentColor"), Lang.Source, message);
                parentModel.Title = title;
                parentModel.RecipientContact = new SimpleContact
                {
                    Firstname = firstname,
                };
                var result = provider.Process("Communication", null, null, tz, parentModel);

                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(message));
            }
        }

        [TestClass]
        public class CompanyContact
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompanyContactTest()
            {
                var title = "message de la société";
                var touser = "toto";
                var tocompany = "granola";
                var firstname = "firstname";
                var model = new CompanyContactEmailModel("toto@titi.com", Lang.T("#ff6666"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    ToUser = new Entities.Networks.User() { FirstName = touser },
                    ToCompany = new Entities.Networks.Company() { Name = tocompany },
                    FromUserName = firstname,
                    FromUserEmail = "toto@titi.com",
                    FromCompanyName = tocompany,
                    Message = "coucou je suis un nouveau companycontact",
                    ConversationUrl = Lang.T("AppDomain") + "Companies/Messages"
                };
                var result = provider.Process("CompanyContact", null, null, tz, model);

                Assert.IsTrue(result.Contains(touser));
                Assert.IsTrue(result.Contains(tocompany));
                Assert.IsTrue(result.Contains(firstname));
            }
        }

        [TestClass]
        public class CompanyRegisteredNotification
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompanyRegisteredNotificationTest()
            {
                var title = "inscription de l'entreprise";
                var name = "toto";
                var alias = "titi";
                var email = "toto@titi.tata";
                var admin = "contact.admin@company.org";
                var other = "contact.other@company.org";
                var model = new CompanyEmailModel("toto@company.titi", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    Sender = new Entities.Networks.User() { FirstName = "firstname", LastName="last-name", Username="usernamme", },
                    Company = new Entities.Networks.Company() { Name = "companyname" },
                    Name = name,
                    Alias = alias,
                    Email = email,
                    AdminEmails = new string[] { admin, },
                    OtherEmails = new string[] { other, },
                };
                var result = provider.Process("CompanyRegisteredNotification", null, null, tz, model);

                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(alias));
                Assert.IsTrue(result.Contains(admin));
                Assert.IsTrue(result.Contains(other));
            }
        }

        [TestClass]
        public class CompanyRequestAccepted
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompanyRequestAcceptedTest()
            {
                var title = "entreprise accepté";
                var name = "toto";
                var alias = "titi";
                var email = "toto@titi.tata";
                var admin = "contact.admin@company.org";
                var other = "contact.other@company.org";
                var model = new CompanyEmailModel("toto@company.titi", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    Company = new Entities.Networks.Company() { Name = "companyname" },
                    Name = name,
                    Alias = alias,
                    Email = email,
                    AdminEmails = new string[] { admin, },
                    OtherEmails = new string[] { other, },
                };
                var result = provider.Process("CompanyRequestAccepted", null, null, tz, model);

                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(alias));
                Assert.IsTrue(result.Contains(admin));
                Assert.IsTrue(result.Contains(other));
            }
        }

        [TestClass]
        public class CompanyRequestConfirmation
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompanyRequestConfirmationTest()
            {
                var title = "confirmation de l'entreprise";
                var name = "toto";
                var alias = "titi";
                var email = "toto@titi.tata";
                var model = new CompanyEmailModel("toto@company.titi", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    CompanyRequest = new Entities.Networks.CompanyRequest(),
                    Name = name,
                    Alias = alias,
                    Email = email,
                    RequestId = 42,
                    RequestUniqueId = System.Guid.Empty,
                };
                var result = provider.Process("CompanyRequestConfirmation", null, null, tz, model);

                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(alias));
            }
        }

        [TestClass]
        public class CompanyRequestRejected
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompanyRequestRejectedTest_WithReason()
            {
                var title = "entreprise rejeté";
                var name = "toto";
                var alias = "titi";
                var email = "toto@titi.tata";
                var reason = "nananananere !";
                var model = new CompanyEmailModel("toto@company.titi", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    CompanyRequest = new Entities.Networks.CompanyRequest(),
                    Name = name,
                    Alias = alias,
                    Email = email,
                    Reason = reason,
                };
                var result = provider.Process("CompanyRequestRejected", null, null, tz, model);

                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(alias));
                Assert.IsTrue(result.Contains(reason));
            }

            [TestMethod]
            public void CompanyRequestRejectedTest_NoReason()
            {
                var title = "entreprise rejeté";
                var name = "toto";
                var alias = "titi";
                var email = "toto@titi.tata";
                var model = new CompanyEmailModel("toto@company.titi", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    CompanyRequest = new Entities.Networks.CompanyRequest(),
                    Name = name,
                    Alias = alias,
                    Email = email,
                    Reason = null,
                };
                var result = provider.Process("CompanyRequestRejected", null, null, tz, model);

                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(alias));
            }
        }

        [TestClass]
        public class CompleteProfile
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void CompleteProfileTest()
            {
                var title = "suggestion";
                var firstname = "toto";
                var lastname = "titi";
                var email = "toto@titi.tata";
                var comment = "cause you s*cks!";
                var model = new CompleteProfileEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    ContactFirstName = firstname,
                    ContactLastName = lastname,
                    ContactUrl = Lang.T("AppDomain") + "Person/toto",
                    CompleteProfileUrl = Lang.T("AppDomain") + "Account/Settings",
                    Comment = comment,
                };
                var result = provider.Process("CompleteProfile", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(comment));
            }
        }

        [TestClass]
        public class ContactRequest
        {
            /*
             *  Exception remain at this point in Sparkle.Entities.Networks.User.Job
             */

            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ContactRequestTest()
            {
                var title = "contact request";
                var firstname = "toto";
                var email = "toto@titi.tata";

                var contact = new Entities.Networks.User()
                    {
                        FirstName = firstname,
                        Email = "lol@gmail.com",
                    };
                var job = new Entities.Networks.Job
                        {
                            Libelle = "",
                        };
                var company = new Sparkle.Entities.Networks.Company();

                var model = new ContactRequestEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    Contact = new UserModel(contact),
                    ContactJob = job,
                    ContactCompany = company,
                };
                InitializeModel(model);
                var result = provider.Process("ContactRequest", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
            }
        }

        [TestClass]
        public class ContactRequestAccepted
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ContactRequestAcceptedTest()
            {
                var title = "contact request accepted";
                var firstname = "toto";
                var email = "toto@titi.tata";
                var model = new ContactRequestEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    Contact = new UserModel(new Entities.Networks.User() { Email = "lol@gmail.com", }),
                };
                var result = provider.Process("ContactRequestAccepted", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
            }
        }

        [TestClass]
        public class Event
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void EventTest()
            {
                var title = "new event";
                var firstname = "toto";
                var lastname = "titi";
                var eventname = "eventoto";
                var eventdesc = "description event";
                var email = "toto@titi.tata";
                var model = new EventEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Title = null;
                model.FirstName = firstname;
                model.ContactFirstName = firstname;
                model.ContactLastName = lastname;
                model.ProfileUrl = Lang.T("AppDomain") + "Person/toto";
                model.EventName = eventname;
                model.EventDate = DateTime.Now;
                model.EventDescription = eventdesc;
                model.EventUrl = Lang.T("AppDomain") + "Event/42";
                model.EventReponseYesUrl = Lang.T("AppDomain") + "Events/EventResponse/42?response=1";
                model.EventReponseMaybeUrl = Lang.T("AppDomain") + "Events/EventResponse/42?response=2";
                model.EventReponseNoUrl = Lang.T("AppDomain") + "Events/EventResponse/42?response=3";

                var result = provider.Process("Event", null, null, tz, model);

                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(eventname));
                Assert.IsTrue(result.Contains(eventdesc));
            }
        }

        [TestClass]
        public class ExternalCompanyContact
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ExternalCompanyContactTest()
            {
                var title = "external company contact";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var message = "bonjour je suis un message external company";
                var model = new ExternalCompanyContactEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FromUserName = firstname,
                    FromUserEmail = email,
                    FromCompanyName = lastname,
                    Message = message,
                    ResponseUrl = Lang.T("AppDomain") + "Companies/Contact/company",
                };

                var result = provider.Process("ExternalCompanyContact", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(email));
                Assert.IsTrue(result.Contains(message));
            }
        }

        [TestClass]
        public class Group
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void GroupTest()
            {
                var firstname = "toto";
                var lastname = "company";
                var name = "company";
                var desc = "bonjour je suis une company";
                var email = "toto@titi.tata";
                var model = new GroupEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = null,
                    FirstName = firstname,
                    ContactFirstName = firstname,
                    ContactLastName = lastname,
                    ProfileUrl = Lang.T("AppDomain") + "Person/toto",
                    GroupName = name,
                    GroupDescription = desc,
                    GroupPicture = Lang.T("AppDomain") + "/Content/Networks/" + Lang.T("AppNameKey") + "/Groups/42.jpg",
                    GroupUrl = Lang.T("AppDomain") + "Group/42"
                };

                var result = provider.Process("Group", null, null, tz, model);

                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(name));
                Assert.IsTrue(result.Contains(desc));
                Assert.IsTrue(result.Contains(email));
            }
        }

        [TestClass]
        public class Invite
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void InviteTest()
            {
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var model = new InviteEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Title = firstname + " " + lastname + " vous invite sur " + Lang.T("AppName");
                model.Contact = new Entities.Networks.User() { FirstName = "tata", LastName = "titi" };
                model.Contact.Picture = Lang.T("AppDomain") + "/Data/PersonPicture/toto";
                model.InvitationUrl = Lang.T("AppDomain") + "Account/Register/invitationkey";
                model.Network = new NetworkModel
                {
                    About = @"# title 1

hello world

## title 2

hello hello hello

[link](http://sparklenetworks.com/)

yep",
                };

                var result = provider.Process("Invite", null, null, tz, model);

                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(email));
                Assert.IsTrue(result.Contains("title 1"));
            }
        }

        [TestClass]
        public class InvitationWithApply
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void InvitationWithApplyTest()
            {
                var recipient = new SimpleContact() { EmailAddress = "toto@titi.tata" };
                var model = new UserModel()
                {
                    FirstName = "Roger",
                    LastName = "Fournier",
                    Picture = "pictureUrl.png",
                };

                var emailModel = new BaseEmailModel<UserModel>(recipient, Lang.T("AccentColor"), Lang.Source, model);
                emailModel.Data["InviteLink"] = "invitelink.url";
                emailModel.Data["PeopleLink"] = "peoplelink.url";
                emailModel.Data["AboutNetwork"] = "super awesome network about kawai";

                var result = provider.Process("InvitationWithApply", null, null, tz, emailModel);

                Assert.IsTrue(result.Contains("Roger Fournier"));
                Assert.IsTrue(result.Contains("pictureUrl.png"));
                Assert.IsTrue(result.Contains("invitelink.url"));
                Assert.IsTrue(result.Contains("peoplelink.url"));
                Assert.IsTrue(result.Contains("super awesome network about kawai"));
            }
        }

        [TestClass]
        public class PendingUserRegistrations
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod, Ignore] // ignored because the feature is not yet fully developped
            public void Works()
            {
                var recipientContact = new SimpleContact
                {
                    Firstname = "toto",
                    Lastname = "tata",
                    EmailAddress = "toto@titi.tata",
                };
                var model = new Sparkle.Services.Networks.EmailModels.PendingUserRegistrationsModel(recipientContact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.PendingUsers = new List<Sparkle.Entities.Networks.User>()
                {
                    new Sparkle.Entities.Networks.User
                    {
                        Email = "newguy1@test.com",
                        FirstName = "New",
                        LastName = "Guy 1",
                        Id = 1,
                    },
                    new Sparkle.Entities.Networks.User
                    {
                        Email = "newguy2@test.com",
                        FirstName = "New",
                        LastName = "Guy 2",
                        Id = 2,
                    },
                };
                var newGuy3 = new Sparkle.Entities.Networks.User
                {
                    Email = "newguy1@test.com",
                    FirstName = "New",
                    LastName = "Guy 3",
                    Id = 3,
                };
                model.NewPendingUser = newGuy3;
                model.PendingUsersCount = 17;

                var result = provider.Process("PendingUserRegistrations", null, null, tz, model);

                Assert.IsTrue(result.Contains(recipientContact.Firstname));
                Assert.IsTrue(result.Contains(recipientContact.Lastname));
                Assert.IsTrue(result.Contains(recipientContact.EmailAddress));
                ////Assert.IsTrue(result.Contains(placename));
                ////Assert.IsTrue(result.Contains(desc));
            }
        }

        [TestClass]
        public class NewCompanyDetailsForApproval
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void NewCompanyDetailsForApprovalTest()
            {
                var title = "je suis un titre company pproval";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var model = new CompanyEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    Company = new Entities.Networks.Company(),
                    Name = firstname,
                    Alias = lastname,
                    Email = email,
                    CompanyId = 42,
                    RequestId = 12,
                    RequestUniqueId = System.Guid.Empty,
                };
                var result = provider.Process("NewCompanyDetailsForApproval", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(email));
            }
        }

        [TestClass]
        public class NewUserConfirmEmail
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void NewUserConfirmationEmailTest()
            {

            }
        }

        [TestClass]
        public class PrivateMessage
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PrivateMessageTest()
            {
                var title = "je suis un nouveau private messag";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var subject = "coucou t'as un message";
                var text = "eh oui un message de test";
                var model = new PrivateMessageEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    ContactFirstName = firstname,
                    ContactLastName = lastname,
                    ContactUrl = Lang.T("AppDomain") + "Person/toto",
                    Subject = subject,
                    Message = text,
                    ConversationUrl = Lang.T("AppDomain") + "Conversations/toto"
                };
                var result = provider.Process("PrivateMessage", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(email));
                Assert.IsTrue(result.Contains(model.ConversationUrl));
            }
        }

        [TestClass]
        public class Proposal
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ProposalTest()
            {
                var title = "je suis un nouveau private messag";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var text = "eh oui un message de test";
                var message = "eh oui un test de message";
                var model = new ProposalEmailModel(email, Lang.T("accentcolor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    EmailText = text,
                    FirstName = firstname,
                    ContactFirstName = firstname,
                    ContactLastName = lastname,
                    ContactUrl = Lang.T("appdomain") + "person/toto",
                    Message = message,
                    ConversationUrl = Lang.T("appdomain") + "conversations/toto",
                    Time = DateTime.Now.ToString("h:mm"),
                };
                var result = provider.Process("Proposal", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(lastname));
                Assert.IsTrue(result.Contains(email));
                Assert.IsTrue(result.Contains(text));
                Assert.IsTrue(result.Contains(message));
            }
        }

        [TestClass]
        public class Publication
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PublicationSimpleTest()
            {
                var title = "je suis un nouveau message de publication";
                var pubtitle = "titre de publication";
                var email = "toto@titi.tata";
                var text = "je suis une publication";
                var linkname = "link publication";
                var subject = "sujet de publication";
                var timeline = new BasicTimelineItemModel
                {
                    Id = 1,
                    DateUtc = new DateTime(2014, 4, 12, 16, 52, 00, DateTimeKind.Utc),
                    LikesCount = 12,
                    PostedByName = "Antoine Sottiau",
                    PostedByPictureUrl = "http://some1network.com/Data/PersonPicture/antoine.sottiau/Small",
                    PostedByUrl = "http://some1network.com/Person/antoine.sottiau",
                    Text = text,
                    Url = "http://some1network.com/Ajax/Item/11029",
                    IsUserAuthorized = true,
                };
                timeline.ApplyColors(Entities.Networks.TimelineType.Profile);
                var model = new PublicationEmailModel(email, Lang.T("accentcolor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    PublicationTitle = pubtitle,
                    TimelineItem = timeline,
                    Subject = subject,
                    LinkName = linkname,
                    LinkUrl = Lang.T("AppDomain") + "Link/lol",
                };
                var result = provider.Process("Publication", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(title, result);
                SrkToolkit.Testing.Assert.Contains(email, result);
                SrkToolkit.Testing.Assert.Contains(text, result);
                SrkToolkit.Testing.Assert.Contains(linkname, result);
                SrkToolkit.Testing.Assert.Contains(timeline.PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(timeline.Text, result);
            }

            [TestMethod]
            public void PublicationCommentsTest()
            {
                var title = "je suis un nouveau message de publication";
                var pubtitle = "titre de publication";
                var email = "toto@titi.tata";
                var text = "je suis une publication";
                var linkname = "link publication";
                var subject = "sujet de publication";
                var timeline = new BasicTimelineItemModel
                {
                    Id = 1,
                    DateUtc = new DateTime(2014, 4, 12, 16, 52, 00, DateTimeKind.Utc),
                    LikesCount = 12,
                    PostedByName = "Some One",
                    PostedByPictureUrl = "http://some1network.com/Data/PersonPicture/some.one/Small",
                    PostedByUrl = "http://some1network.com/Person/some.one",
                    Text = text,
                    Url = "http://some1network.com/Ajax/Item/11029",
                    IsUserAuthorized = true,
                };
                timeline.ApplyColors(Entities.Networks.TimelineType.Profile);
                timeline.Items = new List<BasicTimelineItemModel>();
                var baseComment = new BasicTimelineItemModel
                {
                    Id = 1,
                    DateUtc = new DateTime(2014, 4, 12, 16, 52, 00, DateTimeKind.Utc),
                    LikesCount = 12,
                    PostedByName = "Another One",
                    PostedByPictureUrl = "http://some1network.com/Data/PersonPicture/another.one/Small",
                    PostedByUrl = "http://some1network.com/Person/another.one",
                    Text = "comment nice ",
                    Url = "http://some1network.com/Ajax/Item/11029#1",
                    IsUserAuthorized = true,
                };
                timeline.ApplyColors(Entities.Networks.TimelineType.Profile);
                var json = JsonConvert.SerializeObject(baseComment);
                for (int i = 0; i < 5; i++)
                {
                    var item = JsonConvert.DeserializeObject<BasicTimelineItemModel>(json);
                    item.Id += i;
                    item.Text += i + " allo";
                    timeline.Items.Add(item);
                }

                var model = new PublicationEmailModel(email, Lang.T("accentcolor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    PublicationTitle = pubtitle,
                    TimelineItem = timeline,
                    Subject = subject,
                    LinkName = linkname,
                    LinkUrl = Lang.T("AppDomain") + "Link/lol",
                };
                var result = provider.Process("Publication", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(title, result);
                SrkToolkit.Testing.Assert.Contains(email, result);
                SrkToolkit.Testing.Assert.Contains(text, result);
                SrkToolkit.Testing.Assert.Contains(linkname, result);
                SrkToolkit.Testing.Assert.Contains(timeline.PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(timeline.Text, result);
                SrkToolkit.Testing.Assert.Contains("comment nice 0", result);
                SrkToolkit.Testing.Assert.Contains("comment nice 4", result);
            }
        }

        [TestClass]
        public class RecoverPassword
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void RecoverPasswordTest()
            {
                var title = "je suis un message de reinit mdp";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var link = "http://recoverpassword.link";
                var model = new RecoverPasswordEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    FirstName = firstname,
                    Login = lastname,
                    Email = email,
                    Link = link,
                };
                var result = provider.Process("RecoverPassword", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(email));
                Assert.IsTrue(result.Contains(link));
            }
        }

        [TestClass]
        public class Register
        {
            private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void RegisterTest()
            {
                var title = "message de bienvenue";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var link = "http://recoverpassword.link";
                var model = new RegisterEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Title = title;
                model.FirstName = firstname;
                model.Login = lastname;
                var result = provider.Process("Register", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(email));
            }

            [TestMethod]
            public void RegisterWithMessage()
            {
                var title = "message de bienvenue";
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var link = "http://recoverpassword.link";
                var model = new RegisterEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Title = title;
                model.FirstName = firstname;
                model.Login = lastname;
                model.Message = "This is markdown.\r\n\r\nA new paragraph. A [link](http://duckduckgo.com/).\r\n\r\nBye.";
                var result = provider.Process("Register", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
                Assert.IsTrue(result.Contains(email));
                SrkToolkit.Testing.Assert.Contains("<p>This is markdown.", result);
                SrkToolkit.Testing.Assert.Contains("href=\"http://duckduckgo.com/\"", result);
                SrkToolkit.Testing.Assert.Contains("<p>Bye.</p>", result);
            }
        }

        [TestClass]
        public class Reminder
        {
            private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ReminderTest()
            {
                var firstname = "toto";
                var lastname = "company";
                var email = "toto@titi.tata";
                var model = new ReminderEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.InvitedEmail = email;
                model.ContactName = firstname + " " + lastname;
                model.ContactLogin = lastname;
                var result = provider.Process("Reminder", null, null, tz, model);

                // No assert tests because Reminder.csrzr doesn't use model vars
            }
        }

        [TestClass]
        public class WeeklyGroupNewsletter
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void WeeklyGroupNewsletterTest()
            {
                var title = "je suis une weeklygroup newsletter";
                var firstname = "toto";
                var email = "toto@titi.tata";
                var model = new GroupNewsletterEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = title,
                    Person = new Entities.Networks.User() { FirstName = firstname, },
                    Groups = new List<Entities.Networks.MemberGroupNewsletterGroup>(),
                    Registered = true,
                };
                var result = provider.Process("WeeklyGroupNewsletter", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
                Assert.IsTrue(result.Contains(firstname));
            }

            [TestMethod]
            public void ShowsTimelines()
            {
                var firstname = "toto";
                var email = "toto@titi.tata";
                var model = new GroupNewsletterEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Vos groupes",
                    Person = new Entities.Networks.User() { FirstName = firstname, },
                    Groups = new List<Entities.Networks.MemberGroupNewsletterGroup>(),
                    Registered = true,
                };

                for (int groupId = 0; groupId < 2; groupId++)
                {
                    var group = new Entities.Networks.MemberGroupNewsletterGroup
                    {
                        Group = new Entities.Networks.Group
                        {
                            Id = groupId,
                            Name = "Groupe #" + groupId,
                        },
                        PendingRequestsCount = groupId,
                        Timeline = new BasicTimelineItemModel
                        {
                            IsRootNode = true,
                            Items = new List<BasicTimelineItemModel>(),
                        },
                    };
                    model.Groups.Add(group);

                    for (int itemId = 0; itemId < 2; itemId++)
                    {
                        var itemUniqueId = ((1 + groupId) * 100) + (itemId + 1);
                        var item = new BasicTimelineItemModel
                        {
                            ForegroundColor = "#f00",
                            CommentsCount = 2,
                            DateUtc = DateTime.UtcNow,
                            Id = itemUniqueId,
                            LikesCount = 3,
                            PostedByName = "Rémi Palet",
                            PostedByPictureUrl = "http://some1network.com/Data/PersonPicture/remi.palet/Small",
                            PostedByUrl = "http://some1network.com/Person/remi.palet",
                            PostedIntoContainerName = "groupe",
                            PostedIntoName = group.Group.Name,
                            PostedIntoUrl = "http://some1network.com/Group/" + groupId,
                            PostedIntoVerb = "dans",
                            Text = "hello world G" + groupId + " I" + itemId + " lalalalalalal",
                            Type = Entities.Networks.TimelineType.Group,
                            Url = "http://some1network.com/Ajax/Item/" + itemUniqueId,
                            Items = new List<BasicTimelineItemModel>(),
                            IsUserAuthorized = true,
                        };
                        item.ApplyColors(Entities.Networks.TimelineType.Group);
                        group.Timeline.Items.Add(item);

                        for (int commentId = 0; commentId < 2; commentId++)
                        {
                            var commentUniqueId = ((1 + groupId) * 1000) + ((itemId + 1) * 100) + commentId;
                            var comment = new BasicTimelineItemModel
                            {
                                DateUtc = DateTime.UtcNow,
                                Id = commentUniqueId,
                                IsUserAuthorized = true,
                                LikesCount = commentId,
                                PostedByName = "Some One",
                                PostedByPictureUrl = "http://some1network.com/Data/PersonPicture/some.one/Small",
                                PostedByUrl = "http://some1network.com/Person/some.one",
                                Text = "super comnt G" + groupId + " I" + itemId + " C" + commentId + " bla",
                                Url = "http://some1network.com/Ajax/Item/" + itemUniqueId + "#comment" + commentUniqueId,
                            };
                            comment.ApplyColors(Entities.Networks.TimelineType.Profile);
                            item.Items.Add(comment);
                        }
                    }
                }

                var result = provider.Process("WeeklyGroupNewsletter", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains("hello world G0 I0 lalala", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G0 I0 C0 bla", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G0 I0 C1 bla", result);
                SrkToolkit.Testing.Assert.Contains("hello world G0 I1 lalala", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G0 I1 C0 bla", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G0 I1 C1 bla", result);
                SrkToolkit.Testing.Assert.Contains("hello world G1 I0 lalala", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G1 I0 C0 bla", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G1 I0 C1 bla", result);
                SrkToolkit.Testing.Assert.Contains("hello world G1 I1 lalala", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G1 I1 C0 bla", result);
                SrkToolkit.Testing.Assert.Contains("super comnt G1 I1 C1 bla", result);
                SrkToolkit.Testing.Assert.Contains("http://some1network.com/Ajax/Item/101", result);
                SrkToolkit.Testing.Assert.Contains("http://some1network.com/Ajax/Item/102", result);
                SrkToolkit.Testing.Assert.Contains("http://some1network.com/Ajax/Item/201", result);
                SrkToolkit.Testing.Assert.Contains("http://some1network.com/Ajax/Item/202", result);
            }
        }

        [TestClass]
        public class WeeklyNewsletter
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void WeeklyNewsletterTest()
            {
                var title = "je suis une weekly newsletter";
                var email = "toto@titi.tata";
                var model = new Sparkle.Services.EmailModels.NewsletterEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Title = title;
                model.HasNewRegistrants = false;
                model.NewRegistrants = new List<Sparkle.Services.EmailModels.NewRegistrant>();
                model.CountNewRegistrants = 0;
                model.OtherCountNewRegistrants = 0;
                model.HasEvents = false;
                model.Events = new List<Sparkle.Services.Networks.Models.EventModel>();
                model.HasOtherEvents = false;
                model.OtherEvents = new List<Sparkle.Services.Networks.Models.EventModel>();
                model.HasCompaniesPublications = false;
                model.CompaniesPublications = new List<Services.EmailModels.CompanyPublication>();
                model.HasNewGroups = false;
                model.NewGroups = new List<Services.EmailModels.NewGroup>();
                model.Subscriber = new Services.Objects.WeeklyMailSubscriber();
                model.Registered = false;
                model.RecipientInvitedCode = "42XXXEnterprise";
                model.Incubators = new List<Data.Networks.Objects.CompanyExtended>();
                model.Startups = new List<Data.Networks.Objects.CompanyExtended>();
                model.Companies = new List<Data.Networks.Objects.CompanyExtended>();
                model.AdsTotal = 0;
                model.TrackerDisplay = "trackerDisplay";
                model.TrackerFollow = "trackerFollow";
                InitializeModel(model);
                var result = provider.Process("WeeklyNewsletter", null, null, tz, model);

                Assert.IsTrue(result.Contains(title));
            }

            [TestMethod]
            public void ShowCompaniesTimelineItems()
            {
                var email = "toto@titi.tata";
                var model = new Sparkle.Services.EmailModels.NewsletterEmailModel(email, Lang.T("#CC2222"), Sparkle.UI.Lang.Source);

                model.Title = "C'est Lundi à Supertechnologies";
                InitializeModel(model);
                model.Registered = true;
                model.AppConfiguration.Values["Features.EnableCompanies"].RawValue = "True";

                model.HasCompaniesPublications = true;
                model.CompaniesTimeline = new BasicTimelineItemModel
                {
                    IsRootNode = true,
                    Items = new List<BasicTimelineItemModel>(),
                };
                model.CompaniesTimeline.Items.Add(new BasicTimelineItemModel
                {
                    DateUtc = new DateTime(2014, 9, 2, 12, 45, 7, DateTimeKind.Utc),
                    Id = 1001,
                    LikesCount = 20,
                    PostedByName = "Company 67",
                    PostedByPictureUrl = "http://some1network.com/Data/CompanyPicture/sparklenetworks",
                    PostedByUrl = "http://some1network.com/Company/sparklenetworks",
                    Text = "hello les gens\r\n\r\nnous organisons un pot a l'occasion de notre __101ieme client__.",
                    Url = "http://some1network.com/Ajax/Item/1001",
                    Type = Entities.Networks.TimelineType.Company,
                    IsUserAuthorized = true,
                });
                model.CompaniesTimeline.Items.Add(new BasicTimelineItemModel
                {
                    DateUtc = new DateTime(2014, 9, 6, 12, 45, 7, DateTimeKind.Utc),
                    Id = 1002,
                    LikesCount = 21,
                    PostedByName = "Company 67",
                    PostedByPictureUrl = "http://some1network.com/Data/CompanyPicture/sparklenetworks",
                    PostedByUrl = "http://some1network.com/Company/sparklenetworks",
                    Text = "hi le reseau\r\n\r\ndecrouvrer notre nouveau produit http://test.com/.",
                    Url = "http://some1network.com/Ajax/Item/1002",
                    Type = Entities.Networks.TimelineType.Company,
                    IsUserAuthorized = true,
                });

                foreach (var item in model.CompaniesTimeline.Items)
                {
                    item.ApplyColors(item.Type);
                }

                model.TrackerDisplay = "trackerDisplay";
                model.TrackerFollow = "trackerFollow";
                var result = provider.Process("WeeklyNewsletter", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[0].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[0].PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[0].PostedByPictureUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[0].PostedByUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[0].Url, result);
                SrkToolkit.Testing.Assert.Contains("hello les gens", result);
                SrkToolkit.Testing.Assert.Contains("nous organisons", result);
                SrkToolkit.Testing.Assert.Contains("<strong>101ieme client</strong>", result);

                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[1].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[1].PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[1].PostedByPictureUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[1].PostedByUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.CompaniesTimeline.Items[1].Url, result);
                SrkToolkit.Testing.Assert.Contains("hi le resequ", result);
                SrkToolkit.Testing.Assert.Contains("decrouvrer notre nouveau produit", result);
                SrkToolkit.Testing.Assert.Contains("href=\"http://test.com/\"", result);
            }

            [TestMethod]
            public void ShowPeopleTimelineItems()
            {
                var email = "toto@titi.tata";
                var model = new Sparkle.Services.EmailModels.NewsletterEmailModel(email, Lang.T("#CC2222"), Sparkle.UI.Lang.Source);

                model.Title = "C'est Lundi à Supertechnologies";
                InitializeModel(model);
                model.Registered = true;
                model.AppConfiguration.Values["Features.EnableCompanies"].RawValue = "True";

                model.HasPeoplePublications = true;
                model.PeopleTimeline = new BasicTimelineItemModel
                {
                    IsRootNode = true,
                    Items = new List<BasicTimelineItemModel>(),
                };
                model.PeopleTimeline.Items.Add(new BasicTimelineItemModel
                {
                    DateUtc = new DateTime(2014, 9, 2, 12, 45, 7, DateTimeKind.Utc),
                    Id = 1001,
                    LikesCount = 20,
                    PostedByName = "Company 67",
                    PostedByPictureUrl = "http://some1network.com/Data/CompanyPicture/sparklenetworks",
                    PostedByUrl = "http://some1network.com/Company/sparklenetworks",
                    Text = "hello les gens\r\n\r\nnous organisons un pot a l'occasion de notre __101ieme client__.",
                    Url = "http://some1network.com/Ajax/Item/1001",
                    Type = Entities.Networks.TimelineType.Public,
                    IsUserAuthorized = true,
                });
                model.PeopleTimeline.Items.Add(new BasicTimelineItemModel
                {
                    DateUtc = new DateTime(2014, 9, 6, 12, 45, 7, DateTimeKind.Utc),
                    Id = 1002,
                    LikesCount = 21,
                    PostedByName = "Company 67",
                    PostedByPictureUrl = "http://some1network.com/Data/CompanyPicture/sparklenetworks",
                    PostedByUrl = "http://some1network.com/Company/sparklenetworks",
                    Text = "hi le reseau\r\n\r\ndecrouvrer notre nouveau produit http://test.com/.",
                    Url = "http://some1network.com/Ajax/Item/1002",
                    Type = Entities.Networks.TimelineType.Public,
                    IsUserAuthorized = true,
                });

                foreach (var item in model.PeopleTimeline.Items)
                {
                    item.ApplyColors(item.Type);
                }

                model.TrackerDisplay = "trackerDisplay";
                model.TrackerFollow = "trackerFollow";
                var result = provider.Process("WeeklyNewsletter", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[0].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[0].PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[0].PostedByPictureUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[0].PostedByUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[0].Url, result);
                SrkToolkit.Testing.Assert.Contains("hello les gens", result);
                SrkToolkit.Testing.Assert.Contains("nous organisons", result);
                SrkToolkit.Testing.Assert.Contains("<strong>101ieme client</strong>", result);

                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[1].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[1].PostedByName, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[1].PostedByPictureUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[1].PostedByUrl, result);
                SrkToolkit.Testing.Assert.Contains(model.PeopleTimeline.Items[1].Url, result);
                SrkToolkit.Testing.Assert.Contains("hi le resequ", result);
                SrkToolkit.Testing.Assert.Contains("decrouvrer notre nouveau produit", result);
                SrkToolkit.Testing.Assert.Contains("href=\"http://test.com/\"", result);
            }

            [TestMethod]
            public void ShowEventTimelineItems()
            {
                var email = "toto@titi.tata";
                var model = new Sparkle.Services.EmailModels.NewsletterEmailModel(email, Lang.T("#CC2222"), Sparkle.UI.Lang.Source);

                model.Title = "C'est Lundi à Supertechnologies";
                InitializeModel(model);
                model.Registered = true;

                model.HasEvents = true;
                model.Events = new List<EventModel>();

                model.Events.Add(new EventModel
                {
                    Id = 42,
                    DateEventUtc = DateTime.UtcNow,
                    Description = "Hello\r\n\r\nCeci est un super événement de __test__ !!",
                    Name = "Event fictif",
                });
                model.Events.Add(new EventModel
                {
                    Id = 66,
                    DateEventUtc = DateTime.UtcNow,
                    Description = "Bonjour bonjour !  \r\nOn a jamais assez d'événement sur le *réseau* c'est trop bien !",
                    Name = "Event imaginaire",
                });

                model.TrackerDisplay = "trackerDisplay";
                model.TrackerFollow = "trackerFollow";
                var result = provider.Process("WeeklyNewsletter", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.Events[0].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.Events[0].Name, result);
                SrkToolkit.Testing.Assert.Contains("Hello", result);
                SrkToolkit.Testing.Assert.Contains("un super événement", result);
                SrkToolkit.Testing.Assert.Contains("<strong>test</strong>", result);

                SrkToolkit.Testing.Assert.Contains(model.Events[1].Id.ToString(), result);
                SrkToolkit.Testing.Assert.Contains(model.Events[1].Name, result);
                SrkToolkit.Testing.Assert.Contains("Bonjour bonjour !", result);
                SrkToolkit.Testing.Assert.Contains("jamais assez d&#x27;événement", result);
                SrkToolkit.Testing.Assert.Contains("<em>réseau</em>", result);
            }
        }

        [TestClass]
        public class NetworksEmail
        {
            // Test method that checks the existence of test methods for each .csrzr in Sparkle.EmailTemplates
            // Excluding test methods for Test.csrzr and Master.csrzr

            [TestMethod]
            public void NetworksEmailExistenceOfTest()
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom("Sparkle.EmailTemplates.dll");
                string[] res = ass.GetManifestResourceNames();
                var list = res.Where(name => !name.EndsWith("Master.cshtml") && !name.Contains("._")).ToList();
                var missings = new List<string>(res.Length);
                foreach (var name in list)
                {
                    var tmp = name.Split('.');
                    var tmpT = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
                    var lol = (from t in tmpT
                               where t.Name == tmp[3]
                               select t).FirstOrDefault();
                    if (lol == null)
                        missings.Add(tmp[3]);
                }

                if (missings.Count > 0)
                {
                    Assert.Fail("Missing email template test for templates '" + string.Join("', '", missings) + "'");
                }
            }
        }

        [TestClass]
        public class TechnicalMessages
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void TechnicalMessagesTest()
            {
                var email = "toto@titi.tata";
                var model = new TechnicalMessagesModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                model.Messages = new List<string>()
                {
                    "lala",
                    "lolo",
                };
                model.Recipients = new List<Person>()
                {
                    new Person
                    {
                        Username = "user1",
                        FirstName = "first1",
                        LastName = "last1",
                    },
                    new Person
                    {
                        Username = "user2",
                        FirstName = "first2",
                        LastName = "last2",
                    },
                };

                var result = provider.Process("TechnicalMessages", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains("lala", result);
                SrkToolkit.Testing.Assert.Contains("lolo", result);
                SrkToolkit.Testing.Assert.Contains("user1", result);
                SrkToolkit.Testing.Assert.Contains("first1", result);
                SrkToolkit.Testing.Assert.Contains("last1", result);
                SrkToolkit.Testing.Assert.Contains("user2", result);
                SrkToolkit.Testing.Assert.Contains("first2", result);
                SrkToolkit.Testing.Assert.Contains("last2", result);
            }
        }

        [TestClass]
        public class RegisterRequest
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void RegisterRequestTest()
            {
                var email = "toto@titi.tata";
                var contact = new SimpleContact()
                {
                    EmailAddress = email,
                    Firstname = "kitty",
                };
                var company = new CompanyModel
                {
                    Id = 123,
                    Name = "My Minor Company",
                    Alias = "my-minor-company",
                };
                var model = new RegisterRequestEmailModel(contact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Company = company,
                    Request = new RegisterRequestModel
                    {
                        CompanyId = 123,
                        Company = company,
                        DateCreatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        DateUpdatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        EmailAddress = email,
                        Id = 456,
                        NetworkId = 1,
                        StatusCode = Entities.Networks.RegisterRequestStatus.New,
                    },
                    CompanyHasNoAdmin = false,
                    OtherRequestCount = 42424242,
                };
                InitializeModel(model);
                
                var result = provider.Process("RegisterRequest", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(contact.Firstname, result);
                SrkToolkit.Testing.Assert.Contains(model.OtherRequestCount.ToString(), result);
                SrkToolkit.Testing.Assert.Contains("Companies/Management/" + company.Alias, result);
            }
        }

        [TestClass]
        public class RegisterRequestConfirmation
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void RegisterRequestConfirmationTest()
            {
                var email = "toto@titi.tata";
                var contact = new SimpleContact()
                {
                    EmailAddress = email,
                    Firstname = "kitty",
                };
                var company = new CompanyModel
                {
                    Id = 123,
                    Name = "My Minor Company",
                    Alias = "my-minor-company",
                };
                var model = new RegisterRequestEmailModel(contact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Company = company,
                    Request = new RegisterRequestModel
                    {
                        CompanyId = 123,
                        Company = company,
                        DateCreatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        DateUpdatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        EmailAddress = email,
                        Id = 456,
                        NetworkId = 1,
                        StatusCode = Entities.Networks.RegisterRequestStatus.New,
                        Code = Guid.NewGuid(),
                    },
                    CompanyHasNoAdmin = false,
                    OtherRequestCount = 42424242,
                };
                InitializeModel(model);
                
                var result = provider.Process("RegisterRequestConfirmation", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains("Account/RegisterRequestResult/" + model.Request.Code, result);
            }
        }

        [TestClass]
        public class RegisterRequestDenied
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void RegisterRequestDeniedTest()
            {
                var email = "toto@titi.tata";
                var contact = new SimpleContact()
                {
                    EmailAddress = email,
                    Firstname = "kitty",
                };
                var company = new CompanyModel
                {
                    Id = 123,
                    Name = "My Minor Company",
                    Alias = "my-minor-company",
                };
                var model = new RegisterRequestEmailModel(contact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Company = company,
                    Request = new RegisterRequestModel
                    {
                        CompanyId = 123,
                        Company = company,
                        DateCreatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        DateUpdatedUtc = new DateTime(2014, 1, 1, 12, 40, 0, DateTimeKind.Utc),
                        EmailAddress = email,
                        Id = 456,
                        NetworkId = 1,
                        StatusCode = Entities.Networks.RegisterRequestStatus.New,
                        Code = Guid.NewGuid(),
                    },
                };
                InitializeModel(model);
                
                var result = provider.Process("RegisterRequestDenied", null, null, tz, model);
            }
        }

        [TestClass]
        public class Test
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void TestTemplate()
            {
                var email = "toto@titi.tata";
                var contact = new SimpleContact()
                {
                    EmailAddress = email,
                    Firstname = "kitty",
                };
                var model = new SimpleEmailModel(contact, Lang.T("AccentColor"), Sparkle.UI.Lang.Source);
                InitializeModel(model);
                
                var result = provider.Process("Test", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains("hello", result);
            }
        }

        [TestClass]
        public class EmailChangeRequest
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void EmailChangeRequestTest()
            {
                var email = "toto@titi.tata";
                var model = new EmailChangeEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Confirmer votre changement d e-mail",
                    FirstName = "FirstName",
                    Login = "Login",
                    Email = "email@email.com",
                    Link = "http://test.com/test",
                    CreateDateUtc = DateTime.Now.ToString(),
                };

                var result = provider.Process("EmailChangeRequest", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.Title, result);
                SrkToolkit.Testing.Assert.Contains(model.FirstName, result);
                SrkToolkit.Testing.Assert.Contains(model.Email, result);
                SrkToolkit.Testing.Assert.Contains(model.Link, result);
            }
        }

        [TestClass]
        public class PrivateGroupJoinRequest
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PrivateGroupJoinRequestTest()
            {
                var email = "toto@titi.tata";
                var model = new GroupJoinRequestEmailModel(email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Un utilisateur demande a rejoindre le groupe azerty",
                    AdminFirstName = "Admin-FirstName",
                    UserFirstName = "User-FirstName",
                    UserLastName = "User-LastName",
                    GroupName = "azerty",
                    RequestLink = Lang.T("AppDomain") + "Groups/Requests/1",
                };

                var result = provider.Process("PrivateGroupJoinRequest", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.Title, result);
                SrkToolkit.Testing.Assert.Contains(model.AdminFirstName, result);
                SrkToolkit.Testing.Assert.Contains(model.UserFirstName, result);
                SrkToolkit.Testing.Assert.Contains(model.UserLastName, result);
                SrkToolkit.Testing.Assert.Contains(model.RequestLink, result);
            }
        }

        [TestClass]
        public class PrivateGroupJoinResponse
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PrivateGroupJoinResponseTest()
            {
                bool success = true;
                var model = new GroupJoinResponseEmailModel("test@test.com", Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Demande " + (success ? "acceptee ! :)" : "rejetee ! :("),
                    Success = success,
                    FirstName = "kevin",
                    GroupName = "sueprgroupe",
                    GroupLink = Lang.T("AppDomain") + "Group/1",
                    GroupListLink = Lang.T("AppDomain") + "Groups2",
                };

                var result = provider.Process("PrivateGroupJoinResponse", null, null, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.Title, result);
                SrkToolkit.Testing.Assert.Contains(model.FirstName, result);
                SrkToolkit.Testing.Assert.Contains(model.GroupName, result);
                SrkToolkit.Testing.Assert.Contains(model.GroupLink, result);
            }
        }

        [TestClass]
        public class MultipleEmailTemplateGeneration
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void MultipleEmailTemplateGenerationTest()
            {
                var FirstName = "TotoALaPiscine";
                var Email = "toto@titi.tata";
                var Pin = "TotoPin";
                var model = new AddResumeConfirmationEmailModel(Email, Lang.T("AccentColor"), Sparkle.UI.Lang.Source)
                {
                    Title = "Confirmation de l'enregistrement de votre profil",
                    FirstName = FirstName,
                    Id = 42,
                    Pin = Pin,
                };
                string result = "";

                for (int i = 0; i < 100; i++)
                {
                    result = provider.Process("AddResume", null, null, tz, model);
                    Assert.IsTrue(result.Contains(FirstName));
                    Assert.IsTrue(result.Contains(Email));
                }

            }
        }

        [TestClass]
        public class AdminWorks
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void AdminWorksTest()
            {
                bool success = true;
                CultureInfo culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { EmailAddress = "test@test.com", };
                var recipient2 = new SimpleContact { EmailAddress = "toto@test.com", };
                var user = new UserModel { Id = 1, Email = recipient.EmailAddress, FirstName = "Al", LastName = "Fonss", };
                var user2 = new UserModel { Id = 2, Email = recipient2.EmailAddress, FirstName = "Notified", LastName = "Tooo", };
                var emailContact = user.GetEmailContact();
                var emailContact2 = user2.GetEmailContact();
                var levels = new Sparkle.Entities.Networks.NetworkAccessLevel[] { Sparkle.Entities.Networks.NetworkAccessLevel.NetworkAdmin, };
                var workModel = new AdminWorksModel();
                workModel.Recipients = new List<AdminWorkRecipient>();
                workModel.Recipients.Add(new AdminWorkRecipient { User = user, Contact = emailContact, NetworkAccessLevels = levels, });
                workModel.Recipients.Add(new AdminWorkRecipient { User = user2, Contact = emailContact2, NetworkAccessLevels = levels, });
                workModel.DiscloseRecipients = true;
                workModel.Items.Add(new AdminWorkModel(AdminWorkType.NewApplyRequestToManage, AdminWorkTask.AcceptOrRefuse, AdminWorkPriority.Current, new DateTime(2014, 1, 1, 1, 1, 0)));
                workModel.Items.Add(new AdminWorkModel(AdminWorkType.UserEmailChangeRequest, AdminWorkTask.AcceptOrRefuse, AdminWorkPriority.High, new DateTime(2014, 1, 1, 1, 1, 0)));
                workModel.Items.Add(new AdminWorkModel(AdminWorkType.NewApplyRequestToManage, AdminWorkTask.AcceptOrRefuse, AdminWorkPriority.Low, new DateTime(2014, 1, 1, 1, 1, 0)));

                var workModel1 = workModel.For(workModel.Recipients[0]);
                var model = new BaseEmailModel<AdminWorksModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, workModel1)
                {
                    Title = "tyguih fghujii",
                };

                var result = provider.Process("AdminWorks", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains(model.Title, result);
                SrkToolkit.Testing.Assert.Contains("Bonjour Al", result);
                SrkToolkit.Testing.Assert.Contains("Main tasks", result);
                SrkToolkit.Testing.Assert.Contains("Important tasks", result);
                SrkToolkit.Testing.Assert.Contains("Other tasks", result);
                SrkToolkit.Testing.Assert.Contains("Accept or refuse", result);
                SrkToolkit.Testing.Assert.Contains("Register request", result);
                SrkToolkit.Testing.Assert.Contains("Change of email address", result);
                SrkToolkit.Testing.Assert.Contains(workModel.Recipients[0].DisplayName, result);
                SrkToolkit.Testing.Assert.Contains(workModel.Recipients[1].DisplayName, result);
            }
        }

        [TestClass]
        public class ApplyRequestConfirmation
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ApplyRequestConfirmationTest()
            {
                bool success = true;
                CultureInfo culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { EmailAddress = "test@test.com", };

                var model = new Services.Networks.Users.ApplyRequestModel
                {
                    Key = Guid.NewGuid(),
                };
                var secret = model.GetSecretKey();
                var emailModel = new BaseEmailModel<Services.Networks.Users.ApplyRequestModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, model);
                var url = "http://test/Apply/Confirm/" + model.Key + "?Secret=" + Uri.EscapeDataString(secret);
                emailModel.Data["RequestLink"] = url;

                var result = provider.Process("ApplyRequestConfirmation", null, culture, tz, emailModel);

                SrkToolkit.Testing.Assert.Contains("Bonjour", result);
                SrkToolkit.Testing.Assert.Contains(url, result);
            }
        }

        [TestClass]
        public class ApplyRequestAccepted
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ApplyRequestAcceptedTest()
            {
                CultureInfo culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { EmailAddress = "test@test.com", };

                var applyModel = new Sparkle.Services.Networks.Users.ApplyRequestModel();
                applyModel.UserDataModel.User = new UserPoco { FirstName = "Fred", };

                var model = new BaseEmailModel<Sparkle.Services.Networks.Users.ApplyRequestModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, applyModel);
                var url = "http://test/Account/Recovery/kevin.lebas/"
                    + Uri.EscapeDataString(Convert.ToBase64String(Encoding.Unicode.GetBytes(Guid.NewGuid().ToString())));
                model.Data["RequestLink"] = url;

                var result = provider.Process("ApplyRequestAccepted", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains("Bonjour Fred", result);
                SrkToolkit.Testing.Assert.Contains(url, result);
            }
        }

        [TestClass]
        public class ApplyRequestRefused
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void ApplyRequestRefusedTest()
            {
                CultureInfo culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { EmailAddress = "test@test.com", };

                var applyModel = new Sparkle.Services.Networks.Users.ApplyRequestModel();
                applyModel.UserDataModel.User = new UserPoco { FirstName = "Jean", };
                applyModel.RefusedRemark = "Remarque de refus";

                var model = new BaseEmailModel<Sparkle.Services.Networks.Users.ApplyRequestModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, applyModel);

                var result = provider.Process("ApplyRequestRefused", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains("Bonjour Jean", result);
                SrkToolkit.Testing.Assert.Contains("Remarque de refus", result);
            }
        }

        [TestClass]
        public class PartnerResourceProposalAccepted
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PartnerResourceProposalAcceptedTest()
            {
                var culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { Firstname = "monsieur test", EmailAddress = "test@test.com", };

                var partnerModel = new PartnerResourceModel { Name = "partner test", };
                var model = new BaseEmailModel<PartnerResourceModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, partnerModel);
                model.Data["RequestLink"] = "link.test";

                var result = provider.Process("PartnerResourceProposalAccepted", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains("Bonjour monsieur test", result);
                SrkToolkit.Testing.Assert.Contains("partner test", result);
                SrkToolkit.Testing.Assert.Contains("link.test", result);
            }
        }

        [TestClass]
        public class PartnerResourceProposalRefused
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void PartnerResourceProposalRefusedTest()
            {
                var culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { Firstname = "monsieur test", EmailAddress = "test@test.com", };

                var partnerModel = new PartnerResourceModel { Name = "partner test", };
                var model = new BaseEmailModel<PartnerResourceModel>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, partnerModel);

                var result = provider.Process("PartnerResourceProposalRefused", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains("Bonjour monsieur test", result);
                SrkToolkit.Testing.Assert.Contains("partner test", result);
            }
        }

        [TestClass]
        public class UserCustom
        {
            [TestInitialize]
            public void Initialize()
            {
                Lang.Source = Strings.Empty;
            }

            [TestMethod]
            public void UserCustomTest()
            {
                var culture = new CultureInfo("en-US");
                var recipient = new SimpleContact { EmailAddress = "test@test.com", };

                var title = "Titre du mail";
                var message = "Coucou je test un petit mail  \nOn va voir ce que ça donne !\n\n> Et une petite quote\n> Bisous bisous !\n";

                var emailModel = new Sparkle.Services.Networks.Texts.EditTextRequest
                {
                    Title = title,
                    Value = message,
                };
                var model = new BaseEmailModel<Sparkle.Services.Networks.Texts.EditTextRequest>(recipient, Lang.T("AccentColor"), Sparkle.UI.Lang.Source, emailModel);

                var result = provider.Process("UserCustom", null, culture, tz, model);

                SrkToolkit.Testing.Assert.Contains("Titre du mail", result);
                SrkToolkit.Testing.Assert.Contains("Coucou je test un petit mail", result);
                SrkToolkit.Testing.Assert.Contains("On va voir ce que ça donne !", result);
                SrkToolkit.Testing.Assert.Contains("Et une petite quote", result);
                SrkToolkit.Testing.Assert.Contains("Bisous bisous", result);
            }
        }
    }
}
