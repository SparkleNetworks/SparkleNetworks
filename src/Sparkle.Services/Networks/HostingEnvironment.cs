
namespace Sparkle.Services.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HostingEnvironment
    {
        private ServiceIdentity serviceIdentity;
        private string remoteClient;

        public HostingEnvironment()
        {
        }
        
        public Action<IServiceFactory, Action<IServiceFactory>> ThreadPoolDelegate { get; set; }

        public ContextParallelismMode ParallelismMode { get; set; }

        public ServiceIdentity Identity
        {
            get { return this.serviceIdentity ?? (this.serviceIdentity = ServiceIdentity.Anonymous); }
            set { this.serviceIdentity = value; }
        }

        public string LogBasePath { get; set; }

        public string LogPath { get; set; }

        public string RemoteClient
        {
            get { return this.remoteClient ?? "-remote-client-not-set-"; }
            set { this.remoteClient = value; }
        }
    }
}
