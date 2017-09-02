
namespace Sparkle.Services.Main.Internal
{
    using Sparkle.Services.Main.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains a verification rule for the <see cref="MainServiceFactory"/>.
    /// </summary>
    internal class ServiceFactoryAssertion
    {
        private readonly string name;
        private readonly Func<MainServiceFactory, bool> action;
        private readonly string errorMessage;

        public ServiceFactoryAssertion(string name, Func<MainServiceFactory, bool> action, string errorMessage)
        {
            this.name = name;
            this.action = action;
            this.errorMessage = errorMessage;
        }

        public ServiceFactoryAssertion(string name, Func<MainServiceFactory, bool> action)
            : this(name, action, null)
        {
        }

        public ServiceFactoryAssertion(string name, Action<MainServiceFactory> action, string errorMessage)
        {
            this.name = name;
            this.action = new Func<MainServiceFactory, bool>(s => { action(s); return true; });
            this.errorMessage = errorMessage;
        }

        public ServiceFactoryAssertion(string name, Action<MainServiceFactory> action)
            : this(name, action, null)
        {
        }

        /// <summary>Execute the verification</summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal void Verify(MainServiceFactory context)
        {
            if (!this.action(context))
            {
                if (this.errorMessage != null)
                    throw new InvalidOperationException("Test '" + this.name + "' failed: '" + this.errorMessage + "'");
                else
                    throw new InvalidOperationException("Test '" + this.name + "' failed");
            }
        }
    }
}
