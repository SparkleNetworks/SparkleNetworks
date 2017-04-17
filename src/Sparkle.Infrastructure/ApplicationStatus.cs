
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Conventional values for an applications status.
    /// </summary>
    public enum ApplicationStatus : short
    {
        /// <summary>
        /// The app is suspended from any executing task.
        /// </summary>
        Disabled = -2,

        /// <summary>
        /// Frontal access is disabled for maintenance tasks.
        /// </summary>
        DownForMaintenance = -1,

        /// <summary>
        /// The app was newly created and is not visible.
        /// </summary>
        New = 0,

        /// <summary>
        /// The app should be running.
        /// </summary>
        Enabled = 1,
    }
}
