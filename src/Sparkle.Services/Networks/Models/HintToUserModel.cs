
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class HintToUserModel
    {
        public HintToUserModel()
        {
        }

        public HintToUserModel(int hintId, int userId)
        {
            this.HintId = hintId;
            this.UserId = userId;
        }

        public HintToUserModel(HintsToUser item)
        {
            this.HintId = item.HintId;
            this.UserId = item.UserId;
            this.DateDismissedUtc = item.DateDismissedUtc.AsUtc();
        }

        public int HintId { get; set; }

        public int UserId { get; set; }

        public DateTime? DateDismissedUtc { get; set; }
    }
}
