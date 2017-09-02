
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Filters;
    using Sparkle.Data.Networks;
    using Sparkle.Data.Networks.Objects;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Lang;
    using Sparkle.Services.Networks.Models;
    using Sparkle.Services.Networks.Models.Profile;
    using Sparkle.Services.Networks.PrivateMessages;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class PrivateMessageService : ServiceBase, IPrivateMessageService
    {
        private static readonly NetworkAccessLevel[] sendMessageWithoutSubscriptionRoles = new NetworkAccessLevel[] { NetworkAccessLevel.SparkleStaff, NetworkAccessLevel.NetworkAdmin, };

        internal PrivateMessageService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }

        protected IPrivateMessageRepository PrivateMessageRepository
        {
            get { return this.Repo.PrivateMessage; }
        }

        /// <summary>
        /// Selects the conversations from user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Message> SelectConversationsFromUserId(int userId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .Conversations(userId)
                .ToList();
        }

        /// <summary>
        /// Selects from user id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Message> SelectFromUserId(int userId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .WithId(userId)
                .ToList();
        }

        /// <summary>
        /// Selects the conversation.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="contactId">The contact id.</param>
        /// <returns></returns>
        public IList<Message> SelectConversation(int userId, int contactId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .Between(userId, contactId)
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        /// <summary>
        /// Selects the sended and received messags.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public IList<Message> SelectSendedAndReceivedMessags(int userId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .FromOrTo(userId)
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }

        /// <summary>
        /// Selects the last received message.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public Message SelectLastReceivedMessage(int userId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .To(userId)
                .OrderByDescending(o => o.CreateDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public long Insert(Message item)
        {
            PrivateMessageRepository.Insert(item);
            return 1;
        }

        public Message Update(Message item)
        {
            return this.PrivateMessageRepository.Update(item);
        }

        public IList<Message> GetUnread(int userId)
        {
            return this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .To(userId)
                .Unread()
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }

        public IList<Message> GetUnreadAndMarkAsRead(int userId)
        {
            var result = this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .To(userId)
                .Unread()
                .OrderByDescending(o => o.CreateDate)
                .ToList();

            foreach (var message in result)
            {
                message.Displayed = true;
                this.Services.PrivateMessage.Update(message);
            }

            return result;
        }
        
        public IList<Message> GetFromId(int userId, int messageId)
        {
            var result = this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .To(userId)
                .FromId(messageId)
                .OrderByDescending(o => o.CreateDate)
                .ToList();

            return result;
        }

        public int Count()
        {
            return this.PrivateMessageRepository
                .Select()
                .Where(o => o.From != null && o.From.NetworkId == this.Services.NetworkId)
                .Count();
        }

        public int CountToday()
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            var to = from.AddDays(1D);
            return this.PrivateMessageRepository
                .Select()
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public int CountLast24Hours()
        {
            var to = DateTime.UtcNow;
            var from = to.AddDays(-1D);
            return this.PrivateMessageRepository
                .Select()
                .Where(i => from <= i.CreateDate && i.CreateDate <= to)
                .Count();
        }

        public void MarkAsReadByUserId(int fromUserId, int toUserId)
        {
            var messages = this.PrivateMessageRepository
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .To(toUserId)
                .Unread()
                .From(fromUserId)
                .ToList();

            foreach (var message in messages)
            {
                message.Displayed = true;
                this.Services.PrivateMessage.Update(message);
            }
        }

        public int CountUnread(int userId)
        {
            return this.PrivateMessageRepository
                .Select()
                .To(userId)
                .Unread()
                .Count();
        }

        public IList<ConversationModel> GetConversationsFor(int userId)
        {
            var list = this.PrivateMessageRepository.GetUsersConversations(userId, this.Services.NetworkId);
            var me = this.Services.People.GetLiteByIdFromAnyNetwork(new int[] { userId, })[0];
            var users = this.Services.People.GetLiteByIdFromAnyNetwork(list.Select(r => r.OtherUserId).ToArray());
            var results = new List<ConversationModel>();
            foreach (var item in list)
            {
                var user = users.Single(u => u.Id == item.OtherUserId);
                var result = new ConversationModel(item, me, user);
                results.Add(result);
            }

            return results;
        }

        public ConversationModel GetConversation(int myUserId, int otherUserId, bool markMessagesRead)
        {
            var users = this.Services.People.GetLiteByIdFromAnyNetwork(new int[] { myUserId, otherUserId, });
            var myUser = users.Single(u => u.Id == myUserId);
            var otherUser = users.Single(u => u.Id == otherUserId);

            var result = new ConversationModel(myUser, otherUser);
            var messages = this.PrivateMessageRepository.GetMessages(myUserId, otherUserId);
            result.Messages = messages
                .Select(m => new ConversationMessageModel(m))
                .ToList();

            if (markMessagesRead && messages.Count > 0)
            {
                var latest = messages.OrderBy(m => m.CreateDate).Last();
                this.PrivateMessageRepository.MarkReadUntil(myUserId, otherUserId, latest.Id);
            }

            return result;
        }
        /*
        [Obsolete("Use the overload")]
        public void Send(Message message)
        {
            // insert message
            this.Services.PrivateMessage.Insert(message);

            // Envoi de l'email
            var notificationChoice = this.Services.Notifications.SelectNotifications(message.ToUserId);
            
            if (notificationChoice.PrivateMessage && this.Services.Subscriptions.IsUserSubscribed(message.ToUserId))
            {
                this.Services.Email.SendPrivateMessage(message);
            }
        }
        */
        public SendPrivateMessageResult Send(SendPrivateMessageRequest request)
        {
            const string methodName = "Send";
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new SendPrivateMessageResult(request);

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                result.Errors.Add(SendPrivateMessageError.EmptyMessage, NetworksEnumMessages.ResourceManager);
            }

            if (request.Message.Length > 1200)
            {
                result.Errors.Add(SendPrivateMessageError.MessageIsTooLong, NetworksEnumMessages.ResourceManager);
            }

            var sendingUser = this.Services.People.GetById(request.ActingUserId, Data.Options.PersonOptions.Company);
            var targetUser = this.Services.People.GetById(request.TargetUserId, Data.Options.PersonOptions.Company);
            bool subscriptionsEnabled = this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnabled;
            bool maySendEmail = true;

            if (sendingUser != null)
            {
                if (!sendingUser.IsActive)
                {
                    result.Errors.Add(SendPrivateMessageError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                }

                if (subscriptionsEnabled)
                {
                    if ((!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(sendingUser.Id)) || sendingUser.NetworkAccessLevel.Value.HasAnyFlag(sendMessageWithoutSubscriptionRoles))
                    {
                        // user is allowed to send
                    }
                    else
                    {
                        result.Errors.Add(SendPrivateMessageError.NotAuthorized, NetworksEnumMessages.ResourceManager);
                    }
                }
            }
            else
            {
                result.Errors.Add(SendPrivateMessageError.NoSuchActingUser, NetworksEnumMessages.ResourceManager);
            }

            if (targetUser != null)
            {
                if (!targetUser.IsActive)
                {
                    result.Errors.Add(SendPrivateMessageError.TargetUserIsInactive, NetworksEnumMessages.ResourceManager);
                }

                if (subscriptionsEnabled)
                {
                    if (!this.Services.AppConfiguration.Tree.Features.Subscriptions.IsEnforced || this.Services.Subscriptions.IsUserSubscribed(targetUser.Id))
                    {
                        // user is allowed to receive
                    }
                    else
                    {
                        // user is allowed to receive but shall not receive emails
                        maySendEmail = false;
                    }
                }
            }
            else
            {
                result.Errors.Add(SendPrivateMessageError.NoSuchTargetUser, NetworksEnumMessages.ResourceManager);
            }

            if (result.Errors.Count > 0)
            {
                return this.LogResult(result, methodName, "From:" + request.ActingUserId + " To:" + request.TargetUserId);
            }

            // insert message
            var message = new Message();
            message.FromUserId = sendingUser.Id;
            message.ToUserId = targetUser.Id;
            message.Text = request.Message;
            message.Source = (byte)request.Source;
            message.CreateDate = DateTime.UtcNow;
            this.Repo.PrivateMessage.Insert(message);

            result.Succeed = true;
            result.Item = new ConversationMessageModel(message);

            // send email
            var notificationChoice = this.Services.Notifications.SelectNotifications(message.ToUserId);
            if (notificationChoice.PrivateMessage && maySendEmail)
            {
                result.EmailStatus = false;
                try
                {
                    this.Services.Email.SendPrivateMessage(message);
                    result.EmailStatus = true;
                }
                catch (SparkleServicesException ex)
                {
                    Trace.WriteLine("PrivateMessageService.Send: failed to send email: " + ex.ToSummarizedString());
                }
            }

            return this.LogResult(result, methodName, "Message:" + message.Id);
        }

        public Message GetPrivateMessageById(int timelineItemId)
        {
            return this.Repo.PrivateMessage
                .CreateQuery(MessageOptions.From | MessageOptions.To)
                .WithMessageId(timelineItemId)
                .SingleOrDefault();
        }
    }
}
