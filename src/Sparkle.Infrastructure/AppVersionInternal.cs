
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    using System;

    /// <summary>
    /// Contains information about the current project.
    /// </summary>
    internal static class AppVersionInternal
    {
        /// <summary>
        /// The component name.
        /// </summary>
        public const string Name = "Sparkle";

        /// <summary>
        /// The component name.
        /// </summary>
        public const string Company = "Sparkle Networks";

        /// <summary>
        /// The component name.
        /// </summary>
        public const string Copyright = "Copyright © Sparkle Networks 2011";

        public const string DefaultCulture = "fr-FR";

        /// <summary>
        /// The major version number.
        /// </summary>
        public const int Major = 1;

        /// <summary>
        /// The minor version number.
        /// </summary>
        public const int Minor = 0;

        /// <summary>
        /// The build number.
        /// </summary>
        public const int Build = 0;

        /// <summary>
        /// The source control revision number.
        /// </summary>
        public const int Revision = 140;

        /// <summary>
        /// The full version string like "1.0.0.0".
        /// </summary>
        public const string Full = "1.1.0.0";

        /// <summary>
        /// The version exposed as a <see cref="System.Version"/> object.
        /// </summary>
        public static readonly System.Version SystemVersion = new System.Version(Full);

        /// <summary>
        /// The complete name and version number.
        /// </summary>
        public const string FullName = Name + " " + Full;
    }
}
