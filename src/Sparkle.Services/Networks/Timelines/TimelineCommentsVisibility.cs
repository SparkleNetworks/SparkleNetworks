
namespace Sparkle.Services.Networks.Timelines
{
    using System;

    public enum TimelineCommentsVisibility
    {
        /// <summary>
        /// Comments are not visible.
        /// </summary>
        None,

        /// <summary>
        /// Comment are hidden. The user can expand the list.
        /// </summary>
        Hidden,

        /// <summary>
        /// Few comments are displayed. The user can expand the list.
        /// </summary>
        Few,

        /// <summary>
        /// All comments are displayed.
        /// </summary>
        All,
    }
}
