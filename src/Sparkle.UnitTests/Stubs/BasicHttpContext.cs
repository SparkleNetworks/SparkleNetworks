﻿
namespace Sparkle.UnitTests.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Web.SessionState;

    public class BasicHttpContext : HttpContextBase
    {
        private HttpSessionStateBase session;
        private IPrincipal user;
        private Dictionary<object, object> items = new Dictionary<object, object>();

        public BasicHttpContext(
            HttpSessionStateBase session = null,
            IPrincipal user = null)
        {
            this.session = session;
            this.user = user;
        }

        public override HttpSessionStateBase Session
        {
            get
            {
                return this.session ?? (this.session = new BasicHttpSessionState());
            }
        }

        public override IPrincipal User
        {
            get
            {
                return this.user;
            }
            set
            {
                this.user = value;
            }
        }

        public override System.Collections.IDictionary Items
        {
            get
            {
                return this.items;
            }
        }
    }
}
