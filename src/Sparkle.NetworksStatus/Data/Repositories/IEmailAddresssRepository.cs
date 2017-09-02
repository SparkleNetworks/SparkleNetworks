
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    partial interface IEmailAddresssRepository
    {
        IList<EmailAddress> GetById(int[] emailIds);

        EmailAddress Get(SrkToolkit.Common.Validation.EmailAddress emailAddress);
        EmailAddress Get(string account, string tag, string domain);
    }
}
