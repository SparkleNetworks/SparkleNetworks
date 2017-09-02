
namespace Sparkle.EmailUtility.Splitters
{
    using Sparkle.EmailUtility.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ISplitter
    {
        int Priority { get; }

        bool IsMatch(MessageBody request);
        MessageBodyParts Process(MessageBody request);
    }
}
