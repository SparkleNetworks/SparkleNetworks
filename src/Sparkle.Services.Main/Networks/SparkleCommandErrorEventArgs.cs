
namespace Sparkle.Common.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SparkleCommandErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }

        public string Universe { get; set; }
    }
}
