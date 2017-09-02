
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class ApplyRequest : IEntityInt32Id
    {
        public override string ToString()
        {
            return string.Format(
                "ApplyRequest {0} DC{1} {2}",
                this.Id,
                this.DateCreatedUtc,
                this.DateDeletedUtc != null ? "deleted" :
                this.DateRefusedUtc != null ? "refused" :
                this.DateAcceptedUtc != null ? "accepted" : 
                this.DateEmailConfirmedUtc != null ? "pending accept":
                this.DateSubmitedUtc != null ? "pending email confirm" :
                this.DateSubmitedUtc == null ? "not submitted" : "???");
        }
    }
}
