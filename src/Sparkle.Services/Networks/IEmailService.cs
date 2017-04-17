
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Objects;
    using Sparkle.Services.EmailModels;
    using Sparkle.Services.Networks.EmailModels;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Objects;
    using Sparkle.Entities.Networks.Neutral;
    using Sparkle.Services.Networks.Users;
    using Sparkle.Services.Networks.Subscriptions;

    public interface IEmailService
    {
        void SendErrorReport(string content, string recipients);
        void SendInvitation(User inviter, Invited invited);
        void SendMailChimpReport(string report);

        void SendContactRequest(int userId, User item);
        void SendContactRequestAccepted(int userId, User item);
        void SendNotification(User me, User contact, Event @event);
        void SendNotification(User inviter, User invited, Group group);
        void SendNotification(User me, User contact, TimelineItem wall, Group group = null, Event @event = null, TimelineItemComment comment = null, NotificationType? notifType = null);
        void SendProposeToEat(int userId, User contact, List<User> others, Place place);

        void SendRecallMail(int invitedId);
        void SendRecallMail(Invited invited);

        void SendRecovery(User person, string recoveryLink, string message = null, string subject = null, bool isPasswordReset = true);
        ////void SendRecovery(UserModel person, string recoveryLink, string message = null, string subject = null, bool isPasswordReset = true);
        
        void SendRegistred(User me, string message = null);
        void SendRegistred(UserModel me, string message = null);
        
        void SendNewsletter(WeeklyMailSubscriber contact, DateTime start, NotificationFrequencyType frequency);
        string MakeNewsletter(WeeklyMailSubscriber contact, DateTime start, NotificationFrequencyType frequency, NewsletterEmailModel model);

        void SendPrivateMessage(Message message);
        void SendProposal(ProposalRequest proposalRequest);
        void SendCompleteProfile(User profile, User me);
        void SendWeeklyGroupNewsletter(MemberGroupNewsletter person, string subject);
        void SendAddResumeConfirmation(Resume resume);

        void SendEmailChangeRequest(User person, string email, string confirmationLink);

        /// <summary>
        /// When someone tries to register a new company, this sends an email to the recipients (typically administrators) to notify them to approve the company.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="recipients">the recipients</param>
        void SendNewCompanyDetailsForApproval(Company item, IList<Sparkle.Entities.Networks.Neutral.Person> recipients);

        /// <summary>
        /// When someone tries to register a new company, this sends an email to the recipients (typically administrators) to notify them to approve the company.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="recipients">the recipients</param>
        void SendNewCompanyDetailsForApproval(CompanyRequest item, IList<Sparkle.Entities.Networks.Neutral.Person> recipients);

        /// <summary>
        /// When someone tries to register a new company, this sends an email to the requester (company contact) to notify them the company will be approved by the team.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="requesterEmail">the recipient</param>
        void SendCompanyRequestConfirmation(CompanyRequest item, string requesterEmail);

        /// <summary>
        /// When a company registration is accepted, this sends an email to the requester (company contact) to notify them the company is approved by the team.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="requesterEmail">the recipient</param>
        /// <param name="adminEmails">The admin emails (informative).</param>
        /// <param name="otherEmails">The member emails (informative).</param>
        void SendCompanyRequestAccepted(Company item, string requesterEmail, string[] adminEmails, string[] otherEmails);

        /// <summary>
        /// When a company is created without request, this sends an email to the contact address (company contact) to notify the company is created by the team.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="requesterEmail">the recipient</param>
        /// <param name="adminEmails">The admin emails (informative).</param>
        /// <param name="otherEmails">The member emails (informative).</param>
        void SendCompanyRegisteredNotification(Company item, User inviter, string requesterEmail, string[] adminEmails, string[] otherEmails);

        /// <summary>
        /// When a company registration is accepted, this sends an email to the requester (company contact) to notify them the company is approved by the team.
        /// </summary>
        /// <param name="item">the company to approve</param>
        /// <param name="requesterEmail">the recipient</param>
        /// <param name="adminEmails">The admin emails.</param>
        /// <param name="otherEmails">The member emails.</param>
        void SendCompanyRequestRejected(CompanyRequest item, string requesterEmail, string reason);

        void SendCompanyContact(CompanyContact contact);

        void SendNewUserConfirmEmail(NewUserConfirmEmail emailModel);

        void SendPendingUserRegistrations(PendingUserRegistrationsModel model);

        void SendTechnicalMessages(IList<string> messages, NetworkAccessLevel[] networkAccessLevel);

        void SendRegisterRequest(RegisterRequestEmailModel model);

        void SendRegisterRequestConfirmation(RegisterRequestEmailModel model);

        void SendRegisterRequestDenied(RegisterRequestEmailModel model);

        void SendPrivateGroupJoinRequest(User user, Group group, int joining);

        void SendGroupJoinResponse(User user, Group group, bool success);

        InboundEmailReport HandleInboundEmail(InboundEmailModel obj);

        AdminWorksModel GetAdminWorkModel(
            bool companiesValidation = false,
            bool usersValidation = false,
            bool companiesNoAdmin = false,
            bool adsValidation = false);

        void SendAdminWorkEmail(AdminWorksModel model, bool discloseRecipients, IList<UserModel> tos);
        void SendAdminWorkEmail(AdminWorksModel model, bool discloseRecipients, params NetworkAccessLevel[] roles);

        void SendApplyRequestConfirmation(ApplyRequestModel model, UserModel requester);
        void SendApplyRequestAccepted(ApplyRequestModel model, UserModel accepted);
        void SendApplyRequestRefused(ApplyRequestModel model, UserModel refused);

        void SendPartnerResourceProposalAccepted(PartnerResources.PartnerResourceModel model, UserModel user);
        void SendPartnerResourceProposalRefused(PartnerResources.PartnerResourceModel model, UserModel user);

        void SendSubscriptionActivated(SubscriptionModel subscription, UserModel user);
        void SendSubscriptionEnded(SubscriptionModel subscription, UserModel user);

        void SendInvitationWithApply(UserModel me, UserModel invited, string invitationLink);

        void SendTests(List<string> usernames);

        /// <summary>
        /// Sends a test email to the specified email address using the specified provider configuration key.
        /// </summary>
        /// <param name="providerConfigurationKey">A configuration key that represents an email provider.</param>
        /// <param name="recipient">The email address to send the email to.</param>
        /// <returns></returns>
        EmailSendResult TestSend(string providerConfigurationKey, string recipient);
    }
}
