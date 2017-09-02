
namespace Sparkle.Services.Networks
{
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IInboundEmailMessagesService
    {
        int AddMandrillEmailLog(Models.InboundEmailMessage inboundEmailMessage, string nameInvariant, DateTime utcNow, bool succeed);
        void LinkMandrillLogToWallItem(int logId, int itemId, bool newItem, ItemCaptureResult type);
    }
}
