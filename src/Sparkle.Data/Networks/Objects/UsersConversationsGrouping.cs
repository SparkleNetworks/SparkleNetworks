
namespace Sparkle.Data.Networks.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class UsersConversationsGrouping
    {
        public int MyUserId { get; set; }

        public int OtherUserId { get; set; }

        public int? SentDisplayedCount { get; set; }
        public DateTime? SentDisplayedLastDate { get; set; }
        public int? SentDisplayedLastId { get; set; }

        public int? SentUndisplayedCount { get; set; }
        public DateTime? SentUndisplayedLastDate { get; set; }
        public int? SentUndisplayedLastId { get; set; }

        public int? ReceivedDisplayedCount { get; set; }
        public DateTime? ReceivedDisplayedLastDate { get; set; }
        public int? ReceivedDisplayedLastId { get; set; }

        public int? ReceivedUndisplayedCount { get; set; }
        public DateTime? ReceivedUndisplayedLastDate { get; set; }
        public int? ReceivedUndisplayedLastId { get; set; }
    }
}
