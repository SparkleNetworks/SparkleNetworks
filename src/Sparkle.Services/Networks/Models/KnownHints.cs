
namespace Sparkle.Services.Networks.Models
{
    using Sparkle.Entities.Networks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class KnownHints
    {
        public const string MaxAdSeenDateKey = "MaxAdSeenDate";

        public static HintModel MaxAdSeenDate
        {
            get
            {
                return new HintModel
                {
                    HintType = HintType.Internal,
                    Alias = MaxAdSeenDateKey,
                    IsEnabled = true,
                };
            }
        }
    }
}
