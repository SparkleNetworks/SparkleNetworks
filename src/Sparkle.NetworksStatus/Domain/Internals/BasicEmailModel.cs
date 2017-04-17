
namespace Sparkle.NetworksStatus.Domain.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public class BasicEmailModel<TModel>
    {
        private IDictionary<string, object> data;

        public BasicEmailModel()
        {
            this.data = new Dictionary<string, object>();
        }

        public BasicEmailModel(TModel model)
            : this()
        {
            this.Model = model;
        }

        public TModel Model { get; set; }

        public MailAddress Recipient { get; set; }

        public MailAddress Sender { get; set; }

        public IDictionary<string, object> Data
        {
            get { return this.data; }
        }

        public string Subject { get; set; }
    }
}
