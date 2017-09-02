
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    /// <summary>
    /// Contains information about the current project.
    /// </summary>
    public static class AppVersion
    {
        /// <summary>
        /// The component name.
        /// </summary>
        public const string Name = AppVersionInternal.Name;

        /// <summary>
        /// The component name.
        /// </summary>
        public const string Company = AppVersionInternal.Company;

        /// <summary>
        /// The component name.
        /// </summary>
        public const string Copyright = AppVersionInternal.Copyright;

        /// <summary>
        /// The component name.
        /// </summary>
        public const string DefaultCulture = AppVersionInternal.DefaultCulture;

        /// <summary>
        /// The major version number.
        /// </summary>
        public const int Major = AppVersionInternal.Major;

        /// <summary>
        /// The minor version number.
        /// </summary>
        public const int Minor = AppVersionInternal.Minor;

        /// <summary>
        /// The build number.
        /// </summary>
        public const int Build = AppVersionInternal.Build;

        /// <summary>
        /// The source control revision number.
        /// </summary>
        public const int Revision = AppVersionInternal.Revision;

        /// <summary>
        /// The full version string like "1.0.0.0".
        /// </summary>
        public const string Full = AppVersionInternal.Full;

        /// <summary>
        /// The version exposed as a <see cref="System.Version"/> object.
        /// </summary>
        public static readonly System.Version SystemVersion = new System.Version(Full);

        /// <summary>
        /// The complete name and version number.
        /// </summary>
        public static readonly string FullName = AppVersionInternal.FullName;
    }
}
