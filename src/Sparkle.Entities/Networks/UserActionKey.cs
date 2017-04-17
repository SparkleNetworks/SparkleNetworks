
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class UserActionKey : IEntityInt32Id
    {
        public const string UserEmailConfirmActionKey = "UserEmailConfirmActionKey";
        public const string RecoverPasswordFromEmailActionKey = "RecoverPasswordFromEmail";

        public bool IsBlocked
        {
            get
            {
                if (this.RemainingUsages <= 0)
                    return true;

                if (this.Result != null)
                    return true;

                return false;
            }
        }

        public bool IsExpired
        {
            get
            {
                if (this.DateExpiresUtc != null && this.DateExpiresUtc.Value <= DateTime.UtcNow)
                    return true;

                return false;
            }
        }

        public override string ToString()
        {
            return "UserActionKey " + this.Id;
        }
    }
}
