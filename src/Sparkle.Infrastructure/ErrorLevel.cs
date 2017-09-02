
#if SSC
namespace SparkleSystems.Configuration
#else
namespace Sparkle.Infrastructure
#endif
{
    /// <summary>
    /// Error levels for loggin.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class ErrorLevel
    {
        private static readonly ErrorLevel _success = new ErrorLevel(0, "Success");
        private static readonly ErrorLevel _critical = new ErrorLevel(1, "Critical");
        private static readonly ErrorLevel _integrity = new ErrorLevel(2, "Integrity");
        private static readonly ErrorLevel _internal = new ErrorLevel(3, "Internal");
        private static readonly ErrorLevel _data = new ErrorLevel(4, "Data");
        private static readonly ErrorLevel _authn = new ErrorLevel(5, "Authn");
        private static readonly ErrorLevel _authz = new ErrorLevel(6, "Authz");
        private static readonly ErrorLevel _business = new ErrorLevel(7, "Business");
        private static readonly ErrorLevel _input = new ErrorLevel(8, "Input");
        private static readonly ErrorLevel _thirdParty = new ErrorLevel(9, "ThirdPty");

        private short errorLevel;
        private string name;

        public ErrorLevel(short errorLevel, string name)
        {
            this.errorLevel = errorLevel;
            this.name = name;
        }

        /// <summary>
        /// Successfull operation.
        /// </summary>
        public static ErrorLevel Success { get { return _success; } }

        /// <summary>
        /// An error causing system instability.
        /// </summary>
        public static ErrorLevel Critical { get { return _critical; } }

        /// <summary>
        /// Data integrity error.
        /// </summary>
        public static ErrorLevel Integrity { get { return _integrity; } }

        /// <summary>
        /// Internal error.
        /// </summary>
        public static ErrorLevel Internal { get { return _internal; } }

        /// <summary>
        /// Data access error.
        /// </summary>
        public static ErrorLevel Data { get { return _data; } }

        /// <summary>
        /// Authentication error.
        /// </summary>
        public static ErrorLevel Authn { get { return _authn; } }

        /// <summary>
        /// Authorization error.
        /// </summary>
        public static ErrorLevel Authz { get { return _authz; } }

        /// <summary>
        /// Business rule error.
        /// </summary>
        public static ErrorLevel Business { get { return _business; } }

        /// <summary>
        /// User input error.
        /// </summary>
        public static ErrorLevel Input { get { return _input; } }

        /// <summary>
        /// Third-party component error.
        /// </summary>
        public static ErrorLevel ThirdParty { get { return _thirdParty; } }

        /// <summary>
        /// The current error level value.
        /// </summary>
        public short Value { get { return this.errorLevel; } }

        public string Name { get { return this.name; } }
    }
}
