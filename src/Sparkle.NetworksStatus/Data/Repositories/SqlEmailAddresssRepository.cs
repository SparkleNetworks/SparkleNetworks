
namespace Sparkle.NetworksStatus.Data.Repositories
{
    using PetaPoco;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class SqlEmailAddresssRepository : IEmailAddresssRepository
    {
        public IList<EmailAddress> GetById(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return new List<EmailAddress>();

            var sql = Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("Id IN(@ids)", new { ids = ids, });
            return this.PetaContext.Fetch<EmailAddress>(sql);
        }

        public EmailAddress Get(SrkToolkit.Common.Validation.EmailAddress emailAddress)
        {
            return this.Get(emailAddress.AccountPart, emailAddress.TagPart, emailAddress.DomainPart);
        }

        public EmailAddress Get(string account, string tag, string domain)
        {
            var sql = Sql.Builder
                .Select("*")
                .From(TableName)
                .Where("AccountPart = @0 AND TagPart = @1 AND DomainPart = @2", account, tag, domain);
            return this.PetaContext.SingleOrDefault<EmailAddress>(sql);
        }
    }
}
