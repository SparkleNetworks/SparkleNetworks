
namespace Sparkle.Services.Networks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.Entities.Networks;

    public class AdminWorksModel
    {
        private IList<AdminWorkModel> items = new List<AdminWorkModel>();
        private IList<AdminWorkRecipient> recipients;
        
        public IList<AdminWorkModel> Items
        {
            get { return this.items; }
            set { this.items = value; }
        }

        public NetworkAccessLevel[] NetworkAccessLevels { get; set; }

        public IList<AdminWorkRecipient> Recipients
        {
            get { return this.recipients; }
            set { this.recipients = value; }
        }

        public AdminWorkRecipient SelfRecipient { get; set; }

        public bool DiscloseRecipients { get; set; }

        public AdminWorksModel For(AdminWorkRecipient recipient)
        {
            return new AdminWorksModel
            {
                Items = this.Items,
                NetworkAccessLevels = this.NetworkAccessLevels,
                SelfRecipient = recipient,
                Recipients = this.DiscloseRecipients
                    ? this.Recipients
                        .Where(r => r.User != null && !(r.NetworkAccessLevels != null && r.NetworkAccessLevels.Count == 1 && r.NetworkAccessLevels.Single() == NetworkAccessLevel.SparkleStaff))
                        .OrderBy(r => r.User.DisplayName)
                        .ToList()
                    : null,
                DiscloseRecipients = this.DiscloseRecipients,
            };
        }
    }
}
