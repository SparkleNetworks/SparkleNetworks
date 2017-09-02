
namespace Sparkle.Services.Main.Networks
{
    using Sparkle.Data.Networks;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks;
    using SrkToolkit.Common.Validation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public class InboundEmailMessagesService : ServiceBase, IInboundEmailMessagesService
    {
        [DebuggerStepThrough]
        internal InboundEmailMessagesService(IRepositoryFactory repositoryFactory, IServiceFactory serviceFactory)
            : base(repositoryFactory, serviceFactory)
        {
        }
        
        public int AddMandrillEmailLog(Services.Networks.Models.InboundEmailMessage inboundEmailMessage, string nameInvariant, DateTime utcNow, bool succeed)
        {
            var sender = new EmailAddress(inboundEmailMessage.FromEmail);
            var receiver = new EmailAddress(inboundEmailMessage.Email);

            return this.Repo.InboundEmailMessage.Insert(new InboundEmailMessage
                {
                    NetworkId = this.Services.NetworkId,
                    SenderEmailAccount = sender.AccountPart,
                    SenderEmailTag = sender.TagPart,
                    SenderEmailDomain = sender.DomainPart,
                    ReceiverEmailAccount = receiver.AccountPart,
                    ReceiverEmailTag = receiver.TagPart,
                    ReceiverEmailDomain = receiver.DomainPart,
                    Provider = (int)InboundEmailProvider.Mandrill,
                    DateReceivedUtc = utcNow,
                    SourceEmailFileName = nameInvariant,
                    ProviderSpamScore = inboundEmailMessage != null ? (inboundEmailMessage.SpamReport != null ? new float?(inboundEmailMessage.SpamReport.Score) : null) : null,
                    DkimSigned = inboundEmailMessage != null ? (inboundEmailMessage.Dkim != null ? new bool?(inboundEmailMessage.Dkim.Signed) : null) : null,
                    DkimValid = inboundEmailMessage != null ? (inboundEmailMessage.Dkim != null ? new bool?(inboundEmailMessage.Dkim.Valid) : null) : null,
                    SpfTestResult = inboundEmailMessage != null ? (inboundEmailMessage.Spf != null ? (!string.IsNullOrEmpty(inboundEmailMessage.Spf.Result) ? inboundEmailMessage.Spf.Result : null) : null) : null,
                    SpfTestDetail = inboundEmailMessage != null ? (inboundEmailMessage.Spf != null ? (!string.IsNullOrEmpty(inboundEmailMessage.Spf.Detail) ? inboundEmailMessage.Spf.Detail : null) : null) : null,
                    Success = succeed,
                }).Id;
        }

        public void LinkMandrillLogToWallItem(int logId, int itemId, bool newItem, Sparkle.Services.Networks.Models.ItemCaptureResult type)
        {
            if (type == Sparkle.Services.Networks.Models.ItemCaptureResult.TimelineItem)
            {
                if (newItem)
                {
                    var wallItem = this.Services.Wall.SelectByPublicationId(itemId);
                    wallItem.InboundEmailId = logId;
                    wallItem = this.Repo.Wall.Update(wallItem);
                }
                else
                {
                    var commentItem = this.Services.WallComments.SelectById(itemId);
                    commentItem.InboundEmailId = logId;
                    this.Services.WallComments.Update(commentItem);
                }
            }
            else if (type == Sparkle.Services.Networks.Models.ItemCaptureResult.PrivateMessage)
            {
                var privateMessage = this.Services.PrivateMessage.GetPrivateMessageById(itemId);
                privateMessage.InboundEmailId = logId;
                this.Services.PrivateMessage.Update(privateMessage);
            }
        }

    }
}
