using System;

namespace vCalendar
{
    /// <summary>
    /// A calendar event
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets the date created UTC.
        /// </summary>
        public DateTime DateCreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the date updated UTC.
        /// </summary>
        public DateTime DateUpdatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the date start UTC.
        /// </summary>
        public DateTime DateStartUtc { get; set; }

        /// <summary>
        /// Gets or sets the date end UTC.
        /// </summary>
        public DateTime DateEndUtc { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        public EventClass Class { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EventStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the party status.
        /// </summary>
        public PartyStatus PartyStatus { get; set; }
    }

    /// <summary>
    /// This property defines the access classification for a calendar component.
    /// </summary>
    public enum EventClass
    {
        /// <summary>
        /// public
        /// </summary>
        Public,

        /// <summary>
        /// private
        /// </summary>
        Private,

        /// <summary>
        /// confidential
        /// </summary>
        Confidential,
    }

    /// <summary>
    /// The event status
    /// </summary>
    public enum EventStatus
    {
        /// <summary>
        /// Indicates the event is tentative
        /// </summary>
        Tentative,

        /// <summary>
        /// Indicates the event is confirmed
        /// </summary>
        Confirmed,

        /// <summary>
        /// Indicates the event was cancelled
        /// </summary>
        Cancelled,
    }

    /// <summary>
    /// The event party status
    /// </summary>
    public enum PartyStatus
    {
        NeedsAction,
        Accepted,
        Tentative,
        Declined,
        Delegated,

    }
}
