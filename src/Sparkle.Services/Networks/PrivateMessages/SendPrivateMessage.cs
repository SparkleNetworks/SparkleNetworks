
namespace Sparkle.Services.Networks.PrivateMessages
{
    using Sparkle.Services.Networks.Models;
    using SrkToolkit.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Namespace = Names.PublicNamespace)]
    public class SendPrivateMessageRequest : BaseRequest
    {
        public SendPrivateMessageRequest()
        {
        }

        [IgnoreDataMember]
        public int ActingUserId { get; set; }

        [DataMember]
        public int TargetUserId { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public MessageSource Source { get; set; }
    }

    [DataContract(Namespace = Names.PublicNamespace)]
    public class SendPrivateMessageResult : BaseResult<SendPrivateMessageRequest, SendPrivateMessageError>
    {
        public SendPrivateMessageResult(SendPrivateMessageRequest request)
            : base(request)
        {
        }

        [DataMember]
        public bool EmailStatus { get; set; }

        [DataMember]
        public ConversationMessageModel Item { get; set; }
    }

    public enum SendPrivateMessageError
    {
        NoSuchActingUser,
        NotAuthorized,
        NoSuchTargetUser,
        TargetUserIsInactive,
        TargetUserIsNotSubscribed,
        EmptyMessage,
        MessageIsTooLong,
    }
}
