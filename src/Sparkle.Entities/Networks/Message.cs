
namespace Sparkle.Entities.Networks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    partial class Message
    {
    }

    public enum MessageOptions
    {
        None = 0,
        From = 0x0001,
        To   = 0x0002,
    }
}
