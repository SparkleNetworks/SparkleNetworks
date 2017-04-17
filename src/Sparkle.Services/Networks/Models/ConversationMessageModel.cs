
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ConversationMessageModel
    {
        public ConversationMessageModel(Entities.Networks.Message item)
        {
            this.FromUserId = item.FromUserId;
            this.ToUserId = item.ToUserId;
            this.Id = item.Id;
            this.Archived = item.Archived;
            this.CreateDate = item.CreateDate;
            this.Displayed = item.Displayed;
            this.Subject = item.Subject;
            this.Text = item.Text;
            this.SourceId = item.Source;
        }

        public ConversationMessageModel()
        {
        }

        public int Id { get; set; }

        public int ToUserId { get; set; }

        public int FromUserId { get; set; }

        public bool? Archived { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Displayed { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public byte SourceId { get; set; }

        public string Html { get; set; }

        public MessageSource Source
        {
            get { return (MessageSource)this.SourceId; }
            set { this.SourceId = (byte)value; }
        }
    }

    /// <summary>
    /// The source of a message.
    /// DO NOT ALTER the values.
    /// </summary>
    public enum MessageSource : byte
    {
        Unknown = 0,

        /// <summary>
        /// The user used the main messaging UI.
        /// </summary>
        SiteConversation = 1,

        /// <summary>
        /// The user used the sidebar messaging UI.
        /// </summary>
        SiteChat = 2,

        /// <summary>
        /// The message has been imported.
        /// </summary>
        ExternalImport = 3,

        /// <summary>
        /// Special message that occur when a group is deleted.
        /// </summary>
        GroupClosure = 4,

        /// <summary>
        /// The message has been received via a system inbound email.
        /// </summary>
        InboundEmail = 5,

        /// <summary>
        /// The user used another UI in the main website.
        /// </summary>
        Site = 6,
    }
}
