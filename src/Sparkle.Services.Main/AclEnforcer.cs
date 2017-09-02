
namespace Sparkle.Services.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AclEnforcer
    {
        private readonly ServiceIdentity identity;
        private bool allowAnonymous;
        private bool allowUsers;
        private bool allowAdministrators;
        private bool denyAnonymous;
        private bool denyUsers;
        private bool denyAdministrators;

        public AclEnforcer(ServiceIdentity identity)
        {
            this.identity = identity;
        }

        public AclEnforcer AllowAnonymous()
        {
            if (this.denyAnonymous)
                throw new InvalidOperationException();
            this.allowAnonymous = true;
            return this;
        }

        public AclEnforcer AllowUsers()
        {
            if (this.denyUsers)
                throw new InvalidOperationException();
            this.allowUsers = true;
            return this;
        }

        public AclEnforcer AllowAdministrators()
        {
            if (this.denyAdministrators)
                throw new InvalidOperationException();
            this.allowAdministrators = true;
            return this;
        }

        public AclEnforcer DenyAnonymous()
        {
            if (this.allowAnonymous)
                throw new InvalidOperationException();
            this.denyAnonymous = true;
            return this;
        }

        public AclEnforcer DenyUsers()
        {
            if (this.allowUsers)
                throw new InvalidOperationException();
            this.denyUsers = true;
            return this;
        }

        public AclEnforcer DenyAdministrators()
        {
            if (this.allowAdministrators)
                throw new InvalidOperationException();
            this.denyAdministrators = true;
            return this;
        }

        public bool Result
        {
            get { return this.Compute(); }
        }

        public bool Compute()
        {
            if (this.identity.IsAnonymous)
            {
                if (this.denyAnonymous || !this.allowAnonymous)
                    return false;
                else
                    return true;
            }

            if (this.identity.IsUser)
            {
                if (this.denyUsers || !this.allowUsers)
                    return false;
                else
                    return true;
            }

            if (this.identity.IsAdministrator)
            {
                if (this.denyAdministrators || !this.allowAdministrators)
                    return false;
                else
                    return true;
            }

            if (this.identity.IsRoot)
            {
                return true;
            }

            return false;
        }

        /// <exception cref="ForbiddenException">not allowed</exception>
        public void Check()
        {
            if (!this.Compute())
                throw new ForbiddenException();
        }
    }
}
