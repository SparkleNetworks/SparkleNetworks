using System;
using System.Collections.Generic;

namespace vCalendar
{
    /// <summary>
    /// The calendar definition
    /// </summary>
    public class Calendar
    {
        /// <summary>
        /// Gets or sets the product id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        public string ProductId{ get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the original URL.
        /// </summary>
        /// <value>
        /// The original URL.
        /// </value>
        public Uri OriginalUrl { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public CalendarScale Scale { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public CalendarMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public IList<Event> Events { get; set; }

        /// <summary>
        /// Sets the product id.
        /// </summary>
        /// <param name="company">The company.</param>
        /// <param name="product">The product.</param>
        /// <param name="version">The version.</param>
        /// <param name="languageCode">The language code.</param>
        public void SetProductId(string company, string product, string version, string languageCode)
        {
            this.ProductId = string.Format("-//{0}//NONSGML {1} {2}//{3}", company, product, version, languageCode.ToUpperInvariant());
        }
    }

    /// <summary>
    /// The calendar used
    /// </summary>
    public enum CalendarScale
    {
        /// <summary>
        /// The gregorian
        /// </summary>
        Gregorian,
    }

    /// <summary>
    /// The calendar methods.
    /// http://docstore.mik.ua/rfc/rfc2446.html#s3.2
    /// </summary>
    public enum CalendarMethod
    {
        /// <summary>
        /// Post notification of an event. Used primarily as a method of advertising the existence of an event.
        /// </summary>
        Publish,

        /// <summary>
        /// Make a request for an event. This is an explicit invitation to one or more "Attendees". Event Requests are also used to update or change an existing event. Clients that cannot handle REQUEST may degrade the event to view it as an PUBLISH.
        /// </summary>
        Request,

        /// <summary>
        /// Reply to an event request. Clients may set their status ("partstat") to ACCEPTED, DECLINED, TENTATIVE, or DELEGATED.
        /// </summary>
        Reply,

        /// <summary>
        /// Add one or more instances to an existing event.
        /// </summary>
        Add,

        /// <summary>
        /// Cancel one or more instances of an existing event.
        /// </summary>
        Cancel,

        /// <summary>
        /// A request is sent to an "Organizer" by an "Attendee" asking for the latest version of an event to be resent to the requester.
        /// </summary>
        Refresh,

        /// <summary>
        /// Counter a REQUEST with an alternative proposal, Sent by an "Attendee" to the "Organizer".
        /// </summary>
        Counter,

        /// <summary>
        /// Decline a counter proposal. Sent to an "Attendee" by the "Organizer".
        /// </summary>
        DeclineCounter,
    }
}
