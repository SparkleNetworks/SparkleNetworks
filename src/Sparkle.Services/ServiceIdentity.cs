
namespace Sparkle.Services
{
    using System;
    using Sparkle.Entities;
    using Sparkle.Entities.Networks;

    /// <summary>
    /// Object used by the business layer to apply rules and enforce access control.
    /// </summary>
    public sealed class ServiceIdentity
    {

        // don't even dare to touch this
        private ServiceIdentity()
        {
        }

        #region Creators

        /// <summary>
        /// Gets the anonymous identity.
        /// </summary>
        public static ServiceIdentity Anonymous
        {
            get
            {
                return new ServiceIdentity
                {
                    IsAnonymous = true
                };
            }
        }

        /// <summary>
        /// Gets a User identity for the specified person.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <returns></returns>
        public static ServiceIdentity User(User person)
        {
            if (person == null)
                throw new ArgumentNullException("person");
            return new ServiceIdentity
            {
                IsUser = true,
                Person = person,
            };
        }
        
        /// <summary>
        /// Gets a User identity for the specified username.
        /// </summary>
        /// <param name="login">The username.</param>
        /// <returns></returns>
        public static ServiceIdentity User(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("The value cannot be empty", "username");
            return new ServiceIdentity
            {
                IsUser = true,
                Person = new Sparkle.Entities.Networks.Neutral.Person
                {
                    Username = username,
                },
            };
        }

        /// <summary>
        /// Gets a User identity for the specified username.
        /// </summary>
        /// <param name="login">The username.</param>
        /// <returns></returns>
        public static ServiceIdentity UserWithEmailAddress(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentException("The value cannot be empty", "emailAddress");
            return new ServiceIdentity
            {
                IsUser = true,
                Person = new Sparkle.Entities.Networks.Neutral.Person
                {
                    Email = emailAddress,
                },
            };
        }

        /// <summary>
        /// Gets a User identity for a GetMeLunch user.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static ServiceIdentity GmlUser(int id, string email)
        {
            return new ServiceIdentity
            {
                IsAnonymous = true,
                GmlId = id + "/" + email,
            };
        }

        #endregion

        /// <summary>
        /// An anonymous user responds to all business rules and has a very limited set of features.
        /// </summary>
        public bool IsAnonymous { get; private set; }

        /// <summary>
        /// A user responds to all business rules.
        /// </summary>
        public bool IsUser { get; private set; }

        /// <summary>
        /// An administrator can bypass a few business rules like quotas but cannot break data integrity.
        /// </summary>
        public bool IsAdministrator { get; private set; }

        /// <summary>
        /// Gets the associated person if this identity is for a person.
        /// </summary>
        public IPerson Person { get; private set; }

        /// <summary>
        /// Can break business rules.
        /// Can break data integrity.
        /// Big bad boss identity.
        /// </summary>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets or sets the GetMeLunch id.
        /// </summary>
        /// <value>
        /// The GML id.
        /// </value>
        public string GmlId { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            this.Check();

            if (this.GmlId != null)
            {
                return "GML:" + this.GmlId;
            }

            if (this.IsAnonymous)
                return Infrastructure.SysLogger.AnonymousIdentity;
            ////if (this.IsAdministrator)
            ////    return "admin:" + this.Person.Login;
            if (this.IsUser)
                return "user:" + this.Person.Username;
            if (this.IsRoot)
                return Infrastructure.SysLogger.RootIdentity;

            throw new InvalidOperationException("Identity object is incoherent");
        }

        public ServiceIdentity Clone()
        {
            return new ServiceIdentity
            {
                GmlId = this.GmlId,
                IsAdministrator = this.IsAdministrator,
                IsAnonymous = this.IsAnonymous,
                IsRoot = this.IsRoot,
                IsUser = this.IsUser,
                Person = this.Person != null ? new Sparkle.Entities.Networks.Neutral.Person
                {
                    Email = this.Person.Email,
                    UserId = this.Person.UserId,
                    Username = this.Person.Username,
                } : null,
            };
        }

        private void Check()
        {
            if (this.IsUser && this.Person == null)
                throw new InvalidOperationException("Identity object is incoherent");
        }

        /// <summary>
        /// Provides access to dangerous identities.
        /// </summary>
        public static class Unsafe
        {
            /// <summary>
            /// Returns the super master über god user.
            /// Use with care.
            /// </summary>
            public static ServiceIdentity Root
            {
                get { return new ServiceIdentity { IsRoot = true }; }
            }
        }
    }
}
