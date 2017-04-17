
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Networks.Definitions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PictureAccess
    {
        public const string CacheDateFormat = "yyyyMMddTHHmmssZ";

        public PictureFormat Format { get; set; }

        public string FilePath { get; set; }

        public byte[] Bytes { get; set; }

        public string Remark { get; set; }

        public string MimeType { get; set; }

        public DateTime DateChangedUtc { get; set; }
    }
}
