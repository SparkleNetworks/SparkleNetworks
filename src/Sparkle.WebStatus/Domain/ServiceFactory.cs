
namespace Sparkle.WebStatus.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ServiceFactory
    {
        private readonly ServiceConfiguration configuration;
        private NetworksService networks;
        private DomainNameRecordsService domainNameRecords;
        private ServiceContext context;
        private EmailService email;
        private BasicUsersService basicUsers;
        private ApiKeysService apiKeys;

        public ServiceFactory(ServiceConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public NetworksService Networks
        {
            get { return this.networks ?? (this.networks = new NetworksService(this.configuration)); }
        }

        public BasicUsersService BasicUsers
        {
            get { return this.basicUsers ?? (this.basicUsers = new BasicUsersService(this.configuration)); }
        }

        public ApiKeysService ApiKeys
        {
            get { return this.apiKeys ?? (this.apiKeys = new ApiKeysService(this.configuration)); }
        }

        public DomainNameRecordsService DomainNameRecords
        {
            get { return this.domainNameRecords ?? (this.domainNameRecords = new DomainNameRecordsService(this.configuration)); }
        }

        public ServiceContext Context
        {
            get { return this.context ?? (this.context = new ServiceContext()); }
        }

        public ServiceConfiguration Configuration
        {
            get { return this.configuration; }
        }

        public EmailService Email
        {
            get { return this.email ?? (this.email = new EmailService(this)); }
        }

        internal void Verify()
        {
            var errors = new List<Tuple<string, Exception>>();

            if (string.IsNullOrEmpty(this.Configuration.SmtpConfiguration))
            {
                errors.Add(new Tuple<string, Exception>("WebStatusServices does not have a SMTP configuration", null));
            }

            try
            {
                this.Networks.GetAll();
            }
            catch (Exception ex)
            {
                errors.Add(new Tuple<string, Exception>("WebStatusServices fails to access local database", ex));
            }

            if (errors.Count > 0)
            {
                var agg = new InvalidOperationException("WebStatusServices verification failed with " + errors.Count + " errors" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => e.Item1 + (e.Item2 != null ? (": " + e.Item2.Message) : string.Empty))));
                agg.Data["Errors"] = errors;
                throw agg;
            }
        }
    }
}