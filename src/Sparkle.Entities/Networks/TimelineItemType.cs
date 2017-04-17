
namespace Sparkle.Entities.Networks
{
    using System;

    /// <summary>
    /// Timeline item type.
    /// Not to be confused with <see cref="TimelineType"/>.
    /// </summary>
    public enum TimelineItemType
    {
        /// <summary>
        /// A standard text publication (0).
        /// </summary>
        TextPublication = 0,

        /// <summary>
        /// A user joined the network (1).
        /// </summary>
        UserJoined = 1,

        /// <summary>
        /// A user is in contact with a user (2).
        /// </summary>
        UserInContact = 2,

        /// <summary>
        /// A poll (3).
        /// </summary>
        Poll = 3,

        /// <summary>
        /// An event (4).
        /// </summary>
        Event = 4,

        /// <summary>
        /// A obsolete type (5).
        /// </summary>
        Obsolete5 = 5,

        /// <summary>
        /// The introducing message (6).
        /// </summary>
        Introducing = 6,

        /// <summary>
        /// A company profile was updated (7).
        /// </summary>
        CompanyProfileUpdated = 7,

        /// <summary>
        /// A user joined a group (8).
        /// </summary>
        JoinedGroup = 8,

        /// <summary>
        /// A user updated its profile (12).
        /// </summary>
        PeopleProfileUpdated = 12,

        /// <summary>
        /// A obsolete type (14).
        /// </summary>
        Obsolete14 = 14,

        /// <summary>
        /// An ad (15).
        /// </summary>
        Ad = 15,

        /// <summary>
        /// A deal (16).
        /// </summary>
        Deal = 16,

        /// <summary>
        /// A company joined the network (17).
        /// </summary>
        CompanyJoined = 17,

        /// <summary>
        /// A new resource has been created
        /// </summary>
        NewPartnerResource = 18,

        /// <summary>
        /// A resource has been updated
        /// </summary>
        PartnerResourceUpdate = 19,

        /// <summary>
        /// A Twitter publication (30).
        /// </summary>
        Twitter = 30,
    }
}
